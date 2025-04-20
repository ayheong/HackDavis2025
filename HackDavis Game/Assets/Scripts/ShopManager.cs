using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public float money = 50.0f;
    public TextMeshProUGUI money_text_box;
    public ShopButton[] shop_buttons;
    public DonateButton donate_button;

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

    public void IncrementMoney()
    {
        foreach (ShopButton shop_button in shop_buttons)
        {
            money += shop_button.amount * shop_button.index * Time.deltaTime;
        }
    }

    public void SetMoneyText()
    {
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
        button.Purchase();
        // TODO BUY THE ITEM
    }

    public void Donate(DonateButton donate_button)
    {
        money += donate_button.amount;
    }
}
