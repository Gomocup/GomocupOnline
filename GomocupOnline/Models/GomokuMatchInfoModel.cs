using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GomocupOnline.Models
{
    public class GomokuMatchInfoModel
    {
        [DisplayName("LastChange")]
        public DateTime LastChange { get; set; }

        [DisplayName("Match")]
        public string FileName { get; set; }
    }
}