using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GomocupOnline.Models
{
    public class TournamentMatch
    {
        public GomokuMatchInfoModel[] Matches { get; set; }

        public string Tournament { get; set; }
    }

    public class GomokuMatchInfoModel
    {
        [DisplayName("LastChange")]
        public DateTime LastChange { get; set; }

        [DisplayName("Match")]
        public string FileName { get; set; }

        [DisplayName("Player1")]
        public string Player1 { get; set; }

        [DisplayName("Player2")]
        public string Player2{ get; set; }

        [DisplayName("Moves")]
        public int Moves { get; set; }

        /// <summary>
        /// 1 - vyhrava Player1,  -1 vyhrava Player2, 0 remiza
        /// </summary>
        [DisplayName("Result")]
        public int Result { get; set; }
    }
}