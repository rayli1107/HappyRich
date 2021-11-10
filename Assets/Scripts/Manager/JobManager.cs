using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private Profession[] _professions;
    [SerializeField]
    private Profession[] _partTimeJobs;
    [SerializeField]
    private float _applyOldJobSuccessChance = 0.5f;
    [SerializeField]
    private Vector2 _jobBonusMultiplier = new Vector2(0.1f, 0.4f);
    [SerializeField]
    private float _jobBonusIncrement = 0.1f;
#pragma warning restore 0649

    public float applyOldJobSuccessChance => _applyOldJobSuccessChance;
    public static JobManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public List<Profession> GetInitialProfessionList(System.Random random)
    {
        List<Profession> professions = new List<Profession>();
        professions.AddRange(_professions);
        return professions;
    }

    public Profession FindInitialProfession(System.Random random)
    {
        return _professions[random.Next(_professions.Length)];
    }

    public Profession FindJob(System.Random random, bool hasFullTime)
    {
        List<Profession> jobs = new List<Profession>(_partTimeJobs);
        if (!hasFullTime)
        {
            foreach (Profession job in _professions)
            {
                if (job.searchable)
                {
                    jobs.Add(job);
                }
            }
        }
        return jobs[random.Next(jobs.Count)];
    }

    public int GetJobBonus(Profession job, System.Random random)
    {
        int increment = Mathf.FloorToInt(job.salary * _jobBonusIncrement);
        int low = Mathf.FloorToInt(job.salary * _jobBonusMultiplier.x) / increment;
        int high = Mathf.FloorToInt(job.salary * _jobBonusMultiplier.y) / increment;
        return random.Next(low, high + 1) * increment;
    }
}