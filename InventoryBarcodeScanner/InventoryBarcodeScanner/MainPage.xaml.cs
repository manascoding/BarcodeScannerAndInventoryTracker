using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Phone.Tasks;
using System.Windows.Threading;
using System.ComponentModel;
using ZXing;
using ZXing.Common;


namespace InventoryBarcodeScanner
{
    public partial class MainPage : PhoneApplicationPage
    {

        private readonly BackgroundWorker scannerWorker;
        private PhotoCamera camera;
        
        // Constructor

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.FirstListBox.ItemsSource = AppContext.appContext.itemList;
        }


        public MainPage()
        {
            InitializeComponent();
            // prepare the backround worker thread for the image processing
            scannerWorker = new BackgroundWorker();
            scannerWorker.DoWork += ScannerWorkerDoWork;
            scannerWorker.RunWorkerCompleted += ScannerWorkerRunWorkerCompleted;

            this.FirstListBox.ItemsSource = AppContext.appContext.itemList;
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            List<BarcodeItem> main = new List<BarcodeItem>(AppContext.appContext.itemList);
            this.FirstListBox.ItemsSource = main;
            if (this.camera != null)
            {
                if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true)
                {
                    this.camera = new PhotoCamera(CameraType.Primary);
                    this.camera.Initialized += OnPhotoCameraInitialized;
                    CameraButtons.ShutterKeyPressed += OnButtonFullPress;
                    // The event is fired when the viewfinder is tapped (for focus).
                    CodeScannerCanvas.Tap += new EventHandler<GestureEventArgs>(FocusTapped);
                    this.camera.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(CameraPictureReady);
                    viewfinderBrush.SetSource(this.camera);
                }
            }
        }

        
        // Dispose of camera when exiting. 
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (this.camera != null)
            {
                this.camera.Dispose();
            }
        }

        // This will rotate the camera tot he correct position and do the preparation procedures. 
        private void OnPhotoCameraInitialized(object sender, CameraOperationCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    this.camera.FlashMode = FlashMode.Off;
                    viewfinderTransform.Rotation = this.camera.Orientation;
                }
                catch (Exception i)
                {

                    i.ToString();
                }

            });
        }

        // Take the picture and try to find a barcode in it. 
        private void OnButtonFullPress(object sender, EventArgs e)
        {
            if (this.camera != null)
            {
                try
                {
                    focusBrackets.Foreground = new SolidColorBrush(Colors.Blue);
                    this.camera.CaptureImage();
                }
                catch (Exception exp)
                {
                    System.Diagnostics.Debug.WriteLine(exp.ToString());
                }
            }
        }

        // Provide touch focus in the viewfinder.
        private void FocusTapped(object sender, GestureEventArgs e)
        {
            if (this.camera != null)
            {
                textBox3.Visibility = Visibility.Collapsed;
                if (this.camera.IsFocusAtPointSupported)
                {
                    this.camera.FocusAtPoint(.5, .5);
                }
                else if (this.camera.IsFocusSupported)
                {
                    this.camera.Focus();
                }
            }
        }

        //Get the picture and try to decypher it using the database. 
        private void CameraPictureReady(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {

            Stream image = e.ImageStream;
            Dispatcher.BeginInvoke(() =>
            {
                // setting the image in the display and start scanning in the background
                var bmp = new BitmapImage();
                bmp.SetSource(image);
                var tempbmp = new WriteableBitmap(bmp);

                int startX = tempbmp.PixelWidth / 5;
                int startY = tempbmp.PixelHeight / 5;

                var final = tempbmp.Crop(startX, startY, startX * 3, startY * 3);
                scannerWorker.RunWorkerAsync(final);

            });

        }

        //This will process the result and then call the main thred to display the resutl and either move on or inform the user. 
        private void ScannerWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // processing the result of the background scanning
            if (e.Error == null)
            {
                var result = (Result)e.Result;
                Dispatcher.BeginInvoke(() => this.DisplayResult(result));
            }
        }


        // When the pivot has changed we need to act accordingly. 
        private void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = this.mainPivot.SelectedIndex;
            if (index == 0)
            {
                List<BarcodeItem> main = new List<BarcodeItem>(AppContext.appContext.itemList);
                this.FirstListBox.ItemsSource = main;
            }
            else
            {
                //We need to initialize the camera.
                if (this.camera == null)
                {
                    if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true)
                    {
                        this.camera = new PhotoCamera(CameraType.Primary);
                        this.camera.Initialized += OnPhotoCameraInitialized;
                        CameraButtons.ShutterKeyPressed += OnButtonFullPress;
                        // The event is fired when the viewfinder is tapped (for focus).
                        CodeScannerCanvas.Tap += new EventHandler<GestureEventArgs>(FocusTapped);
                        this.camera.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(CameraPictureReady);
                        viewfinderBrush.SetSource(this.camera);
                    }
                }
            }

        }

        //THis decodes the image and then sends it tot he completed worker to display the result. 
        private static void ScannerWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // scanning for a barcode            
            e.Result = new BarcodeReader().Decode((WriteableBitmap)e.Argument);

        }

        //Display the result and inform the user of the next step. 
        private void DisplayResult(Result result)
        {
            focusBrackets.Foreground = new SolidColorBrush(Colors.Gray);
            if (result != null)
            {
                //Take the user to the product page view
                AppContext.appContext.itemEdit = new BarcodeItem(result.Text, result.BarcodeFormat.ToString());
                NavigationService.Navigate(new Uri("/ProductInfoView.xaml", UriKind.Relative));

            }
            else
            {
                textBox3.Visibility = Visibility.Visible;
            }
        }

        private void ItemButtonClick(object sender, RoutedEventArgs e)
        {
            AppContext.appContext.itemEdit = (sender as Button).DataContext as BarcodeItem;
            NavigationService.Navigate(new Uri("/ProductInfoView.xaml", UriKind.Relative));
        }

    }
}