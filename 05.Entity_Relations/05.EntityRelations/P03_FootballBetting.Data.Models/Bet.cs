using System;

namespace P03_FootballBetting.Data.Models
{
    public enum PredictionOptions
    {
        Win = 1,
        Draw = 0,
        Lost = -1
    }
    public class Bet
    {
        public int BetId { get; set; }

        public decimal Amount { get; set; }

        public PredictionOptions Prediction { get; set; }

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int GameId { get; set; }

        public Game Game { get; set; }
    }
}
