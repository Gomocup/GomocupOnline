using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GomocupOnline.Models
{
    public class TournamentModel
    {
        public string Title { get; set; }

        //public List<GomokuMatchModel> Matches { get; set; }

        public TournamentModel(string path)
        {
            Title = Path.GetFileName(path);

            string[] files = Directory.GetFiles(path, "*.psq");

            //files.Select(f => new GomokuMatchModel(f)).ToList();
        }
    }
}