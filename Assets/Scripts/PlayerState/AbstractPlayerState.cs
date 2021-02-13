using PlayerInfo;

namespace PlayerState
{
    public class AbstractPlayerState
    {
        protected Player player => GameManager.Instance.player;
        public string name { get; private set; }
        public virtual string description => "";
        public virtual int happinessModifier => 0;
        public virtual int expenseModifier => 0;

        public AbstractPlayerState(string name)
        {
            this.name = name;
        }
    }
}
