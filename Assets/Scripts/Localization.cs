using ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Localization
{
    public string GetJobName(Profession job)
    {
        return string.Format("<color=#00ffffff>{0}</color>", job.professionName);
    }

    private string GetCurrencyFactor(int amount, int unit, string prefix)
    {
        if (amount % unit == 0)
        {
            return (amount / unit).ToString("C0") + prefix;
        }
        else
        {
            return ((float)amount / unit).ToString("C1") + prefix;
        }
    }

    private string GetAbsCurrency(int amount)
    {
        int m = 1000000;
        int k = 1000;
        if (amount >= m)
        {
            return GetCurrencyFactor(amount, m, "M");
        }
        else if (amount >= k)
        {
            return GetCurrencyFactor(amount, k, "K");
        }
        else
        {
            return amount.ToString("C0");
        }
    }
    public string GetCurrency(int amount)
    {
        if (amount < 0)
        {
            return "-" + GetAbsCurrency(-1 * amount);
        }
        return GetAbsCurrency(amount);
    }

    public string GetPercent(float pct)
    {
        if (pct >= 0)
        {
            return "+" + pct.ToString("#0%");
        }
        return pct.ToString("#0%");
    }
}
