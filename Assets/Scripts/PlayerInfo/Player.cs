using Actions;
using Assets;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.PlayerDetails;
using UnityEngine;

public partial class GameInstanceData
{
    public PlayerInfo.Player playerData;
}

namespace PlayerInfo
{
    [Serializable]
    public class Spouse
    {
        [SerializeField]
        private int _additionalIncome;
        public int additionalIncome => _additionalIncome;

        [SerializeField]
        private int _additionalExpense;
        public int additionalExpense => _additionalExpense;

        [SerializeField]
        private int _additionalHappiness;
        public int additionalHappiness => _additionalHappiness;

        public Spouse(int income, int expense, int happiness)
        {
            _additionalIncome = income;
            _additionalExpense = expense;
            _additionalHappiness = happiness;
        }
    }

    [Serializable]
    public class Player : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<string> _jobs;
        public List<Profession> jobs { get; private set; }

        [SerializeField]
        private List<string> _oldJobs;
        public List<Profession> oldJobs { get; private set; }

        public Portfolio portfolio { get; private set; }
        public int cash => portfolio.cash;

        [SerializeField]
        private int _personalExpenses;

        [SerializeField]
        private int _costPerChild;

        public int expenseModifier
        {
            get
            {
                int modifier = 100;
                foreach (AbstractPlayerState state in states)
                {
                    modifier += state.expenseModifier;
                }
                return modifier;
            }
        }
        public int personalExpenses => (_personalExpenses * expenseModifier) / 100;
        public int costPerChild => (_costPerChild * expenseModifier) / 100;
        public int numChild;
        public int age;
        public int meditatedCount = 0;

        [SerializeField]
        public Spouse spouse;

        public int maxHappiness => 100;

        public int happiness
        {
            get
            {
                int happiness = _defaultHappiness;
                foreach (AbstractPlayerState state in states)
                {
                    happiness += state.happinessModifier;
                }
                return happiness;
            }
        }

        [SerializeField]
        private string _personality;
        public Personality personality;


        private List<AbstractPlayerState> _passiveStates;

        [SerializeField]
        private List<string> _selfReflectionStates;
        public List<SelfReflectionState> selfReflectionStates { get; private set; }

        [SerializeField]
        private List<TimedPlayerStateData> _timedPlayerStates;
        public List<TimedPlayerState> timedPlayerStates { get; private set; }
        public List<AbstractPlayerState> states
        {
            get
            {
                List<AbstractPlayerState> states = new List<AbstractPlayerState>();
                states.Add(personality);
                states.AddRange(_passiveStates);
                states.AddRange(selfReflectionStates);
                states.AddRange(timedPlayerStates);
                return states;
            }
        }

        [SerializeField]
        private int _defaultHappiness;
        public int defaultHappiness => _defaultHappiness;

        public List<InvestmentPartner> contacts { get; private set; }
        public List<SkillInfo> skills { get; private set; }
        public List<SpecialistInfo> specialists { get; private set; }

        public Player(Profession profession, int defaultHappiness)
        {
            oldJobs = new List<Profession>();
            jobs = new List<Profession>();
            jobs.Add(profession);
            portfolio = new Portfolio(profession);
            skills = new List<SkillInfo>();
            specialists = new List<SpecialistInfo>();

            _personalExpenses = profession.personalExpenses;
            _costPerChild = profession.costPerChild;
            numChild = 0;
            age = profession.startingAge;
            _defaultHappiness = defaultHappiness;
            meditatedCount = 0;

            contacts = new List<InvestmentPartner>();

            _passiveStates = new List<AbstractPlayerState>()
            {
                new OneJobState(this),
                new TwoJobState(this),
                new MarriageState(this),
                new ChildrenState(this),
                new AssetManagementStress(this),
            };

            selfReflectionStates = new List<SelfReflectionState>();
            timedPlayerStates = new List<TimedPlayerState>();
        }

        public void OnPlayerTurnStart(System.Random random)
        {
            portfolio.OnTurnStart(random);
            UpdateContacts();
            foreach (AbstractPlayerState state in states)
            {
                state.OnPlayerTurnStart();
            }
        }

        private void UpdateContacts()
        {
            List<InvestmentPartner> newContacts = new List<InvestmentPartner>();
            foreach (InvestmentPartner contact in contacts)
            {
                contact.OnTurnStart();
                if (contact.duration > 0)
                {
                    newContacts.Add(contact);
                }
            }
            contacts = newContacts;
        }

        public void DistributeCashflow(Action callback, System.Random random)
        {
            Localization local = Localization.Instance;

            Snapshot snapshot = new Snapshot(this);
            EventLogManager.Instance.Log(
                () => UI.UIManager.Instance.ShowIncomeExpenseStatusPanel(snapshot),
                string.Format("Distribute Cashflow {0}", local.GetIncomeRange(snapshot.totalCashflowRange)));

            int cashflow = snapshot.GetActualIncome(random);
            portfolio.AddCash(cashflow);
            callback?.Invoke();
        }

        public Profession GetMainJob()
        {
            foreach (Profession job in jobs)
            {
                if (job.fullTime)
                {
                    return job;
                }
            }
            return null;
        }

        public void AddJob(Profession job)
        {
            EventLogManager.Instance.LogFormat(
                "Add Job: {0}", job.professionName);
            jobs.Add(job);
            oldJobs.Remove(job);
        }

        public void LoseJob(Profession job)
        {
            EventLogManager.Instance.LogFormat(
                "Lose Job: {0}", job.professionName);
            if (jobs.Remove(job) && !oldJobs.Contains(job))
            {
                oldJobs.Add(job);
            }
        }

        public void AddSelfReflectionState(SelfReflectionState state)
        {
            EventLogManager.Instance.LogFormat(
                "Add Self Reflection State: {0}", state.name);
            selfReflectionStates.Add(state);
        }

        public void AddTimedState(TimedPlayerState state)
        {
            EventLogManager.Instance.LogFormat(
                "Add Mental State: {0}", state.name);
            timedPlayerStates.RemoveAll(s => s.GetType() == state.GetType());
            timedPlayerStates.Add(state);
        }

        public void RemoveMentalState(TimedPlayerState state)
        {
            if (timedPlayerStates.Remove(state))
            {
                EventLogManager.Instance.LogFormat(
                    "Remove Mental State: {0}", state.name);
            }
        }

        public void RemoveMentalState<T>()
        {
            if (timedPlayerStates.RemoveAll(s => s.GetType() == typeof(T)) > 0)
            {
                EventLogManager.Instance.LogFormat(
                    "Remove Mental States: {0}", typeof(T).Name);
            }
        }

        public SkillInfo GetSkillInfo(SkillType skillType)
        {
            foreach (SkillInfo skillInfo in skills)
            {
                if (skillInfo.skillType == skillType)
                {
                    return skillInfo;
                }
            }
            return null;
        }

        public bool HasSkill(SkillType skillType) => GetSkillInfo(skillType) != null;

        public void AddSkill(SkillInfo skillInfo)
        {
            if (!HasSkill(skillInfo.skillType))
            {
                EventLogManager.Instance.LogFormat(
                    "Learned Skill: {0}", skillInfo.skillName);
                skills.Add(skillInfo);
            }
        }

        public SpecialistInfo GetSpecialistInfo(SpecialistType specialistType)
        {
            foreach (SpecialistInfo specialistInfo in specialists)
            {
                if (specialistInfo.specialistType == specialistType)
                {
                    return specialistInfo;
                }
            }
            return null;
        }

        public bool HasSpecialist(SpecialistType specialistType) => GetSpecialistInfo(specialistType) != null;

        public void AddSpecialist(SpecialistInfo specialistInfo)
        {
            if (!HasSpecialist(specialistInfo.specialistType))
            {
                EventLogManager.Instance.LogFormat(
                    "Add Specialist: {0}", specialistInfo.specialistName);
                specialists.Add(specialistInfo);
            }
        }


        public List<InvestmentPartner> GetDebtPartners()
        {
            return GetPartners(true, false, false);
        }

        public List<InvestmentPartner> GetEquityPartners()
        {
            return GetPartners(false, false, true);
        }

        public List<InvestmentPartner> GetPartners(
            bool showLowRiskPartners = true,
            bool showMediumRiskPartners = true,
            bool showHighRiskPartners = true)
        {
            List<InvestmentPartner> ret = new List<InvestmentPartner>();

            foreach (InvestmentPartner partner in contacts)
            {
                bool show = false;
                switch (partner.riskTolerance)
                {
                    case RiskTolerance.kHigh:
                        show = showHighRiskPartners;
                        break;
                    case RiskTolerance.kMedium:
                        show = showMediumRiskPartners;
                        break;
                    case RiskTolerance.kLow:
                        show = showLowRiskPartners;
                        break;
                }
                if (show)
                {
                    ret.Add(partner);
                }
            }
            return ret;
        }

        public void OnBeforeSerialize() 
        {
            _jobs = jobs.ConvertAll(j => j.professionName);
            _oldJobs = oldJobs.ConvertAll(j => j.professionName);
            _selfReflectionStates = selfReflectionStates.ConvertAll(s => s.name);
            _personality = personality.name;
        }

        public void OnAfterDeserialize()
        {
            jobs = _jobs.ConvertAll(
                n => JobManager.Instance.GetJobByName(n, true));
            oldJobs = _oldJobs.ConvertAll(
                n => JobManager.Instance.GetJobByName(n, true));
            selfReflectionStates = _selfReflectionStates.ConvertAll(
                n => MentalStateManager.Instance.GetSelfReflectionStateByName(this, n, true));
            personality =
                MentalStateManager.Instance.GetPersonalityByName(this, _personality, true);

        }
    }
}
