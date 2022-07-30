using Bing.Maps;
using CleanStreetBL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
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
    public sealed partial class SubmitReport : Page
    {
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
        CameraCaptureUI dialog;
        readonly FieldObservation fieldObservation;
        readonly string fileName;
        StorageFile sfile = null;
        StorageFile dfile = null;

        //get object for data sharing
        readonly DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();


        public SubmitReport()
        {
            this.InitializeComponent();
            // Set the flow direction based on the current language direction
            if (loader.GetString("LayoutDirection") == "RTL")
            {
                FlowDirection = FlowDirection.RightToLeft;
                BackButton.Icon = new SymbolIcon(Symbol.Forward);
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
            mainGrid.DataContext = fieldObservation;

            myMap.Center = new Location(24.666820, 46.731466);
            myMap.ZoomLevel = 16;
            //myMap.MapType = MapType.Aerial;

            startTrack();

            //register contract handler
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
               DataRequestedEventArgs>(this.ShareHtmlHandler);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager,
             DataRequestedEventArgs>(this.ShareHtmlHandler);
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
        private async void BtnCameraClick(object sender, RoutedEventArgs e)
        {
            try
            {
                statusTextBlock.Text = "";

                dialog = new CameraCaptureUI();
                Size aspectRatio = new Size(16, 9);
                dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;
                dialog.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

                sfile = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                BitmapImage bi = new BitmapImage();
                if (sfile != null)
                {
                    using (IRandomAccessStream fileStream = await sfile.OpenAsync(FileAccessMode.Read))
                    {
                        bi.SetSource(fileStream);
                        App.CapturedImage = bi;
                        MainImage.Source = bi;
                    }

                    statusTextBlock.Text = msgImageCapture;
                }
                else
                {
                    statusTextBlock.Text = msgNoCapture;
                }
            }
            catch (Exception ex)
            {
                statusTextBlock.Text = ex.Message.ToString();
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
                App.context.Insert<FieldObservation>(fieldObservation);
                App.context.SaveChanges();
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
            App.context.Update<FieldObservation>(fieldObservation);
            App.context.SaveChanges();
            statusTextBlock.Text = obsUpdate;

        }


        private static Pushpin GetPushpin()
        {
            Pushpin pushpin = new Pushpin() { Text = "1" };
            return pushpin;
        }

        private void startTrack()
        {
            geolocator = new Geolocator();

            myMap.Children.Add(GetPushpin());
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

        private void displayPosition(object sender, PositionChangedEventArgs args)
        {
            fieldObservation.Lat = args.Position.Coordinate.Point.Position.Latitude;
            fieldObservation.Lng = args.Position.Coordinate.Point.Position.Longitude;
            Location location = new Location(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);

            Pushpin pushpin = GetPushpin();
            MapLayer.SetPosition(pushpin, location);
            myMap.Children.Add(pushpin);


            myMap.SetView(location, 15.0f);
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
    }
}
