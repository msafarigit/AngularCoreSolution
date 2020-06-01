using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularCore.Models.Chart
{
    public class ChartVM
    {
        public List<int> Data { get; set; }
        public string Label { get; set; }

        public ChartVM()
        {
            Data = new List<int>();
        }
    }
}
