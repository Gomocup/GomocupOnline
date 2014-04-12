using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GomocupOnline.Models
{
    public class GomokuCompareModel
    {
        public GomokuMatchModel Reference { get; set; }
        public GomokuMatchModel[] Matches { get; set; }
        public string Player { get; set; }
    }
}