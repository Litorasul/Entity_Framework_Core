
namespace P03_FootballBetting.Data.Models
{
    public class Team
    {
        public int TeamId { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string Initials { get; set; }

        public decimal Budget { get; set; }

        public int PrimaryKitColorId { get; set; }
        //nav

        public int SecondaryKitColorId { get; set; }
        //nav

        public int TownId { get; set; }
        //nav
    }
}
