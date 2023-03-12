using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace MicrobialG
{
    public sealed class PersonalSettings
    {
        private static PersonalSettings instance = null;

        private string def_inputfolder;
        private string def_outputfolder;
        private bool isPythonavailbale;
        private bool pythonpackageinstalled;
        private Dictionary<string, string> custom_colours;

        public static PersonalSettings GetInstance()
        {
            if (instance == null)
            {
                instance = new PersonalSettings();
            }
            return instance;
        }

        public string Def_inputfolder { get => def_inputfolder; set => def_inputfolder = value; }
        public string Def_outputfolder { get => def_outputfolder; set => def_outputfolder = value; }
        public bool IsPythonavailbale { get => isPythonavailbale; set => isPythonavailbale = value; }
        public bool Pythonpackageinstalled { get => pythonpackageinstalled; set => pythonpackageinstalled = value; }
        public Dictionary<string, string> Custom_colours { get => custom_colours; set => custom_colours = value; }

        public void Save(string filename)
        {
            
            using (StreamWriter sw = new StreamWriter(filename))
            {
                var json = JsonConvert.SerializeObject(instance);
                sw.WriteLine(json);
                sw.Flush();
                sw.Close();
            }

            MessageBox.Show("Settings saved successfully", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public PersonalSettings Read(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                string json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<PersonalSettings>(json);
            }
        }

    }
}
