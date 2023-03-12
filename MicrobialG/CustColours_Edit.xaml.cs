using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace MicrobialG
{
    /// <summary>
    /// Interaction logic for CustColours_Edit.xaml
    /// </summary>
    public partial class CustColours_Edit : Window
    {
        public const string UserSettingsFilename = "settings.json";
        public string _UserSettingsPath = UserSettingsFilename;

        public static List<string> names = new List<string>();
        public static List<ColorPicker> colorPickers = new List<ColorPicker>();
        public CustColours_Edit()
        {
            InitializeComponent();
            Fill_List();
        }

        public void Fill_List()
        {
            names.Clear();
            colorPickers.Clear();
            if (PersonalSettings.GetInstance().Custom_colours != null)
            {
                foreach (var item in PersonalSettings.GetInstance().Custom_colours)
                {
                    names.Add(item.Key);
                    colorPickers.Add(new ColorPicker
                    {
                        SelectedColor = (Color)ColorConverter.ConvertFromString(item.Value),
                        AllowDrop = true
                    });
                }
                Names.DataContext = names;
                CustColours.DataContext = colorPickers;
            }
            else
                System.Windows.MessageBox.Show("There are no custom colours yet", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < PersonalSettings.GetInstance().Custom_colours.Count(); i++)
            {
                PersonalSettings.GetInstance().Custom_colours[names[i]] = colorPickers[i].SelectedColor.ToString();
            }
            PersonalSettings.GetInstance().Save(_UserSettingsPath);

        }
    }
}
