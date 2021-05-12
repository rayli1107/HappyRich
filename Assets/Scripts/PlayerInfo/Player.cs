﻿using PlayerState;
using ScriptableObjects;
using System.Collections.Generic;

namespace PlayerInfo
{
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
        public int numChild { get; private set; }
        public int age { get; private set; }

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

        public List<AbstractPlayerState> passiveStates { get; private set; }
        public List<AbstractPlayerState> mentalStates { get; private set; }
        public List<AbstractPlayerState> states
        {
            get
            {
                List<AbstractPlayerState> states = new List<AbstractPlayerState>();
                states.AddRange(passiveStates);
                states.AddRange(mentalStates);
                return states;
            }
        }

        public int defaultHappiness { get; private set; }
        public List<InvestmentPartner> contacts { get; private set; }
        public List<SkillInfo> skills { get; private set; }

        public Player(Profession profession, int defaultHappiness)
        {
            oldJobs = new List<Profession>();
            jobs = new List<Profession>();
            jobs.Add(profession);
            portfolio = new Portfolio(profession);
            skills = new List<SkillInfo>();

            _personalExpenses = profession.personalExpenses;
            _costPerChild = profession.costPerChild;
            numChild = 0;
            age = profession.startingAge;
            this.defaultHappiness = defaultHappiness;

            contacts = new List<InvestmentPartner>();

            passiveStates = new List<AbstractPlayerState>()
        {
            new OneJobState(),
            new TwoJobState()
        };

            mentalStates = new List<AbstractPlayerState>();
        }

        public void OnPlayerTurnStart()
        {
            DistributeCashflow();
            UpdateContacts();
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

        private void DistributeCashflow()
        {
            Snapshot snapshot = new Snapshot(this);
            portfolio.AddCash(snapshot.cashflow);
        }

        public void AddJob(Profession job)
        {
            jobs.Add(job);
            oldJobs.Remove(job);
        }

        public void LoseJob(Profession job)
        {
            if (jobs.Remove(job) && !oldJobs.Contains(job))
            {
                oldJobs.Add(job);
            }
        }

        public void AddMentalState(AbstractPlayerState state)
        {
            mentalStates.Add(state);
        }

        public bool HasSkill(SkillType skillType)
        {
            foreach (SkillInfo skillInfo in skills)
            {
                if (skillInfo.skillType == skillType)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddSkill(SkillInfo skillInfo)
        {
            if (!HasSkill(skillInfo.skillType))
            {
                skills.Add(skillInfo);
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