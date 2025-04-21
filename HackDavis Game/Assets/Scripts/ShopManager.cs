using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public float money = 50.0f;
    public TextMeshProUGUI money_text_box;
    public ShopButton[] shop_buttons;
    public DonateButton donate_button;

    public ForecastManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IncrementMoney();
        SetMoneyText();
    }

    public static string AddCommas(float num)
    {
        // Turn the float into a string
        string number = num.ToString();

        // Reverse the string using a loop (avoiding char[])
        StringBuilder reversed = new StringBuilder();
        for (int i = number.Length - 1; i >= 0; i--)
        {
            reversed.Append(number[i]);
        }

        // Add commas every 3 digits
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < reversed.Length; i++)
        {
            if (i > 0 && i % 3 == 0)
                result.Append(',');

            result.Append(reversed[i]);
        }

        // Reverse the final result using string instead of char[]
        StringBuilder finalString = new StringBuilder();
        for (int i = result.Length - 1; i >= 0; i--)
        {
            finalString.Append(result[i]);
        }

        return finalString.ToString();
    }

    public void IncrementMoney()
    {
        foreach (ShopButton shop_button in shop_buttons)
        {
            if (shop_button.index > 1)
            {
                money += shop_button.amount * Mathf.Pow(5, shop_button.index) * Time.deltaTime;
            }
        }
    }

    public void SetMoneyText()
    {
        //money_text_box.text = "$" + AddCommas(money);
        money_text_box.text = "$" + money.ToString("N0");
    }

    public void ClickButton(ShopButton button)
    {
        if (money >= button.price)
        {
            BuyItem(button);
        }
        else
        {
            // TODO make this print to a text box that is either always there or pops up when you try to
            // do something, or just don't do anything since the button will be grayed out
            Debug.Log("You don't have enough funds for " + button.item_name);
        }
    }

    public void BuyItem(ShopButton button)
    {
        money -= button.price;
        Debug.Log("You purchased " + button.item_name + " for $" + button.price);
        manager.ReduceValues(button.reduce_values);
        if (button.index == 1)
        {
            donate_button.amount += Mathf.Ceil(0.01f * button.price * button.increment_factor);
            //donate_button.textBox.text = "$" + AddCommas(donate_button.amount);
            donate_button.textBox.text = "$" + donate_button.amount.ToString("N0");
        }
        button.Purchase();
        // TODO BUY THE ITEM
    }

    public void Donate(DonateButton donate_button)
    {
        Debug.Log("DONATING!");
        money += donate_button.amount;
        //money_text_box.text = "$" + AddCommas(money);
        money_text_box.text = "$" + money.ToString("N0");
    }
}
