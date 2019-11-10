namespace P03_FootballBetting.Data.Models
{
    public class Player
    {
        public int PlayerId { get; set; }

        public string Name { get; set; }

        public int SquadNumber { get; set; }

        public int TeamId { get; set; }
        //nav

        public int PositionId { get; set; }
        //nav

        public bool IsInjured { get; set; }
    }
}
