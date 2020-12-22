namespace PlayerState
{
    public class AbstractPlayerState
    {
        public Player player { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public virtual int happinessModifier => 0;
        public virtual int expenseModifier => 0;

        public AbstractPlayerState(Player player, string name, string description)
        {
            this.player = player;
            this.name = name;
            this.description = description;
        }
    }
}
