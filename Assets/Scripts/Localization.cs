﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Localization
{
    public string GetCurrency(int amount)
    {
        if (amount < 0)
        {
            return "-" + (-1 * amount).ToString("C0");
        }
        return amount.ToString("C0");
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