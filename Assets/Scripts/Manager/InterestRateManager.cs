using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestRateManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private int _studentLoanRate = 3;
    [SerializeField]
    private int _autoLoanRate = 3;
    [SerializeField]
    private int _realEstateLoanRate = 4;
    [SerializeField]
    private int _businessLoanRate = 4;
    [SerializeField]
    private int _distressedLoanRate = 10;
    [SerializeField]
    private int _personalLoanRate = 15;
    [SerializeField]
    private int _defaultPrivateLoanRate = 6;
    [SerializeField]
    private int _startupBusinessLoanRate = 10;
    [SerializeField]
    private int _startupPrivateLoanRate = 20;
#pragma warning restore 0649

    public int studentLoanRate => _studentLoanRate;
    public int autoLoanRate =>_autoLoanRate;
    public int realEstateLoanRate => _realEstateLoanRate;
    public int personalLoanRate => _personalLoanRate;
    public int defaultPrivateLoanRate => _defaultPrivateLoanRate;
    public int distressedLoanRate => _distressedLoanRate;
    public int businessLoanRate => _businessLoanRate;
    public int startupBusinessLoanRate => _startupBusinessLoanRate;
    public int startupPrivateLoanRate => _startupPrivateLoanRate;

    public static InterestRateManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}
