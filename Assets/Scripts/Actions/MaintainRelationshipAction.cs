using PlayerInfo;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using UI.Panels.Templates;

namespace Actions
{
    

    public class MaintainRelationshipAction : AbstractAction
    {
        private Player _player;

        public MaintainRelationshipAction(Player player) : base(null)
        {
            _player = player;
        }

        private void onMessageDone(ButtonType buttonType)
        {
            UI.UIManager.Instance.UpdatePlayerInfo(_player);
            GameManager.Instance.StateMachine.OnPlayerActionDone();
            RunCallback(true);
        }

        private void noOp()
        {
            UI.UIManager.Instance.ShowSimpleMessageBox(
                "You've reached out to all your professional contacts.",
                ButtonChoiceType.OK_ONLY,
                onMessageDone);
        }

        private void addSpecialist()
        {
            SpecialistInfo info  = SpecialistManager.Instance.GetNewSpecialist(
                _player, GameManager.Instance.Random);
            _player.AddSpecialist(info);

            Localization local = Localization.Instance;
            string message = string.Format(
                "One of your investors introduced you to a {0}, who {1}",
                local.GetSpecialist(info),
                info.specialistDescription);
            UI.UIManager.Instance.ShowSimpleMessageBox(
                message, ButtonChoiceType.OK_ONLY, onMessageDone);
        }

        public override void Start()
        {
            foreach (InvestmentPartner partner in _player.contacts)
            {
                partner.RefreshDuration();
            }

            List<Action> actions = new List<Action>();

            if (SpecialistManager.Instance.HasNewSpecialistsAvailable(_player))
            {
                actions.Add(addSpecialist);
            }
            if (actions.Count == 0)
            {
                actions.Add(noOp);
            }

            int index = GameManager.Instance.Random.Next(actions.Count);
            actions[index].Invoke();
        }
    }
}
