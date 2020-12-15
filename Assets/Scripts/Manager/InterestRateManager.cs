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
    private int _personalLoanRate = 15;
#pragma warning restore 0649

    public int studentLoanRate { get { return _studentLoanRate; } }
    public int autoLoanRate { get { return _autoLoanRate; } }
    public int realEstateLoanRate { get { return _realEstateLoanRate; } }
    public int personalLoanRate { get { return _personalLoanRate; } }

    public static InterestRateManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}
