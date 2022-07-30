using CleanStreetBL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CleanStreetWin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HubPageAr : Page
    {
        readonly ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        private readonly ObservableCollection<HubPageItem> tasks = new ObservableCollection<HubPageItem>();
        private readonly ObservableCollection<HubPageItem> myReports = new ObservableCollection<HubPageItem>();
        private readonly ObservableCollection<HubPageItem> latestPublicObservations = new ObservableCollection<HubPageItem>();

        public HubPageAr()
        {
            this.InitializeComponent();
            // Set the flow direction based on the current language direction
            if (loader.GetString("LayoutDirection") == "RTL")
            {
                FlowDirection = FlowDirection.RightToLeft;
            }
            InitializeData();

            //System.Diagnostics.Debug.WriteLine("CurrentCulture is {0}.", CultureInfo.CurrentCulture.Name);

        }


        private async void InitializeData()
        {

            tasks.Add(new HubPageItem
            {
                ObservationId = 1,
                Image = "Assets/submit-report.png",
                Title = loader.GetString("SubmitReport"),
                Subtitle = "Observation",
                Description = loader.GetString("lblYoumaysubmittheRiyadhcitystreetobservation"),
            });
            tasks.Add(new HubPageItem
            {
                ObservationId = 2,
                Image = "Assets/show-report.png",
                Title = loader.GetString("lblShowReport"),
                Subtitle = "Listing",
                Description = loader.GetString("lblShowmyreports")
            });
            tasks.Add(new HubPageItem
            {
                ObservationId = 3,
                Image = "Assets/about.png",
                Title = loader.GetString("lblAboutUs"),
                Subtitle = "About",
                Description = loader.GetString("KnowAboutUs")
            });
            tasks.Add(new HubPageItem
            {
                ObservationId = 4,
                Image = "Assets/help.png",
                Title = loader.GetString("lblHelp"),
                Subtitle = "Help",
                Description = loader.GetString("lblNeedhelptosubmitreport")
            });


            taskCollectionViewSource.Source = tasks;

            var fos = App.context.GetAll<FieldObservation>().OrderByDescending(d => d.ReportDateTime).Take(10);
            if (fos != null && fos.Count() > 0)
            {
                foreach (var fo in fos)
                {
                    var newHubPageItem = new HubPageItem()
                    {
                        ObservationId = fo.ObservationId,
                        Image = "Assets/placeholder.png",
                        Photo = await GetImageFromStorage(fo.FileName),
                        Title = fo.ObservationName,
                        Subtitle = "",
                        Description = ""
                    };
                    myReports.Add(newHubPageItem);
                }
            }

            myReportCollectionViewSource.Source = myReports;

            latestPublicObservations.Add(new HubPageItem
            {
                ObservationId = 1,
                Image = "Assets/submit-report.png",
                Title = loader.GetString("SubmitReport"),
                Subtitle = "Observation",
                Description = loader.GetString("lblYoumaysubmittheRiyadhcitystreetobservation"),

            });
            latestPublicObservations.Add(new HubPageItem
            {
                ObservationId = 2,
                Image = "Assets/show-report.png",
                Title = loader.GetString("lblShowReport"),
                Subtitle = "Listing",
                Description = loader.GetString("lblShowmyreports")
            });
            latestPublicObservations.Add(new HubPageItem
            {
                ObservationId = 3,
                Image = "Assets/about.png",
                Title = loader.GetString("lblAboutUs"),
                Subtitle = "About",
                Description = loader.GetString("KnowAboutUs")
            });
            latestPublicObservations.Add(new HubPageItem
            {
                ObservationId = 4,
                Image = "Assets/help.png",
                Title = loader.GetString("lblHelp"),
                Subtitle = "Help",
                Description = loader.GetString("lblNeedhelptosubmitreport")
            });


            observationsCollectionViewSource.Source = latestPublicObservations;


        }

        private static async Task<BitmapImage> GetImageFromStorage(string fileName)
        {
            var uri = new Uri("ms-appx:///Assets/placeholder.png", UriKind.Absolute);
            BitmapImage tempBi = new BitmapImage(uri);
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try
            {
                file = await localFolder.GetFileAsync(fileName);
            }
            catch (Exception)
            {
                return tempBi;
            }

            BitmapImage bmi = new BitmapImage();

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    fileStream.Seek(0);
                    bmi.SetSource(fileStream);
                }

                return bmi;

            }
            else
            {
                return tempBi;
            }
        }

        private void MyTasksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HubPageItem item = e.ClickedItem as HubPageItem;
            switch (item.ObservationId)
            {
                case 1:
                    Frame.Navigate(typeof(SubmitReport));
                    break;
                case 2:
                    Frame.Navigate(typeof(ShowAllReport));
                    break;
                case 3:
                    Frame.Navigate(typeof(AboutUs));
                    break;
                case 4:
                    Frame.Navigate(typeof(Help));
                    break;
                default:
                    break;
            }
            System.Diagnostics.Debug.WriteLine(item.Title);

        }

        private void MyReportsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HubPageItem item = e.ClickedItem as HubPageItem;
            System.Diagnostics.Debug.WriteLine(item.ObservationId);
            Frame.Navigate(typeof(ShowReport), item.ObservationId);

        }

        private void SectionHeader_Click(object sender, HubSectionHeaderClickEventArgs e)
        {

            switch (e.Section.Name)
            {
                case "MyReports":
                    Frame.Navigate(typeof(ShowAllReport));
                    break;
                default:
                    break;
            }


        }

        private void pickerDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            var d = sender as DatePicker;
            if (d.Date != null)
            {
                Frame.Navigate(typeof(SearchReportResult), d.Date.DateTime);
            }

        }


    }

}
