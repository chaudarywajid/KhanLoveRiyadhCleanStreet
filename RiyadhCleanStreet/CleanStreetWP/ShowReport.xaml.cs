using CleanStreetWP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CleanStreetWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowReport : Page
    {
        readonly ResourceLoader loader = new ResourceLoader();
        string msgNoImage = "No Image Found";
        string msgNoImageExist = "Image does not exist";
        string msgSetImage = "Image set from storage";
        string msgSelectMail = "Please Select Mail Link to Send Email";

        FieldObservation fieldObservation;
        StorageFolder localFolder;
        BitmapImage bmi = null;
        StorageFile file = null;

        //get object for data sharing
        readonly DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();



        public ShowReport()
        {
            this.InitializeComponent();
            // Set the flow direction based on the current language direction
            if (loader.GetString("LayoutDirection") == "RTL")
            {
                PageSubHeaderTextBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            }

            msgNoImage = loader.GetString("NoImageFound");
            msgNoImageExist = loader.GetString("NoImageExist");
            msgSetImage = loader.GetString("ImageSetFromStorage");
            msgSelectMail = loader.GetString("PleaseSelectMail");

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int foid = (int)e.Parameter;
            System.Diagnostics.Debug.WriteLine("id= " + foid);

            // fieldObservation = App.context.GetAll<FieldObservation>().Where(c => c.ObservationId == foid).FirstOrDefault();

            fieldObservation = App.dbConn.Table<FieldObservation>().Where(c => c.ObservationId == foid).FirstOrDefault();

            this.ContentRoot.DataContext = fieldObservation;
            SetMainImageFromStorage(fieldObservation.FileName);

            myMap.Center = new Geopoint(new BasicGeoposition() { Latitude = 24.666820, Longitude = 46.731466 });
            myMap.ZoomLevel = 16;
            myMap.LandmarksVisible = true;
            //myMap.MapType = MapType.Aerial;

            showLatLng();
            //register contract handler
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
               DataRequestedEventArgs>(this.ShareHtmlHandler);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager,
             DataRequestedEventArgs>(this.ShareHtmlHandler);
        }



        private async void showLatLng()
        {

            BasicGeoposition location = new BasicGeoposition() { Latitude = fieldObservation.Lat, Longitude = fieldObservation.Lng };


            MapIcon MapIcon1 = new MapIcon()
            {
                Location = new Geopoint(location),
                NormalizedAnchorPoint = new Point(1.0, 0.5),
                Title = "Location Needle"
            };
            myMap.MapElements.Add(MapIcon1);


            await myMap.TrySetViewAsync(new Geopoint(location), 15.0f);


        }

        private async void SetMainImageFromStorage(string fileName)
        {
            localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                file = await localFolder.GetFileAsync(fileName);
            }
            catch (Exception)
            {
                statusTextBlock.Text = msgNoImage;
            }

            bmi = new BitmapImage();

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    fileStream.Seek(0);

                    bmi.SetSource(fileStream);
                    MainImage.Source = bmi;
                    statusTextBlock.Text = msgSetImage;
                    //added delay to show photo otherwise blank will appear
                    await Task.Delay(10);
                }
            }
            else
            {
                statusTextBlock.Text = msgNoImageExist;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void ShareHtmlHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (fieldObservation != null)
            {
                DataRequest request = e.Request;
                request.Data.Properties.Title = fieldObservation.ObservationName;
                request.Data.Properties.Description = msgSelectMail;
                request.Data.SetText(String.Format("{0} on {1} {2} at ({3},{4})", fieldObservation.ObservationName, fieldObservation.StreetName, fieldObservation.DistrictName, fieldObservation.Lat, fieldObservation.Lng));

                //string htmlExample = "<p>Here is a local image: </p>";
                //string htmlFormat = HtmlFormatHelper.CreateHtmlFormat(htmlExample);
                //request.Data.SetHtmlFormat(htmlFormat);

                // Because the HTML contains a local image, we need to add it to the ResourceMap.

                if (file != null)
                {

                    // Because we are making async calls in the DataRequested event handler,
                    // we need to get the deferral first.
                    DataRequestDeferral deferral = request.GetDeferral();

                    // Make sure we always call Complete on the deferral.
                    try
                    {
                        StorageFile logoFile = file;

                        List<IStorageItem> storageItems = new List<IStorageItem>();
                        storageItems.Add(logoFile);
                        request.Data.SetStorageItems(storageItems);
                    }
                    finally
                    {
                        deferral.Complete();
                    }
                }

            }
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void HomeAppBarClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
