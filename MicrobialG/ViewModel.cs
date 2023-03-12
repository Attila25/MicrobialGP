using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using LiveChartsCore.Kernel.Sketches;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;
using System.Diagnostics;
using Ookii.Dialogs.Wpf;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Windows.Media;
using LiveChartsCore.Drawing;
using System.Windows.Ink;

namespace MicrobialG
{
   
    public class ViewModel
    {
        public const string UserSettingsFilename = "settings.json";
        public string _UserSettingsPath = UserSettingsFilename;

        private ICommand newdatacmd;

        private ICommand personalize;

        private ICommand importdata;

        private ICommand colouredit;

        private ICommand savecolours;
        

        public bool isAverage;

        public bool newAxis;

        public bool custcolour;

        public string selected_colour;

        private Random rnd = new Random();

        public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();
        public ObservableCollection<ICartesianAxis> YAxes { get; } = new ObservableCollection<ICartesianAxis>();
        public ObservableCollection<ICartesianAxis> XAxes { get; } = new ObservableCollection<ICartesianAxis>();

        public ObservableCollection<RectangularSection> Sections { get; set; } = new ObservableCollection<RectangularSection>();

        internal List<TribesCollection> tribescollection = new List<TribesCollection>();
        public ObservableCollection<Expanders> expanders { get; } = new ObservableCollection<Expanders>();

        internal ObservableCollection<ChartProperties> chart_props = new ObservableCollection<ChartProperties>();

        static string datafolder = "";

        public ICommand NewDataCmd
        {
            get
            {
                if (newdatacmd == null)
                {
                    newdatacmd = new RelayCommand(
                        param => this.New_Click(),
                        param => this.CanSave()
                    );
                }
                return newdatacmd;
            }
        }
        public ICommand ImportDataCmd
        {
            get
            {
                if (importdata == null)
                {
                    importdata = new RelayCommand(
                        param => this.Import_Click(),
                        param => this.CanSave()
                    );
                }
                return importdata;
            }
        }
        public ICommand SaveColours
        {
            get
            {
                if (savecolours == null)
                {
                    savecolours = new RelayCommand(
                        param => this.Save_Colours(),
                        param => this.CanSave()
                    );
                }
                return savecolours;
            }
        }
        public ICommand Personalize
        {
            get
            {
                if (personalize == null)
                {
                    personalize = new RelayCommand(
                        param => this.Psettings(),
                        param => this.CanSave()
                    );
                }
                return personalize;
            }
        }
        public ICommand ColourEdit
        {
            get
            {
                if (colouredit == null)
                {
                    colouredit = new RelayCommand(
                        param => this.Edit_Colours(),
                        param => this.CanSave()
                    );
                }
                return colouredit;
            }
        }
        private bool CanSave()
        {
            return true;
        }
        public bool IsAverage
        {
            get { return this.isAverage; }
            set 
            { 
                this.isAverage = value;
                Update_Chart();
            }
        }
        public bool NewAxis
        {
            get { return this.newAxis; }
            set
            {
                this.newAxis = value;
                Update_Chart();
            }
        }
        public bool CustColour
        {
            get { return this.custcolour; }
            set
            {
                this.custcolour = value;
                Update_Chart();
            }
        }
        public string Selected_Colour
        {
            get 
            {
                
                if (this.selected_colour != null)
                    return this.selected_colour.ToString();

                return null;
            }
            set
            {
                if (value != null)
                    this.selected_colour = value.ToString(); 
            }
        }
        private void New_Click()
        {
            List<string> parameters = new List<string>();

            MessageBox.Show("Please select the Excel file containig the Data", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            Stream myStream = null;
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Filter = "Excel files|*.xlsx";
            theDialog.InitialDirectory = PersonalSettings.GetInstance().Def_inputfolder;
            bool? result = theDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    if ((myStream = theDialog.OpenFile()) != null)
                    {
                        parameters.Add(theDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            MessageBox.Show("Please select the Excel file containig the Layout", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            result = theDialog.ShowDialog();
            theDialog.InitialDirectory = PersonalSettings.GetInstance().Def_inputfolder;
            if (result.HasValue && result.Value)
            {
                try
                {
                    if ((myStream = theDialog.OpenFile()) != null)
                    {
                        parameters.Add(theDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. \nOriginal error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            string outputf = PersonalSettings.GetInstance().Def_outputfolder != "" ? PersonalSettings.GetInstance().Def_outputfolder : Directory.GetCurrentDirectory();
            string python = "-u MicrobialG_P\\ExcelReader.py";

            if (parameters.Count() < 2)
            {
                MessageBox.Show("Please select the files you want to use to proceed", "Watning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if(run_cmd(python, parameters[0], parameters[1], outputf))
                {
                    if(ReadDataFromJSON())
                        Fill_Expander();
                }
                else
                    MessageBox.Show("There was an error during the processing of the data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Import_Click()
        {

            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
            folderDialog.Description = "Please select the folder";
            folderDialog.UseDescriptionForTitle = true;

            if ((bool)folderDialog.ShowDialog())
            {
                datafolder = folderDialog.SelectedPath;
            }
            if (datafolder != "")
            {
                ReadDataFromJSON();
                Fill_Expander();
            }
        }
        private void Edit_Colours()
        {
            CustColours_Edit colouredit = new CustColours_Edit();
            colouredit.Show();
        }
        private void Save_Colours()
        {
            PersonalSettings.GetInstance().Save(_UserSettingsPath);
        }
        private void Psettings()
        {
            UserSettings us = new UserSettings();
            us.Show();
        }

        ObservableCollection<string> present_on_chart;
        
        public ViewModel()
        {
            this.present_on_chart = OnChart.GetInstance().Present_on_chart;
            this.present_on_chart.CollectionChanged += onchart_CollectionChanged;
        }
        void onchart_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                List<string> items = new List<string>();
                if (e.NewItems != null && custcolour && Selected_Colour != null)
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    items.AddRange(e.NewItems.OfType<string>());
                    string key = string.Join(", ", items);
                    key = key.Split(";")[0].Split("_")[0] + "_" + key.Split(";")[1];
                    if (!PersonalSettings.GetInstance().Custom_colours.ContainsKey(key))
                        PersonalSettings.GetInstance().Custom_colours.Add(key, Selected_Colour);
                    else
                        PersonalSettings.GetInstance().Custom_colours[key] = Selected_Colour;
                }
                Update_Chart();
            }
        }
        internal bool ReadDataFromJSON()
        {
            bool jsonreadstatus = false;
            List<Tribes>? temptribelist = new List<Tribes>();
            ChartProperties? cp;
            foreach (string file in Directory.EnumerateFiles(datafolder, "*.json"))
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                if (filename.Contains("ChartProps"))
                {
                    try
                    {
                        StreamReader r = new StreamReader(file);
                        string json = r.ReadToEnd();

                        cp = JsonConvert.DeserializeObject<ChartProperties>(json);
                        chart_props.Add(cp);

                        r.Close();
                        jsonreadstatus = true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    try
                    {
                        StreamReader r = new StreamReader(file);
                        string json = r.ReadToEnd();

                        temptribelist = JsonConvert.DeserializeObject<List<Tribes>>(json);
                        if (temptribelist != null)
                            tribescollection.Add(new TribesCollection(filename, temptribelist));

                        r.Close();
                    }
                    catch (Exception)
                    {
                        jsonreadstatus = false;
                        throw;
                    }
                }
            }
            return jsonreadstatus;
        }
        internal void Fill_Expander()
        {
            foreach (var tribe_items in tribescollection)
            {
                if(!expanders.Any(ex => ex.Title == tribe_items.Id))
                {
                    var listItem = new Expanders(tribe_items.Id);
                    foreach (var tribe_data in tribe_items.Tribes)
                    {
                        listItem.AddListItem(new ExpanderItems(tribe_data.Name, tribe_items.Id));
                    }
                    expanders.Add(listItem);
                }
            }
        }
        internal void Update_Chart()
        {
            
            Series.Clear();
            Sections.Clear();
            YAxes.Clear();

            int index = 0;
            List<string> label = new List<string>();

            if (present_on_chart.Count == 1 && !IsAverage)
            {
                XAxes.Clear();
                string id_of_tribe = present_on_chart[0].Split(";")[0];
                string name_of_tribe = present_on_chart[0].Split(";")[1];
                
                foreach (var props in chart_props)
                {
                    if(props.Name == id_of_tribe.Split('_')[1])
                    {   
                        for (float i = 0; i < props.Cycle_number/2; i++)
                        {
                            label.Add(i.ToString());
                            label.Add((i + 0.5).ToString());
                        }
                    }
                }
                XAxes.Add(new Axis
                {
                    Name = "Time (h)",
                    Labels = label.ToArray(),
                    MinStep = 2,
                    ShowSeparatorLines = true
                });

                foreach (var item in tribescollection)
                {
                    if (item.Id == id_of_tribe)
                    {
                        foreach (var tribes in item.Tribes)
                        {
                            if (tribes.Name == name_of_tribe)
                            {
                                YAxes.Add(new Axis
                                {
                                    Name = "Scale for " + tribes.Name,
                                    ShowSeparatorLines = true,
                                });
                                foreach (var data in tribes.Data)
                                {
                                    Series.Add(
                                    new LineSeries<double>
                                    {
                                        Values = data.Value,
                                        GeometrySize = 0,
                                        Fill = null
                                    });  
                                }
                            }
                        }

                    }
                }
            }
            else if(present_on_chart.Count != 0)
            {
                if (XAxes.Count == 0)
                {
                    XAxes.Add(new Axis
                    {
                        Name = "Time (h)",
                        Labels = label.ToArray(),
                        MinStep = 2,
                        ShowSeparatorLines = true
                    });
                }
                YAxes.Add(new Axis
                {
                    Name = "Arbitrary unit",
                    ShowSeparatorLines = true,
                });
                foreach (var onchart in present_on_chart)
                {
                    string id_of_tribe = onchart.Split(";")[0];
                    string name_of_tribe = onchart.Split(";")[1];
                    string key = onchart.Split(";")[0].Split("_")[0] + "_" + onchart.Split(";")[1];

                    foreach (var item in tribescollection)
                    {
                        if (item.Id == id_of_tribe)
                        {
                            foreach (var tribes in item.Tribes)
                            {
                                if (tribes.Name == name_of_tribe)
                                {
                                    string colour = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256)).ToString();
                                    if (CustColour && PersonalSettings.GetInstance().Custom_colours.ContainsKey(key))
                                    {
                                        colour = PersonalSettings.GetInstance().Custom_colours[key];
                                    }
                                    if (present_on_chart.Count() == 2 && NewAxis && index == 1)
                                    {
                                        YAxes.Add(new Axis
                                        {
                                            Name = "Arbitrary unit (" + tribes.Name + ")",
                                            ShowSeparatorLines = false,
                                            Position = LiveChartsCore.Measure.AxisPosition.End
                                        });
                                        Series.Add(new LineSeries<double>
                                        {
                                            Name = "Sample: " + tribes.Name,
                                            Values = tribes.Average,
                                            GeometrySize = 0,
                                            Fill = null,
                                            ScalesYAt = 1,
                                            Stroke = new SolidColorPaint(SKColor.Parse(colour)) { StrokeThickness = 5 },
                                            GeometryStroke = new SolidColorPaint(SKColor.Parse(colour)) { StrokeThickness = 5 }
                                        });
                                    }
                                    else
                                    {
                                        if(colour != null)
                                        {
                                            Series.Add(new LineSeries<double>
                                            {
                                                Name = "Sample: " + tribes.Name,
                                                Values = tribes.Average,
                                                GeometrySize = 0,
                                                Fill = null,
                                                Stroke = new SolidColorPaint(SKColor.Parse(colour)) { StrokeThickness = 5 },
                                                GeometryStroke = new SolidColorPaint(SKColor.Parse(colour)) { StrokeThickness = 5 }
                                            });
                                        }
                                        else
                                        {
                                            Series.Add(new LineSeries<double>
                                            {
                                                Name = "Sample: " + tribes.Name,
                                                Values = tribes.Average,
                                                GeometrySize = 0,
                                                Fill = null,
                                            });
                                        }
                                    }
                                    index++;
                                    //add_error_bar;
                                }
                            }

                        }
                    }
                }
            }
        }
        private void add_error_bar(Tribes tribes)
        {
            Sections.Add((new RectangularSection
            {
                // creates a section from 3 to 4 in the X axis
                Xi = 10,
                Xj = 10.1,
                Yi = tribes.Deviation_down[10],
                Yj = tribes.Deviation_up[10],
                Fill = new SolidColorPaint(new SKColor(0, 0, 0))
            }));
            Sections.Add((new RectangularSection
            {
                // creates a section from 3 to 4 in the X axis
                Xi = 20,
                Xj = 20.1,
                Yi = tribes.Deviation_down[20],
                Yj = tribes.Deviation_up[20],
                Fill = new SolidColorPaint(new SKColor(0, 0, 0))
            }));
            Sections.Add((new RectangularSection
            {
                // creates a section from 3 to 4 in the X axis
                Xi = 30,
                Xj = 30.1,
                Yi = tribes.Deviation_down[30],
                Yj = tribes.Deviation_up[30],
                Fill = new SolidColorPaint(new SKColor(0, 0, 0))
            }));
            Sections.Add((new RectangularSection
            {
                // creates a section from 3 to 4 in the X axis
                Xi = 40,
                Xj = 40.1,
                Yi = tribes.Deviation_down[40],
                Yj = tribes.Deviation_up[40],
                Fill = new SolidColorPaint(new SKColor(0, 0, 0))
            }));
            Sections.Add((new RectangularSection
            {
                // creates a section from 3 to 4 in the X axis
                Xi = 45,
                Xj = 45.1,
                Yi = tribes.Deviation_down[45],
                Yj = tribes.Deviation_up[45],
                Fill = new SolidColorPaint(new SKColor(0, 0, 0))
            }));                                    
        }
        private bool run_cmd(string cmd, string input_data, string input_layout, string output_folder)
        {
            bool cmd_result = false;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python.exe";
            start.Arguments = string.Format("{0} {1} {2} {3}", cmd, input_data, input_layout, output_folder);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.WindowStyle = ProcessWindowStyle.Normal;
            try
            {
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardError)
                    {
                        string result = reader.ReadToEnd();
                        if (result != null || result != "")
                            MessageBox.Show("There was an error during the processing of the data " + result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        if (result != null || result != "")
                        {
                            datafolder = result.Trim();
                            MessageBox.Show("Results has been generated to the following folder: " + datafolder, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                cmd_result = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
            
            return cmd_result;
        }
    }
}
