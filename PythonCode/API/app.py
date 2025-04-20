from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional
import pandas as pd
from statsmodels.tsa.arima.model import ARIMA

app = FastAPI()

# Enable CORS for Unity or browser-based frontends
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Change to specific origin in production
    allow_methods=["*"],
    allow_headers=["*"],
)

# Request schema: time series + optional ARIMA order
class ForecastRequest(BaseModel):
    data: List[float]
    order: Optional[List[int]] = [1, 1, 1]  # Default: ARIMA(1,1,1)

# Response schema
class ForecastResponse(BaseModel):
    forecast: float

@app.post("/forecast", response_model=ForecastResponse)
def forecast_next_value(request: ForecastRequest):
    if len(request.data) < max(request.order):  # Prevent too-short series
        return {"forecast": request.data[-1]}  # Just return last value if too short

    series = pd.Series(request.data)
    model = ARIMA(series, order=tuple(request.order))
    model_fit = model.fit()

    next_forecast = model_fit.forecast(steps=1).iloc[0]
    return ForecastResponse(forecast=round(float(next_forecast), 2))
