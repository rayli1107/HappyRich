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
            _initAction = new RunOnceAction(_ => runInit());
        }

        public void ExitState()
        {
        }

        private void setupTutorial(Action cb)
        {
            if (TutorialManager.Instance.tutorialEnabled)
            {
                PersonalEventManager.Instance.EnableTutorialActions();
                MarketEventManager.Instance.EnableTutorialActions();
            }
            cb?.Invoke();
        }

        private void runInit()
        {
            GameSaveLoadManager.Instance.Initialize();
            GameManager.Instance.Initialize();
            RealEstateManager.Instance.Initialize(GameManager.Instance.Random);
            StockManager.Instance.Initialize(GameManager.Instance.Random);
            BusinessManager.Instance.Initialize(GameManager.Instance.Random);
            MentalStateManager.Instance.Initialize();
            RiskyInvestmentManager.Instance.Initialize(GameManager.Instance.Random);
            FamilyManager.Instance.Initialize(GameManager.Instance.Random);

            List<Action<Action>> actions = new List<Action<Action>>()
            {
                TutorialManager.Instance.GetEnableTutorialAction(),
                TutorialManager.Instance.StartTutorialScript,
                setupTutorial,
                TutorialManager.Instance.GameInitOnce.Run
            };
            CompositeActions.GetAndAction(actions)?.Invoke(
                () => _stateMachine.ChangeState(_stateMachine.CharacterSelectionState));
        }

        public void Update()
        {
            if (UI.UIManager.Instance != null &&
                UI.UIManager.Instance.ready &&
                GameSaveLoadManager.Instance != null &&
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
                MentalStateManager.Instance != null &&
                SkillManager.Instance != null &&
                SpecialistManager.Instance != null &&
                StartupManager.Instance != null &&
                StockManager.Instance != null &&
                EventLogManager.Instance != null &&
                TutorialManager.Instance != null)
            {
                _initAction.Run(null);
            }
        }
    }
}
