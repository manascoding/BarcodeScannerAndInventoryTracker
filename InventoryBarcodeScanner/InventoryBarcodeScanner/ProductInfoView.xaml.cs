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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;

using ZXing;
using ZXing.Common;

namespace InventoryBarcodeScanner
{
    public partial class ProductInfoView : PhoneApplicationPage
    {

        private string barcodeText;
        private List<String> urlList;
        private Boolean loaded;
        private BarcodeItem currentItem;

        public ProductInfoView()
        {
            InitializeComponent();
        }

        
        //Set up the new card and wait for user input of go back. 
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (loaded) return;
            this.loaded = true;
            if (AppContext.appContext.itemEdit != null)
            {
                this.barcodeNumber.Text = AppContext.appContext.itemEdit.barcodeNumber;
                this.barcodeType.Text = AppContext.appContext.itemEdit.barcodeType;
                this.barcodeText = this.barcodeNumber.Text;

                this.currentItem = new BarcodeItem(this.barcodeText, this.barcodeType.Text);

                this.GenerateBarcode();
                this.setUpSearchTaskUPC();

                if (this.barcodeType.Text.Contains("UPC") || this.barcodeType.Text.Contains("EAN"))
                {
                    this.GetProductInformation();
                    this.CheckBarcodeItem();
                }
                else
                {
                    this.setUpFailCode();
                }

                //update the currentItem against our memory
                this.currentItem = AppContext.appContext.UpdateCurrentItem(this.currentItem);
                

            }
            else
            {
                NavigationService.GoBack();
            }
        }

        //Get the product information if we have a UPC code. 
        private void GetProductInformation()
        {
            UPCInformation run = new UPCInformation(AppContext.appContext.itemEdit.barcodeNumber, this);
            run.generateRequest();
        }

        //Display the Barcode and display it. 
        private void GenerateBarcode()
        {


            IBarcodeWriter writer = new BarcodeWriter
            {
                Format = AppContext.appContext.itemEdit.barcodeTypeHelper(),
                Options = new EncodingOptions
                {
                    Height = 280,
                    Width = 440,
                    PureBarcode = false

                }
            };

            var bmp = writer.Write(AppContext.appContext.itemEdit.barcodeNumber);

            barcodeCanvas.Source = bmp;
        }
        //Search the we for more information abut the product
        private void SearchWeb(object ender, RoutedEventArgs e)
        {
            SearchTask searchTask = new SearchTask();
            searchTask.SearchQuery = AppContext.appContext.itemEdit.barcodeNumber;
            searchTask.Show();
        }
        //Called when we can't get the UPC information
        private void setUpFailCode()
        {
            TextBlock bloc = new TextBlock();
            bloc.Text = "No Information Available.";
            bloc.Foreground = new SolidColorBrush(Colors.Red);
            bloc.TextAlignment = TextAlignment.Center;
            bloc.FontSize = 20;
            bloc.Width = 450;
            bloc.Height = 31;
            this.stackPanel1.Children.Add(bloc);

        }

        // Checks for duplicates
        private void CheckBarcodeItem()
        {
            if (AppContext.appContext.ContainsBarcode(this.currentItem.barcodeNumber, this.currentItem.barcodeType))
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = true;
            }
            else
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
            }
        }

        //Display the searchtask
        private void setUpSearchTaskUPC()
        {
            Button button = new Button();
            button.Foreground = new SolidColorBrush(Colors.White);
            button.Content = "Search the Web for the Barcode";
            button.Click += this.SearchWebURL;
            this.stackPanel1.Children.Add(button);
            
        }
        
        //Display the information gotten from the search. 
        public void DisplayInformation(UPCInformation.ItemInfo response)
        {
            try
            {
                var titles = response.productNames;

                if (titles.Count > 0)
                {
                    this.urlList = response.productLinks;

                    Image image = new Image();
                    Uri img = null;


                    foreach (string urlString in response.imageURLs)
                    {
                        if (urlString.Contains("http://"))
                        {
                            img = new Uri(urlString, UriKind.Absolute);
                            break;
                        }
                    }

                    image.Source = new BitmapImage(img);
                    image.MaxWidth = 450;
                    image.MaxHeight = 300;
                    this.stackPanel1.Children.Add(image);
                    var x = 0;

                    foreach (string title in titles)
                    {
                        if (x == 0 && this.currentItem.itemTitle == null )
                        {
                            this.currentItem.itemTitle = title;
                        }
                        StackPanel panel = new StackPanel();
                        panel.Background = new SolidColorBrush(Colors.White);
                        panel.Margin = new Thickness(0.0, 3.0, 0.0, 0.0);


                        TextBlock bloc = new TextBlock();
                        bloc.TextWrapping = TextWrapping.Wrap;
                        bloc.Text = title;
                        bloc.Foreground = new SolidColorBrush(Colors.Blue);
                        bloc.TextAlignment = TextAlignment.Center;
                        bloc.FontSize = 24;
                        bloc.Width = 450;
                        bloc.MinHeight = 40;

                        TextBlock priceBloc = new TextBlock();
                        priceBloc.TextWrapping = TextWrapping.Wrap;
                        priceBloc.Text = response.productCurrency[x].ToUpper() + " " + response.productPrices[x];
                        priceBloc.FontSize = 20;
                        priceBloc.TextAlignment = TextAlignment.Right;
                        priceBloc.Foreground = new SolidColorBrush(Colors.Black);
                        priceBloc.MinHeight = 30;
                        priceBloc.Width = 450;

                        TextBlock webBloc = new TextBlock();
                        webBloc.TextWrapping = TextWrapping.NoWrap;
                        webBloc.Text = "Visit: " + response.productStore[x];
                        webBloc.FontSize = 20;
                        webBloc.TextAlignment = TextAlignment.Right;
                        webBloc.Foreground = new SolidColorBrush(Colors.Purple);
                        webBloc.MinHeight = 30;
                        webBloc.Width = 450;
                        webBloc.Tap += this.OnWebItemTap;


                        panel.Children.Add(bloc);
                        panel.Children.Add(priceBloc);
                        panel.Children.Add(webBloc);
                        this.stackPanel1.Children.Add(panel);
                        x++;//Increment the counter

                    }
                }
                else
                {
                    this.setUpFailCode();

                }
            }
            catch(Exception){
                // Do nothing for now
            }
        }


        // on tap of the item we go to the corresponding link
        private void OnWebItemTap(object sender, RoutedEventArgs e)
        {
            
            if (sender.GetType() == typeof(TextBlock))
            {
                var count = 0;
                foreach (object type in this.stackPanel1.Children.ToList())
                {
                    if (type.GetType() == typeof(StackPanel))
                    {
                        StackPanel panel = (type as StackPanel);

                        if (panel.Children.ToList().Count == 3 && sender == panel.Children.ToList()[2])
                        {
                            WebBrowserTask task = new WebBrowserTask();
                            task.Uri = new Uri(this.urlList[count], UriKind.Absolute);
                            task.Show();
                            break;                            
                        }
                        else if (panel.Children.ToList().Count == 3)
                        {
                            count++;
                        }
                    }
                    
                }

                
            }
                       
        }

        //Search the web for the URL
        private void SearchWebURL(object sender, RoutedEventArgs e)
        {
            SearchTask task = new SearchTask();
            task.SearchQuery = this.barcodeText;
            task.Show();
        }
                
        
        //Add the item to the list
        private void AddAppButton_Click(object sender, EventArgs e)
        {
            AppContext.appContext.itemEdit = this.currentItem;
            NavigationService.Navigate(new Uri("/ProductEditView.xaml", UriKind.Relative));           
            
        }

        //Delete the item from the list
        private void DeleteAppButton_Click(object sender, EventArgs e)
        {
            AppContext.appContext.DeleteBarcodeItem(this.currentItem);
            NavigationService.GoBack();
        }

        //Edit the item information that is stored 
        private void EditAppButton_Click(object sender, EventArgs e)
        {
            AppContext.appContext.itemEdit = this.currentItem;
            NavigationService.Navigate(new Uri("/ProductEditView.xaml", UriKind.Relative));            
        }
    }
}