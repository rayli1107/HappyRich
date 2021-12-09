using Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class GameInitState : IState
    {
        private StateMachine _stateMachine;
        private RunOnceAction _initAction;

        public GameInitState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void EnterState(StateMachineParameter param)
        {
            _initAction = new RunOnceAction(runInit);
        }

        public void ExitState()
        {
        }

        private void runInit()
        {
            RealEstateManager.Instance.Initialize(GameManager.Instance.Random);
            StockManager.Instance.Initialize(GameManager.Instance.Random);
            BusinessManager.Instance.Initialize(GameManager.Instance.Random);
            SelfImprovementManager.Instance.Initialize();
            RiskyInvestmentManager.Instance.Initialize(GameManager.Instance.Random);
            FamilyManager.Instance.Initialize(GameManager.Instance.Random);

            List<Action<Action>> actions = new List<Action<Action>>()
            {
                TutorialManager.Instance.GetEnableTutorialAction(),
                TutorialManager.Instance.GetGameInitMessageAction()
            };
            CompositeActions.GetAndAction(actions)?.Invoke(
                () => _stateMachine.ChangeState(_stateMachine.CharacterSelectionState));
        }

        public void Update()
        {
            if (UI.UIManager.Instance != null &&
                UI.UIManager.Instance.ready &&
                BusinessManager.Instance != null &&
                FamilyManager.Instance != null &&
                GameManager.Instance != null &&
                InterestRateManager.Instance != null &
                InvestmentManager.Instance != null &&
                InvestmentPartnerManager.Instance != null &&
                JobManager.Instance != null &&
                Localization.Instance != null &&
                LuxuryManager.Instance != null &&
                MarketEventManager.Instance != null &&
                PersonalEventManager.Instance != null &&
                RealEstateManager.Instance != null &&
                RiskyInvestmentManager.Instance != null &&
                SelfImprovementManager.Instance != null &&
                SkillManager.Instance != null &&
                SpecialistManager.Instance != null &&
                StartupManager.Instance != null &&
                StockManager.Instance != null &&
                TutorialManager.Instance != null)
            {
                _initAction.Run();
            }
        }
    }
}
