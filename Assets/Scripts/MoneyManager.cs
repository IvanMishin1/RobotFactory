using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public long Money
    { set {_money = value; RefreshMoneyDisplay();} get { return _money; } }
    private long _money = 0;
    public TMP_Text moneyText;

    public void AddMoney(long amount)
    {
        _money += amount;
        RefreshMoneyDisplay();
    }

    public bool SpendMoney(long amount)
    {
        if (_money >= amount)
        {
            _money -= amount;
            RefreshMoneyDisplay();
            return true;
        }
        return false;
    }
    public bool CanAfford(long amount)
    {
        return _money >= amount;
    }

    void RefreshMoneyDisplay()
    {
        moneyText.text = $"Net Gain: {_money}$";
    }
}
