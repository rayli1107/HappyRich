using InvestmentPartnerInfo;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class GameInstanceData
{
    public PlayerInfo.PlayerData playerData;
}

namespace PlayerInfo
{
    [Serializable]
    public class PlayerData
    {
        public int personalExpenses;
        public int costPerChild;
        public int numChild;
        public int age;
        public int meditatedCount;
        public int defaultHappiness;

        [SerializeField]
        public Spouse spouse;

        // Custom Serialization
        public List<string> jobs;
        public List<string> oldJobs;
        public string personality;
        public List<string> selfReflectionStates;
        public List<TimedPlayerStateData> timedPlayerStates;
        public List<string> skills;
        public List<string> specialists;
        public List<int> contacts;

        public void Initialize(
            Profession profession, int defaultHappiness, string personality)
        {
            personalExpenses = profession.personalExpenses;
            costPerChild = profession.costPerChild;
            numChild = 0;
            age = profession.startingAge;
            meditatedCount = 0;
            this.defaultHappiness = defaultHappiness;
            spouse = null;

            jobs = new List<string>() { profession.professionName };
            oldJobs = new List<string>();
            this.personality = personality;
            selfReflectionStates = new List<string>();
            timedPlayerStates = new List<TimedPlayerStateData>();
            skills = new List<string>();
            specialists = new List<string>();
            contacts = new List<int>();
        }
    }

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

    public class Player
    {
        private PlayerData _playerData;

        public Portfolio portfolio { get; private set; }
        public List<Profession> jobs { get; private set; }
        public List<Profession> oldJobs { get; private set; }
        public int cash => portfolio.cash;
        public Spouse spouse
        {
            get => _playerData.spouse;
            set { _playerData.spouse = value; }
        }

        public int age
        {
            get => _playerData.age;
            set { _playerData.age = value; }
        }

        public int numChild
        {
            get => _playerData.numChild;
            set { _playerData.numChild = value; }
        }

        public int meditatedCount
        {
            get => _playerData.meditatedCount;
            set { _playerData.meditatedCount = value; }
        }

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
        public int personalExpenses => _playerData.personalExpenses * expenseModifier / 100;
        public int costPerChild => _playerData.costPerChild * expenseModifier / 100;

        public int maxHappiness => 100;

        public int happiness
        {
            get
            {
                int happiness = defaultHappiness;
                foreach (AbstractPlayerState state in states)
                {
                    happiness += state.happinessModifier;
                }
                return happiness;
            }
        }

        public Personality personality;
        private List<AbstractPlayerState> _passiveStates;
        public List<SelfReflectionState> selfReflectionStates { get; private set; }

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

        public int defaultHappiness => _playerData.defaultHappiness;

        public List<InvestmentPartner> contacts { get; private set; }
        public List<SkillInfo> skills { get; private set; }
        public List<SpecialistInfo> specialists { get; private set; }

        public Player(PlayerData playerData, Portfolio portfolio)
        {
            _playerData = playerData;
            this.portfolio = portfolio;

            _passiveStates = new List<AbstractPlayerState>()
            {
                new OneJobState(this),
                new TwoJobState(this),
                new MarriageState(this),
                new ChildrenState(this),
                new AssetManagementStress(this),
            };

            LoadData();
        }

        public void SaveData()
        {
            _playerData.jobs = jobs.ConvertAll(j => j.professionName);
            _playerData.oldJobs = oldJobs.ConvertAll(j => j.professionName);
            _playerData.selfReflectionStates = selfReflectionStates.ConvertAll(s => s.name);
            _playerData.personality = personality.name;
            _playerData.timedPlayerStates = timedPlayerStates.ConvertAll(s => s.GetData());
            _playerData.skills = skills.ConvertAll(SkillManager.Instance.GetSkillLabel);
            _playerData.specialists = specialists.ConvertAll(SpecialistManager.Instance.GetSpecialistLabel);
            _playerData.contacts = contacts.ConvertAll(c => c.partnerId);
        }

        public void LoadData()
        {
            jobs = _playerData.jobs?.ConvertAll(
                n => JobManager.Instance.GetJobByName(n, true));
            oldJobs = _playerData.oldJobs?.ConvertAll(
                n => JobManager.Instance.GetJobByName(n, true));
            selfReflectionStates = _playerData.selfReflectionStates?.ConvertAll(
                n => MentalStateManager.Instance.GetSelfReflectionStateByName(this, n, true));
            personality = MentalStateManager.Instance.GetPersonalityByName(
                this, _playerData.personality, true);
            timedPlayerStates = _playerData.timedPlayerStates?.ConvertAll(
                s => MentalStateManager.Instance.CreateTimedStateFromData(this, s, true));
            skills = _playerData.skills?.ConvertAll(
                SkillManager.Instance.GetSkillByLabel);
            specialists = _playerData.specialists?.ConvertAll(
                SpecialistManager.Instance.GetSpecialistInfoByLabel);
            contacts = _playerData.contacts?.ConvertAll(
                InvestmentPartnerManager.Instance.GetPartnerById);
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


    }
}
