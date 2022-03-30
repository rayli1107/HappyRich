using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class PlayerInitState : IState
    {
        private StateMachine _stateMachine;

        public PlayerInitState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            //            GameManager.Instance.CreatePlayer();
            //            GameManager.Instance.player.AddSkill(
            //                SkillManager.Instance.GetSkillInfo(ScriptableObjects.SkillType.BUSINESS_OPERATIONS));
            //            GameManager.Instance.player.AddSkill(
            //                SkillManager.Instance.GetSkillInfo(ScriptableObjects.SkillType.REAL_ESTATE_VALUATION));
/*
                        GameManager.Instance.player.contacts.Add(
                            new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
                        GameManager.Instance.player.contacts.Add(
                            new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
                        GameManager.Instance.player.contacts.Add(
                            new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
                        GameManager.Instance.player.contacts.Add(
                            new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
                        GameManager.Instance.player.contacts.Add(
                            new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
                        GameManager.Instance.player.contacts.Add(
                            new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
                        GameManager.Instance.player.contacts.Add(
                            new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
                        GameManager.Instance.player.contacts.Add(
                            new InvestmentPartner("Alice", 200000, RiskTolerance.kHigh, 10));
                        GameManager.Instance.player.AddSpecialist(
                            SpecialistManager.Instance.GetSpecialistInfo(
                                ScriptableObjects.SpecialistType.REAL_ESTATE_BROKER));
                        GameManager.Instance.player.AddSpecialist(
                            SpecialistManager.Instance.GetSpecialistInfo(
                                ScriptableObjects.SpecialistType.LOAN_AGENT));


            GameManager.Instance.player.portfolio.AddCash(5000000);
            */
            if (GameManager.Instance.cheatMode)
            {
                GameManager.Instance.player.AddSpecialist(
                    SpecialistManager.Instance.GetSpecialistInfo(ScriptableObjects.SpecialistType.VENTURE_CAPITALIST));
                GameManager.Instance.player.portfolio.AddCash(5000000);
                GameManager.Instance.player.contacts.Add(
                    new InvestmentPartner("Alice", 2000000, RiskTolerance.kHigh, 10));
//                GameManager.Instance.player.contacts.Add(
                    //new InvestmentPartner("Bob", 2000000, RiskTolerance.kLow, 10));

            }
            UI.UIManager.Instance.UpdatePlayerInfo(GameManager.Instance.player);
        }

        public void ExitState()
        {
        }

        public void Update()
        {
            StateMachineParameter param = new StateMachineParameter();
            param.newYearEnterMarketEventState = false;
            _stateMachine.ChangeState(_stateMachine.YearStartState, param);
        }
    }
}
