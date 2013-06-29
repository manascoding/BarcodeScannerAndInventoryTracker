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

using ZXing;
using ZXing.Common;

namespace InvetoryBarcodeScanner
{
    public partial class ProductInfoView : PhoneApplicationPage
    {
        public ProductInfoView()
        {
            InitializeComponent();
        }

        //Set up the new card and wait for user input of go back. 
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NewCodeWrapper.infoResult != null)
            {
                this.barcodeNumber.Text = NewCodeWrapper.infoResult.Text;
                this.barcodeType.Text = NewCodeWrapper.infoResult.BarcodeFormat.ToString();

                if (this.barcodeType.Text.Contains("UPC"))
                {
                    this.GetProductInformation();
                }
                this.GenerateBarcode();
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        //Get the product information if we have a UPC code. 
        private void GetProductInformation()
        {
            UPCInformation run = new UPCInformation(NewCodeWrapper.infoResult.Text, this);
            run.generateRequest();
        }

        //Display the Barcode and display it. 
        private void GenerateBarcode()
        {


            IBarcodeWriter writer = new BarcodeWriter
            {
                Format = NewCodeWrapper.infoResult.BarcodeFormat,
                Options = new EncodingOptions
                {
                    Height = 280,
                    Width = 440,
                    PureBarcode = false

                }
            };

            var bmp = writer.Write(NewCodeWrapper.infoResult.Text);

            barcodeCanvas.Source = bmp;
        }
        //Search the we for more information abut the product
        private void SearchWeb(object ender, RoutedEventArgs e)
        {
            SearchTask searchTask = new SearchTask();
            searchTask.SearchQuery = NewCodeWrapper.infoResult.Text;
            searchTask.Show();
        }
        //Display the information gotten from the search. 
        public void DisplayInformation(ShoppingBud.UPCInformation.ItemInfo response)
        {
            //this.informationBlock.Text = response.productNames[0];
        }
    }
}