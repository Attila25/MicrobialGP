using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrobialG
{
    internal class TribesCollection
    {
        private string id;
        private List<Tribes> tribes;

        public TribesCollection(string id, List<Tribes> tribes)
        {
            this.id = id;
            this.tribes = tribes;
        }

        public string Id { get => id; set => id = value; }
        internal List<Tribes> Tribes { get => tribes; set => tribes = value; }
    }
    internal class Tribes
    {

        private string name;
        private string time;
        private Dictionary<string, List<double>> data;
        private List<double> average;
        private List<double> deviation_up;
        private List<double> deviation_down;

        public Tribes(string name, string time, Dictionary<string, List<double>> data, List<double> average, List<double> deviation_up, List<double> deviation_down)
        {
            this.Name = name;
            this.Time = time;
            this.Data = data;
            this.Average = average;
            this.Deviation_up = deviation_up;
            this.Deviation_down = deviation_down;
        }

        public string Name { get => name; set => name = value; }
        public string Time { get => time; set => time = value; }
        public Dictionary<string, List<double>> Data { get => data; set => data = value; }
        public List<double> Average { get => average; set => average = value; }
        public List<double> Deviation_up { get => deviation_up; set => deviation_up = value; }
        public List<double> Deviation_down { get => deviation_down; set => deviation_down = value; }
    }
}
