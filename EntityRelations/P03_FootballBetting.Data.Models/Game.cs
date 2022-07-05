using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using P03_FootballBetting.Data.Models.Enum;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Game
    {
        public Game()
        {
            this.Bets = new HashSet<Bet>();
            this.PlayerStatistics = new HashSet<PlayerStatistic>();
        }

        [Key]
        public int GameId { get; set; }

        [ForeignKey(nameof(Team))]
        public int HomeTeamId { get; set; }
        public virtual Team HomeTeam { get; set; }

        [ForeignKey(nameof(Team))]
        public int AwayTeamId { get; set; }
        public virtual Team AwayTeam { get; set; }

        public byte HomeTeamGoals { get; set; }

        public byte AwayTeamGoals { get; set; }

        public DateTime? DateTime { get; set; }

        public decimal HomeTeamBetRate { get; set; }

        public decimal AwayTeamBetRate { get; set; }

        public decimal DrawBetRate { get; set; }

        public Prediction Result { get; set; }

        public virtual ICollection<Bet> Bets { get; set; }

        public virtual ICollection<PlayerStatistic> PlayerStatistics { get; set; }

    }
}
