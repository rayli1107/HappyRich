using Actions;
using Assets;
using PlayerState;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.PlayerDetails;
using UnityEngine;

namespace PlayerInfo
{
    public class Spouse
    {
        public int additionalIncome { get; private set; }
        public int additionalExpense { get; private set; }
        public int additionalHappiness { get; private set; }

        public Spouse(int income, int expense, int happiness)
        {
            additionalIncome = income;
            additionalExpense = expense;
            additionalHappiness = happiness;
        }
    }

    public class Player
    {
        public List<Profession> jobs { get; private set; }
        public List<Profession> oldJobs { get; private set; }
        public Portfolio portfolio { get; private set; }
        public int cash => portfolio.cash;

        private int _personalExpenses;
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
        public Spouse spouse;

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
        public List<AbstractPlayerState> passiveStates { get; private set; }
        public List<AbstractPlayerState> mentalStates { get; private set; }
        public List<AbstractPlayerState> states
        {
            get
            {
                List<AbstractPlayerState> states = new List<AbstractPlayerState>();
                states.Add(personality);
                states.AddRange(passiveStates);
                states.AddRange(mentalStates);
                return states;
            }
        }

        public int defaultHappiness { get; private set; }
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
            this.defaultHappiness = defaultHappiness;
            meditatedCount = 0;

            contacts = new List<InvestmentPartner>();

            passiveStates = new List<AbstractPlayerState>()
            {
                new OneJobState(this),
                new TwoJobState(this),
                new MarriageState(this),
                new ChildrenState(this),
                new AssetManagementStress(this),
            };

            mentalStates = new List<AbstractPlayerState>();
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

        public void AddMentalState(AbstractPlayerState state)
        {
            EventLogManager.Instance.LogFormat(
                "Add Mental State: {0}", state.name);
            mentalStates.RemoveAll(s => s.GetType() == state.GetType());
            mentalStates.Add(state);
        }

        public void RemoveMentalState(AbstractPlayerState state)
        {
            if (mentalStates.Remove(state))
            {
                EventLogManager.Instance.LogFormat(
                    "Remove Mental State: {0}", state.name);
            }
        }

        public void RemoveMentalState<T>()
        {
            if (mentalStates.RemoveAll(s => s.GetType() == typeof(T)) > 0)
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
