using TMPro;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public int index;
    public TextMeshProUGUI item_name_text;
    public TextMeshProUGUI price_text;
    public TextMeshProUGUI amount_text;
    public string item_name;
    public float price;
    public int amount;
    public float increment_factor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        item_name_text.text = item_name;
        price_text.text = "$" + price;
        amount_text.text = 0.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Purchase()
    {
        UpdateAmount();
        UpdatePrice();
    }

    public void UpdateAmount(int factor = 1)
    {
        amount += factor;
        amount_text.text = amount.ToString();
    }

    public void UpdatePrice(int factor = 1)
    {
        for (int i = 0; i < factor; i++)
        {
            price = Mathf.Ceil(price * increment_factor);
        }
        price_text.text = "$" + price;
    }
}
