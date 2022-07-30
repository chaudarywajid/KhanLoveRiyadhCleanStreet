using CleanStreetWP.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
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
    public sealed partial class SubmitReportAdvCamera : Page
    {

        MediaCapture cameraCapture;

        readonly ResourceLoader loader = new ResourceLoader();
        string msgSelectMail = "Please Select Mail Link to Send Email";
        string msgImageSaved = "CapturedImage Saved Picture Folder!";
        string msgNoPhotoSaved = "No photo saved.";
        string msgImageCapture = "Image Captured successfully!";
        string msgNoCapture = "No photo captured.";
        string plsEnterStreet = "Please Enter Street Name";
        string plsEnterObservation = "Please Choose Observation from list";
        string obsSaved = "Observation Saved";
        string msgPhotoFirst = "Please Take Photo First";
        string obsUpdate = "Observation Updated";


        Geolocator geolocator;
        readonly FieldObservation fieldObservation;
        readonly string fileName;
        StorageFile sfile = null;
        StorageFile dfile = null;

        //get object for data sharing
        readonly DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
        public SubmitReportAdvCamera()
        {
            this.InitializeComponent();

            // Set the flow direction based on the current language direction
            if (loader.GetString("LayoutDirection") == "RTL")
            {

                PageSubHeaderTextBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            }

            msgSelectMail = loader.GetString("PleaseSelectMail");
            msgImageSaved = loader.GetString("msgImageSaved");
            msgImageCapture = loader.GetString("msgImageCapture");
            msgNoPhotoSaved = loader.GetString("msgNoPhotoSaved");
            msgNoCapture = loader.GetString("msgNoCapture");
            plsEnterStreet = loader.GetString("plsEnterStreet");
            plsEnterObservation = loader.GetString("plsEnterObservation");
            obsSaved = loader.GetString("obsSaved");
            msgPhotoFirst = loader.GetString("msgPhotoFirst");
            obsUpdate = loader.GetString("obsUpdate");

            List<string> obsList = new List<string> { "General Remark", "Container Full", "Litter on Street", "Litter Bins-Damaged", "Litter Overflowing", "Building Material", "Scattered waste in portion of street",
            "Refuse/Recycling Collection", "Bins on Street", "Blocked Street Drains", "Flood", "Street Cleansing-Sweeping", "Street Furniture-Damaged", "Street Lighting-Maintenance",
            "Abandoned Trolley", "Abandoned Vehicle", "Commercial Waste"
             };

            List<string> obsListAr = new List<string> { "ملاحظة عامة", "لم يتم تفريغ الحاوية", "يجب توفير حاوية إضافية", "يجب توفير برميل", "يجب إستبدال الحاوية المشوهة", "البرميل ممتلىء بالنفايات ويفيض بالقمامة الى الخارج", "مخلفات الهدم والبناء", "لم يتم كنس الشارع بالكامل",
            "النفايات الخضاراء", "الحاوية غير موجودة فى المكان المحدد لها", "إنسداد مجاري الصرف الصحي", "الفيضانات", "لم يتم كنس الشارع ", "الشارع الأثاث التالف", "صيانة الإضاءة", "تجميع المياه",
            "لم يتم رفع الحيوانات النافقة", "السيارات التالفة", "النفايات التجارية", "يجب استبدال البرميل", "لم يتم رفع النفايات", "مخلفات البناء ", "البرميل يعيق حركة المرور"
             };

            List<string> obsListUr = new List<string> { "جنرل تبصرہ", "ردی کی ٹوکری فراہم نہیں کی", "گندگی سڑک پر", "سڑک پر درخت", "بارش کے پانی کے سڑکوں پر", "سڑک پر عمارتی مواد", "گلی کے حصہ میں بکھرا ہوا کوڑا ",
            "ری سائیکلنگ کا سامان", "سڑک پر کوڑے دان کی ضرورت ہے", "سیوریج کا پانی", "سیلاب", "گلی سویپنگ / صفائی نہیں ہوئی ", "روڈ بلاک ہے", "سٹریٹ لائٹس کی بحالی",
            "متروک جانور", "سڑکوں کی مرمت کی ضرورت ہے", "بجلی کے کھمبے کی بحالی ضرورت"
             };

            if (CultureInfo.CurrentCulture.Name == "ar-SA")
            {
                this.comboBoxObs.DataContext = obsListAr;
            }
            else if (CultureInfo.CurrentCulture.Name == "ur-PK")
            {
                this.comboBoxObs.DataContext = obsListUr;
            }
            else
            {
                this.comboBoxObs.DataContext = obsList;
            }
            fileName = string.Format(@"{0}.jpg", Guid.NewGuid());
            fieldObservation = new FieldObservation()
            {
                FileName = fileName,
                ReportDateTime = System.DateTime.Now
            };
            ContentRoot.DataContext = fieldObservation;


            //register contract handler
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
               DataRequestedEventArgs>(this.ShareHtmlHandler);

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            myMap.Center = new Geopoint(new BasicGeoposition() { Latitude = 24.666820, Longitude = 46.731466 });
            myMap.ZoomLevel = 16;
            myMap.LandmarksVisible = true;
            //myMap.MapType = MapType.Aerial;

            startTrack();
            StartCamera();

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager,
             DataRequestedEventArgs>(this.ShareHtmlHandler);


            // Release resources
            if (cameraCapture != null)
            {
                StopCapturePreview();
                capturePreview.Source = null;
                cameraCapture.Dispose();
                cameraCapture = null;
            }
        }


        private async void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtStreet.Text))
            {
                MessageDialog dialog = new MessageDialog(plsEnterStreet);
                await dialog.ShowAsync();

            }

            else if (comboBoxObs.SelectedIndex == -1)//Nothing selected
            {
                MessageDialog dialog = new MessageDialog(plsEnterObservation);
                await dialog.ShowAsync();

            }
            else if (sfile != null)
            {
                SavePhoto();
                //App.context.Insert<FieldObservation>(fieldObservation);
                //App.context.SaveChanges();
                App.dbConn.Insert(fieldObservation);
                statusTextBlock.Text = obsSaved;
                BtnSave.Visibility = Visibility.Collapsed;
                BtnUpdate.Visibility = Visibility.Visible;
                BtnSubmit.IsEnabled = true;
            }
            else
            {
                MessageDialog dialog = new MessageDialog(msgPhotoFirst);
                await dialog.ShowAsync();

            }

        }

        private void BtnUpdateClick(object sender, RoutedEventArgs e)
        {
            //App.context.Update<FieldObservation>(fieldObservation);
            //App.context.SaveChanges();
            App.dbConn.Update(fieldObservation);
            statusTextBlock.Text = obsUpdate;

        }


        private void startTrack()
        {
            geolocator = new Geolocator();
            geolocator.MovementThreshold = 3;
            // myMap.Children.Add(GetPushpin());
            geolocator.PositionChanged += geolocator_PositionChanged;

        }

        private async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            // Need to set map view on UI thread.
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                displayPosition(this, args);
            });
        }

        private async void displayPosition(object sender, PositionChangedEventArgs args)
        {
            fieldObservation.Lat = args.Position.Coordinate.Point.Position.Latitude;
            fieldObservation.Lng = args.Position.Coordinate.Point.Position.Longitude;
            BasicGeoposition location = new BasicGeoposition() { Latitude = args.Position.Coordinate.Point.Position.Latitude, Longitude = args.Position.Coordinate.Point.Position.Longitude };


            MapIcon MapIcon1 = new MapIcon()
            {
                Location = new Geopoint(location),
                NormalizedAnchorPoint = new Point(1.0, 0.5),
                Title = "Location Needle"
            };
            myMap.MapElements.Add(MapIcon1);


            await myMap.TrySetViewAsync(new Geopoint(location), 15.0f);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void ComboBoxObs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            statusTextBlock.Text = comboBoxObs.SelectedValue.ToString();
            fieldObservation.ObservationName = comboBoxObs.SelectedValue.ToString();
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

                if (dfile != null)
                {
                    var streamRef = RandomAccessStreamReference.CreateFromFile(dfile);
                    request.Data.Properties.Thumbnail = streamRef;
                    request.Data.SetBitmap(streamRef);
                }

            }
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }


        private async void SavePhoto()
        {
            if (sfile != null)
            {
                try
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    dfile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    await sfile.RenameAsync(fileName);
                    await sfile.CopyAndReplaceAsync(dfile);

                    //uncomment if you want to store in local picture folder
                    //StorageFolder picFolder = KnownFolders.PicturesLibrary;
                    //try
                    //{
                    //    await sfile.MoveAsync(picFolder);
                    //}
                    //catch (Exception)
                    //{
                    //    statusTextBlock.Text = "CapturedImage Saved Local Storage!";
                    //}
                }
                catch (Exception)
                {
                    statusTextBlock.Text = msgImageSaved;
                }
            }
            else
            {
                statusTextBlock.Text = msgNoPhotoSaved;
            }
        }

        private void BtnCameraClick(object sender, RoutedEventArgs e)
        {
            CapturePhoto();
        }


        /// <summary>
        /// media capture logic
        /// </summary>
        /// 
        private void StartCamera()
        {

            InitCamera();
            StartCapturePreview();

        }
        async private void InitCamera()
        {
            cameraCapture = new MediaCapture();

            //simple approach

            await cameraCapture.InitializeAsync();
            //cameraCapture.VideoDeviceController.PrimaryUse = CaptureUse.Photo;

            //camera type selection approach

            // var cameraID = await GetCameraID(Windows.Devices.Enumeration.Panel.Front);
            //await captureManager.InitializeAsync(new MediaCaptureInitializationSettings
            //{
            //    StreamingCaptureMode = StreamingCaptureMode.Video,
            //    PhotoCaptureSource = PhotoCaptureSource.Photo,
            //    AudioDeviceId = string.Empty,
            //    VideoDeviceId = cameraID.Id
            //});


        }

        async private void StartCapturePreview()
        {
            capturePreview.Source = cameraCapture;
            await cameraCapture.StartPreviewAsync();
        }

        async private void StopCapturePreview()
        {
            await cameraCapture.StopPreviewAsync();
        }

        async private void CapturePhoto()
        {
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();
            imgFormat.Width = 640;
            imgFormat.Height = 480;

            // create storage file in local app storage 
            sfile = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "TestPhoto.jpg",
                CreationCollisionOption.ReplaceExisting);

            // take photo 
            await cameraCapture.CapturePhotoToStorageFileAsync(imgFormat, sfile);


            statusTextBlock.Text = "";


            BitmapImage bi = new BitmapImage();
            if (sfile != null)
            {
                using (IRandomAccessStream fileStream = await sfile.OpenAsync(FileAccessMode.Read))
                {
                    bi.SetSource(fileStream);
                    App.CapturedImage = bi;
                    MainImage.Source = bi;
                }
            }


        }

        private async Task<DeviceInformation> GetCameraID(Windows.Devices.Enumeration.Panel desired)
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            DeviceInformation deviceID = devices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desired);


            //DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
            //    .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desired);

            if (deviceID != null) return deviceID;
            else throw new Exception(string.Format("Camera of type {0} doesn't exist.", desired));
        }

        private void HomeAppBarClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
