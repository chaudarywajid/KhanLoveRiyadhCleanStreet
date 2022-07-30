using CleanStreetBL.Models;
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
using Windows.UI.Popups;
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
    public sealed partial class SearchReportResult : Page
    {
        readonly ResourceLoader loader = new ResourceLoader();

        string msg = "No Record Found!";

        private readonly ObservableCollection<HubPageItem> myReports = new ObservableCollection<HubPageItem>();
        public SearchReportResult()
        {
            this.InitializeComponent();
            // Set the flow direction based on the current language direction
            if (loader.GetString("LayoutDirection") == "RTL")
            {
                FlowDirection = FlowDirection.RightToLeft;
                BackButton.Icon = new SymbolIcon(Symbol.Forward);
                PageSubHeaderTextBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            }
            msg = loader.GetString("NoRecordFound");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            getData((DateTime)e.Parameter);
        }

        private async void getData(DateTime dt)
        {
            var fos = App.context.GetAll<FieldObservation>().OrderByDescending(d => d.ReportDateTime).ToList();
            fos = fos.Where(d => d.ReportDateTime.Date == dt.Date).ToList();
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
            else
            {

                MessageDialog dialog = new MessageDialog(msg);
                await dialog.ShowAsync();
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                if (loader.GetString("LayoutDirection") == "RTL")
                {
                    Frame.Navigate(typeof(HubPageAr));
                }
                else
                {
                    Frame.Navigate(typeof(HubPage));
                }
            }

            myReportCollectionViewSource.Source = myReports;



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


        private void MyReportsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HubPageItem item = e.ClickedItem as HubPageItem;
            System.Diagnostics.Debug.WriteLine(item.ObservationId);
            Frame.Navigate(typeof(ShowReport), item.ObservationId);

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
