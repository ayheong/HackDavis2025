import pandas as pd
import matplotlib.pyplot as plt
from statsmodels.graphics.tsaplots import plot_acf, plot_pacf
from pmdarima import auto_arima

# List of CSVs (must be in the same folder as this script)
diseases = ["heart_disease", "stroke", "diabetes", "suicide", "cancer"]

results = {}

for disease in diseases:
    print(f"\n=== {disease.upper()} ===")

    # Load CSV
    df = pd.read_csv(f"{disease}.csv")
    df.columns = df.columns.str.strip()  # Clean up any whitespace

    df['Year'] = pd.to_datetime(df['Year'], format='%Y')
    df.set_index('Year', inplace=True)
    series = df['Total Deaths']

    # Plot the time series
    plt.figure(figsize=(10, 4))
    plt.plot(series)
    plt.title(f"{disease.title()} - Total Deaths Over Time")
    plt.xlabel("Year")
    plt.ylabel("Total Deaths")
    plt.tight_layout()
    plt.show()

    # Auto-set number of lags to max allowed by sample size
    max_lags = max(1, int(len(series) / 2) - 1)  # -1 just to be safe

    # Plot ACF and PACF
    fig, ax = plt.subplots(1, 2, figsize=(12, 4))
    plot_acf(series, lags=max_lags, ax=ax[0])
    plot_pacf(series, lags=max_lags, ax=ax[1])
    ax[0].set_title(f"{disease.title()} - ACF")
    ax[1].set_title(f"{disease.title()} - PACF")
    plt.tight_layout()
    plt.show()

    # Run auto_arima to find the best (p,d,q)
    stepwise_model = auto_arima(
        series,
        start_p=0, start_q=0,
        max_p=5, max_q=5,
        seasonal=False,
        d=None,  # Let auto_arima decide
        trace=True,
        error_action='ignore',
        suppress_warnings=True
    )

    print(f"Suggested ARIMA order for {disease}: {stepwise_model.order}")
    results[disease] = stepwise_model.order

# Print all results at the end
print("\n==== FINAL SUGGESTED ARIMA ORDERS ====")
for disease, order in results.items():
    print(f"{disease}: {order}")
