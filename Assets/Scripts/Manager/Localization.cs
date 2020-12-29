using ScriptableObjects;
using UnityEngine;

public class Localization : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private Color _colorPositive = Color.green;
    [SerializeField]
    private Color _colorNegative = Color.red;
    [SerializeField]
    private Color _colorJob = Color.cyan;
    [SerializeField]
    private Color _colorRealEstate = Color.yellow;
    [SerializeField]
    private Color _colorName = Color.blue;
    [SerializeField]
    private Color _colorPlayerState = Color.white;
    [SerializeField]
    private Color _colorWarning = new Color(255, 160, 0);
#pragma warning restore 0649

    public static Localization Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private string colorWrap(string s, Color color)
    {
        return string.Format(
            "<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), s);
    }

    public string GetRealEstateDescription(RealEstateProfile profile)
    {
        return colorWrap(profile.description, _colorRealEstate);
    }

    public string GetPlayerState(PlayerState.AbstractPlayerState state)
    {
        return colorWrap(state.name, _colorPlayerState);
    }

    public string GetRealEstateLabel(RealEstateProfile profile)
    {
        return colorWrap(profile.label, _colorRealEstate);
    }

    public string GetWarning(string message)
    {
        return colorWrap(message, _colorWarning);
    }
    public string GetJobName(Profession job)
    {
        return colorWrap(job.professionName, _colorJob);
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
            return amount.ToString("C0");
        }
    }
    public string GetCurrency(int amount, bool flipped=false)
    {
        bool positive = amount >= 0;
        if (flipped)
        {
            positive = !positive;
        }

        Color c = positive ? _colorPositive : _colorNegative;
        if (amount < 0)
        {
            return colorWrap("-" + GetAbsCurrency(-1 * amount), c);
        }
        return colorWrap(GetAbsCurrency(amount), c);
    }

    public string GetPercentPlain(float pct, bool showPositive=true)
    {
        if (pct >= 0 && showPositive)
        {
            return "+" + pct.ToString("#0%");
        }
        return pct.ToString("#0%");
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
}
