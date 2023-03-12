using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MicrobialG
{
    public class ExpanderItems
    {
        private bool _isChecked;
        public ExpanderItems(string name, string id)
        {
            Name = name;
            Id = id;
        }
        public bool IsChecked
        {
            get
            {
                return this._isChecked;
            }
            set
            {
                //This is called when checkbox state is changed
                this._isChecked = value;

                //Add or remove from collection on parent VM, perform sorting here
                if (this.IsChecked)
                {
                    if(!OnChart.GetInstance().Present_on_chart.Contains(Id + ";" + Name))
                    {
                        OnChart.GetInstance().Present_on_chart.Add(Id + ";" + Name);
                    }
                        
                }
                else
                {
                    if (OnChart.GetInstance().Present_on_chart.Contains(Id + ";" + Name))
                        OnChart.GetInstance().Present_on_chart.Remove(Id + ";" + Name);
                }
            }
        }
        public string Name { get; set; }
        public string Id { get; set; }
    }
    public class Expanders
    {
        public string Title { get; set; }
        public ObservableCollection<ExpanderItems> expenderitems { get; set; } = new ObservableCollection<ExpanderItems>();
        public Expanders(string title)
        {
            Title = title;
        }
        public void AddListItem(ExpanderItems listItem)
        {
            expenderitems.Add(listItem);
        }
    }

    public sealed class OnChart
    {
        private static OnChart instance = null;
        private ObservableCollection<string> present_on_chart = new ObservableCollection<string>();

        public ObservableCollection<string> Present_on_chart { get => present_on_chart; set => present_on_chart = value; }

        public static OnChart GetInstance()
        {
            if (instance == null)
            {
                instance = new OnChart();
            }
            return instance;
        }
    }

}
