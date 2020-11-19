namespace PlayerState
{
    public class OneJobState : PlayerStateInterface
    {
        public string getDescription()
        {
            return "Happiness -10 when working at least one job.";
        }

        public int getHappiness(Player player)
        {
            return player.jobs.Count > 0 ? -10 : 0;
        }

        public string getName()
        {
            return "Working one job.";
        }
    }

    public class TwoJobState : PlayerStateInterface
    {
        public string getDescription()
        {
            return "Happiness -20 when working two job.";
        }

        public int getHappiness(Player player)
        {
            return player.jobs.Count > 1 ? -20 : 0;
        }

        public string getName()
        {
            return "Working two jobs.";
        }
    }
}