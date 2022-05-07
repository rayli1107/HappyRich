using PlayerInfo;
using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private Profession[] _professions;
    [SerializeField]
    private Profession _professionTutorial;
    [SerializeField]
    private Profession[] _partTimeJobs;
    [SerializeField]
    private float _applyOldJobSuccessChance = 0.5f;
    [SerializeField]
    private float _applyNewJobSuccessChance = 0.25f;
    [SerializeField]
    private Vector2 _jobBonusMultiplier = new Vector2(0.1f, 0.4f);
    [SerializeField]
    private float _jobBonusIncrement = 0.1f;
    [SerializeField]
    private int _numberOfChoices = 2;
    [SerializeField]
    private int _maxAllowedJobs = 2;
#pragma warning restore 0649

/*    public float applyOldJobSuccessChance => _applyOldJobSuccessChance;
    public float applyNewJobSuccessChance => _applyNewJobSuccessChance;
*/
    public int maxAllowedJobs => _maxAllowedJobs;

    private Profession _currentFullTime;
    private List<Profession> _currentPartTimes;

    public static JobManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _currentPartTimes = new List<Profession>();
    }

    public List<Profession> GetInitialProfessionList(System.Random random)
    {
        List<Profession> professions = new List<Profession>();
        if (TutorialManager.Instance.tutorialEnabled)
        {
            professions.Add(_professionTutorial);
        }
        else
        {
            professions.AddRange(_professions);
        }
        return professions;
    }

    public Profession FindInitialProfession(System.Random random)
    {
        return _professions[random.Next(_professions.Length)];
    }

    public void OnPlayerTurnStart(System.Random random)
    {
        _currentFullTime = _professions[random.Next(_professions.Length)];

        List<Profession> availableJobs = new List<Profession>(_partTimeJobs);
        _currentPartTimes.Clear();
        while (_currentPartTimes.Count < _numberOfChoices && availableJobs.Count > 0)
        {
            int index = random.Next(availableJobs.Count);
            _currentPartTimes.Add(availableJobs[index]);
            availableJobs.RemoveAt(index);
        }
    }

    public List<Profession> GetAvailableNewJobs(Player player)
    {
        List<Profession> results = new List<Profession>();
        if (!player.jobs.Exists(j => j.fullTime))
        {
            results.Add(_currentFullTime);
        }
        results.AddRange(_currentPartTimes);
        return results;
    }

    public int GetJobBonus(Profession job, System.Random random)
    {
        int increment = Mathf.FloorToInt(job.salary * _jobBonusIncrement);
        int low = Mathf.FloorToInt(job.salary * _jobBonusMultiplier.x) / increment;
        int high = Mathf.FloorToInt(job.salary * _jobBonusMultiplier.y) / increment;
        return random.Next(low, high + 1) * increment;
    }

    public float GetJobSuccessChance(Player player, Profession job)
    {
        return player.oldJobs.Contains(job) ?
            _applyOldJobSuccessChance :
            _applyNewJobSuccessChance;
    }
}