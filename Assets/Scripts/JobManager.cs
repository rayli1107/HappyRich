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
#pragma warning restore 0649

    public static JobManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
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
}