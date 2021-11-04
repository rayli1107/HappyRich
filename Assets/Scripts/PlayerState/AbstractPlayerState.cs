using PlayerInfo;

namespace PlayerState
{
    public class AbstractPlayerState
    {
        protected Player player { get; private set; }
        public string name { get; private set; }
        public virtual string description => "";
        public virtual int happinessModifier => 0;
        public virtual int expenseModifier => 0;

        public AbstractPlayerState(Player player, string name)
        {
            this.player = player;
            this.name = name;
        }

        public virtual void OnPlayerTurnStart()
        {

        }
    }

    public class SelfReflectionState : AbstractPlayerState
    {
        public SelfReflectionState(Player player, string name) : base(player, name)
        {

        }
    }
}
