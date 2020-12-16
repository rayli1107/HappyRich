using ScriptableObjects;
using UnityEngine;

public class InvestmentPartner
{
    public string name { get; private set; }
    public int cash;
    public int duration;

    public InvestmentPartner(string name, int cash, int duration)
    {
        this.name = name;
        this.cash = cash;
        this.duration = duration;
    }
}

public class InvestmentPartnerManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private InvestmentPartnerProfile[] _profiles;
    [SerializeField]
    private string[] _names;
    [SerializeField]
    private int _defaultDuration = 10;
#pragma warning restore 0649

    public static InvestmentPartnerManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;        
    }

    private InvestmentPartner GetPartnerFromProfile(
        InvestmentPartnerProfile profile, System.Random random)
    {
        int min = profile.cashRange.x / profile.cashIncrement;
        int max = profile.cashRange.y / profile.cashIncrement;
        int cash = random.Next(min, max + 1) * profile.cashIncrement;
        string name = _names[random.Next(_names.Length)];
        return new InvestmentPartner(name, cash, _defaultDuration);
    }

    public InvestmentPartner GetPartner(System.Random random)
    {
        int totalWeight = 0;
        foreach (InvestmentPartnerProfile profile in _profiles)
        {
            totalWeight += profile.distributionWeight;
        }

        int index = random.Next(totalWeight);
        foreach (InvestmentPartnerProfile profile in _profiles)
        {
            index -= profile.distributionWeight;
            if (index < 0)
            {
                return GetPartnerFromProfile(profile, random);
            }
        }
        return GetPartnerFromProfile(_profiles[0], random);
    }
}
