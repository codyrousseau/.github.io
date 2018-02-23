using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace crousseau2_College_Strike.ViewModels
{
    public class PositionVM
    {
        public int PositionID { get; set; }
        public string PositionName { get; set; }
        public bool Assigned { get; set; }
    }
}