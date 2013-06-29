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

using ZXing;
using ZXing.Common;

namespace InventoryBarcodeScanner
{
    public partial class ProductEditView : PhoneApplicationPage
    {
        public ProductEditView()
        {
            InitializeComponent();
        }


        //Set up the new card and wait for user input of go back. 
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (AppContext.appContext.itemEdit != null)
            {
                this.barcodeNumber.Text = AppContext.appContext.itemEdit.barcodeNumber;
                this.barcodeType.Text = AppContext.appContext.itemEdit.barcodeType;

                if (AppContext.appContext.itemEdit.itemTitle != null)
                {
                    this.nameInputBox.Text = AppContext.appContext.itemEdit.itemTitle;
                }
                if (AppContext.appContext.itemEdit.quantity == null)
                {
                    this.quantityEntryBox.Text = "1";
                }
                else
                {
                    this.quantityEntryBox.Text = AppContext.appContext.itemEdit.quantity.ToString();
                }               
                this.GenerateBarcode();
            }
            else
            {
                NavigationService.GoBack();
            }
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
        // Update the details of that barcode
        private void SubmitNewCode(object sender, RoutedEventArgs e)
        {
            if (this.nameInputBox.Text.Equals("") || this.quantityEntryBox.Text.Equals(""))
            {
                MessageBox.Show("Invalid Values Inputted");
                return;
            }

            try
            {
                int? quantity = Convert.ToInt32(this.quantityEntryBox.Text);
                BarcodeItem newBarcodeItem = AppContext.appContext.itemEdit;
                newBarcodeItem.quantity = quantity;
                newBarcodeItem.itemTitle = this.nameInputBox.Text;

                AppContext.appContext.AddOrUpdate(newBarcodeItem);
                AppContext.appContext.StoreData();

                NavigationService.RemoveBackEntry();
                NavigationService.GoBack();

            }
            catch (System.FormatException)
            {
                MessageBox.Show("Invalid Quantity Value Inputted");
                return;
            }

            
            
        }

    }
}