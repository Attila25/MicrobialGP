using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrobialG
{
    public class ChartProperties
    {
        private string name;
        private int cycle_number;

        public ChartProperties(string name, int cycle_number)
        {
            this.name = name;
            this.cycle_number = cycle_number;
        }

        public string Name { get => name; set => name = value; }
        public int Cycle_number { get => cycle_number; set => cycle_number = value; }

    }
}
