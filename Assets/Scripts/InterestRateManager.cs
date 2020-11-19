using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestRateManager : MonoBehaviour
{
    public int studentLoanRate = 3;
    public int autoLoanRate = 3;
    public int realEstateLoanRate = 4;
    public int personalLoanRate = 10;

    public static InterestRateManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}
