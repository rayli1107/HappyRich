using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ScriptableObjects.Profession startingProfession;
    public int defaultHappiness = 50;

    public static GameManager Instance { get; private set; }
    public Player player { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = new Player(startingProfession, defaultHappiness);
        StartCoroutine(WaitAndUpdateUI());
    }

    private IEnumerator WaitAndUpdateUI()
    {
        while (!UI.UIManager.Instance.ready)
        {
            yield return null;
        }
        UI.UIManager.Instance.UpdatePlayerInfo(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
