﻿using Assets;
using ScriptableObjects;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Localization : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private string _locale = "en-US";
    [SerializeField]
    private Color _colorPositive = Color.green;
    [SerializeField]
    private Color _colorNegative = Color.red;
    [SerializeField]
    private Color _colorJob = Color.cyan;
    [SerializeField]
    private Color _colorRealEstate = Color.yellow;
    [SerializeField]
    private Color _colorBusiness = new Color(1f, 0.8f, 0f);
    [SerializeField]
    private Color _colorName = Color.blue;
    [SerializeField]
    private Color _colorFranchise = Color.blue;
    [SerializeField]
    private Color _colorPlayerState = Color.white;
    [SerializeField]
    private Color _colorWarning = new Color(255, 160, 0);
    [SerializeField]
    private Color _colorSkill = Color.yellow;
    [SerializeField]
    private Color _colorStock = Color.blue;
    [SerializeField]
    private Color _colorSpecialist = Color.green;
    [SerializeField]
    private Color _colorLuxury = new Color(255, 215, 0);
#pragma warning restore 0649

    public static Localization Instance { get; private set; }
    public Color colorNegative => _colorNegative;
    public Color colorAsset => _colorRealEstate;

    private CultureInfo _cultureInfo;
    private void Awake()
    {
        Instance = this;
        Debug.LogFormat("Current culture: {0}", CultureInfo.CurrentCulture.Name);
        _cultureInfo = new CultureInfo(_locale);
    }

    public string colorWrap(string s, Color color)
    {
        return string.Format(
            "<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), s);
    }

    public string GetRealEstateDescription(RealEstateProfile profile)
    {
        return colorWrap(profile.description, _colorRealEstate);
    }

    public string GetRealEstateDescription(string description)
    {
        return colorWrap(description, _colorRealEstate);
    }

    public string GetPlayerState(string label)
    {
        return colorWrap(label, _colorPlayerState);
    }

    public string GetPlayerState(PlayerState.AbstractPlayerState state)
    {
        return GetPlayerState(state.name);
    }

    public string GetRealEstateLabel(RealEstateTemplate template)
    {
        return colorWrap(template.label, _colorRealEstate);
    }

    public string GetBusinessDescription(string description)
    {
        return colorWrap(description, _colorBusiness);
    }

    public string GetWarning(string message)
    {
        return colorWrap(message, _colorWarning);
    }

    public string GetJobName(Profession job)
    {
        return colorWrap(job.professionName, _colorJob);
    }

    public string GetLuxuryItem(LuxuryItemProfile item)
    {
        return GetLuxuryItem(item.itemName);
    }

    public string GetLuxuryItem(string name)
    {
        return colorWrap(name, _colorLuxury);
    }

    private string GetCurrencyFactor(int amount, int unit, string prefix)
    {
        if (amount % unit == 0)
        {
            return (amount / unit).ToString("C0", _cultureInfo) + prefix;
        }
        else
        {
            return ((float)amount / unit).ToString("C1", _cultureInfo) + prefix;
        }
    }

    public string GetStockName(AbstractStock stock)
    {
        return colorWrap(stock.name, _colorStock);
    }

    public string GetName(string name)
    {
        return colorWrap(name, _colorName);
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
            return amount.ToString("C0", _cultureInfo);
        }
    }

    public string GetCurrencyPlain(int amount, bool showPositiveSign=false)
    {
        if (amount < 0)
        {
            return "-" + GetAbsCurrency(-1 * amount);
        }
        else if (showPositiveSign)
        {
            return "+" + GetAbsCurrency(amount);
        }
        else
        {
            return GetAbsCurrency(amount);
        }
    }

    public string GetCurrency(int amount, bool flipped=false, bool showPositiveSign=false)
    {
        bool positive = amount >= 0;
        if (flipped)
        {
            positive = !positive;
        }

        Color c = positive ? _colorPositive : _colorNegative;
        if (amount < 0)
        {
            return colorWrap(GetCurrencyPlain(amount, showPositiveSign), c);
        }
        return colorWrap(GetCurrencyPlain(amount, showPositiveSign), c);
    }

    public string GetPercentPlain(float pct, bool showPositive=true)
    {
        if (pct >= 0 && showPositive)
        {
            return "+" + pct.ToString("#0%", _cultureInfo);
        }
        return pct.ToString("#0%", _cultureInfo);
    }

    public string GetPercent(float pct, bool showPositive=true)
    {
        if (pct >= 0)
        {
            return colorWrap(GetPercentPlain(pct, showPositive), _colorPositive);
        }
        return colorWrap(GetPercentPlain(pct, showPositive), _colorNegative);
    }

    public string GetValueAsChange(int value)
    {
        if (value >= 0)
        {
            return colorWrap("+" + value.ToString(), _colorPositive);
        }
        else
        {
            return colorWrap(value.ToString(), _colorNegative);
        }
    }

    public string GetSkill(SkillInfo skill)
    {
        return colorWrap(skill.skillName, _colorSkill);
    }

    public string GetSpecialist(SpecialistInfo info)
    {
        return colorWrap(info.specialistName, _colorSpecialist);
    }

    public string GetAsset(AbstractAsset asset)
    {
        return colorWrap(asset.name, colorAsset);
    }

    public string GetLiability(AbstractLiability liability)
    {
        return colorWrap(liability.longName, colorNegative);
    }

    public string Highlight(string s)
    {
        return string.Format("<b>{0}</b>", s);
    }

    public string GetIncomeRange(Vector2Int range, bool flipped = false)
    {
        if (range.x == range.y)
        {
            return GetCurrency(range.x, flipped);
        }
        else
        {
            return string.Format(
                "{0} ~ {1}",
                GetCurrency(range.x, flipped),
                GetCurrency(range.y, flipped));
        }
    }
}
