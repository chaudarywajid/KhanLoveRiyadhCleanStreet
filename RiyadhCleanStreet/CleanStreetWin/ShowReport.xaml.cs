using Bing.Maps;
using CleanStreetBL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
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
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CleanStreetWin
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
                FlowDirection = FlowDirection.RightToLeft;
                BackButton.Icon = new SymbolIcon(Symbol.Forward);
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

            fieldObservation = App.context.GetAll<FieldObservation>().Where(c => c.ObservationId == foid).FirstOrDefault();
            this.mainGrid.DataContext = fieldObservation;
            SetMainImageFromStorage(fieldObservation.FileName);

            myMap.Center = new Location(24.666820, 46.731466);
            myMap.ZoomLevel = 16;
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

        private static Pushpin GetPushpin()
        {
            Pushpin pushpin = new Pushpin() { Text = "1" };
            return pushpin;
        }

        private void showLatLng()
        {

            Location location = new Location(fieldObservation.Lat, fieldObservation.Lng);
            Pushpin pushpin = GetPushpin();
            MapLayer.SetPosition(pushpin, location);
            myMap.Children.Add(pushpin);

            myMap.SetView(location, 15.0f);


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
                    var streamRef = RandomAccessStreamReference.CreateFromFile(file);
                    request.Data.Properties.Thumbnail = streamRef;
                    request.Data.SetBitmap(streamRef);
                }

            }
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}
