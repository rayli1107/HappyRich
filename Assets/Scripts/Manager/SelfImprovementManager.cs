using PlayerState;
using System.Collections.Generic;
using UnityEngine;

public class SelfImprovementManager : MonoBehaviour
{
    public static SelfImprovementManager Instance { get; private set; }
    private List<AbstractPlayerState> _selfReflectionStates;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        _selfReflectionStates = new List<AbstractPlayerState>();
        _selfReflectionStates.Add(new Frugality());
        _selfReflectionStates.Add(new Minimalism());
    }

    public AbstractPlayerState GetSelfReflectionState(System.Random random)
    {
        if (_selfReflectionStates == null || _selfReflectionStates.Count == 0)
        {
            return null;
        }

        int index = random.Next(_selfReflectionStates.Count);
        AbstractPlayerState state = _selfReflectionStates[index];
        _selfReflectionStates.RemoveAt(index);
        return state;
    }
}

