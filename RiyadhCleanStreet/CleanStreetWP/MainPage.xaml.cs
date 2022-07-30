using CleanStreetWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CleanStreetWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        private readonly ObservableCollection<HubPageItem> tasks = new ObservableCollection<HubPageItem>();
        private readonly ObservableCollection<HubPageItem> myReports = new ObservableCollection<HubPageItem>();
        private readonly ObservableCollection<HubPageItem> latestPublicObservations = new ObservableCollection<HubPageItem>();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            InitializeData();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            GetFieldObservations();


        }

        private async void GetFieldObservations()
        {
            //var fos =  App.context.GetAll<FieldObservation>().OrderByDescending(d => d.ReportDateTime).Take(10);

            var fos = App.dbConn.Table<FieldObservation>().OrderByDescending(d => d.ReportDateTime).Take(10);   //sqlite-net query
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
        }

        private void InitializeData()
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
                    Frame.Navigate(typeof(SubmitPicture));
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

    public class HubPageItem
    {

        public int ObservationId { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public BitmapImage Photo { get; set; }
    }
}
