using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using ZXing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;


namespace InventoryBarcodeScanner
{
    public class AppContext
    {

        public BarcodeItem itemEdit { get; set; }
        private IsolatedStorageFile appData;
        private const string INFO_KEY = "BarcodeItemsDataGitHubVersion.Info";
        private AppContext()
        {
            this.appData = IsolatedStorageFile.GetUserStoreForApplication();
        }
        private static AppContext _appContext;
        //singleton pattern since we really only want to have one context for the entire application. 
        public static AppContext appContext
        {
            get
            {
                if (AppContext._appContext == null)
                {
                    AppContext._appContext = new AppContext();
                }
                return AppContext._appContext;
            }
        }


        // List of the Barcode Items
        private List<BarcodeItem> _itemList;
        public  List<BarcodeItem> itemList
        {
            get
            {
                if (this._itemList == null)
                {
                    this._itemList = new List<BarcodeItem>();
                    if (this.appData.FileExists(INFO_KEY))
                    {
                        using (var stream = new IsolatedStorageFileStream(INFO_KEY, FileMode.OpenOrCreate, FileAccess.Read, this.appData))
                        {
                            if (stream.Length > 0)
                            {
                                DataContractSerializer ser = new DataContractSerializer(typeof(List<BarcodeItem>));
                                this._itemList = ser.ReadObject(stream) as List<BarcodeItem>;
                            }
                        }

                    }
                    else
                    {
                        this.StoreData();

                    }
                }

                return _itemList;
            }


        }

        //Store the data in the parmanent storage
        public void StoreData()
        {
            try
            {
                this._itemList = this.itemList;

                if (appData.FileExists(INFO_KEY))
                {
                    appData.DeleteFile(INFO_KEY);
                }

                using (var stream = new IsolatedStorageFileStream(INFO_KEY, FileMode.OpenOrCreate, FileAccess.Write, this.appData))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(List<BarcodeItem>));
                    ser.WriteObject(stream, this._itemList);
                }

            }

            catch (IsolatedStorageException)
            {
                //Do nothing for now
            }
        }

        // Checks whether or not the barcode items are contained in the list
        public bool ContainsBarcode(string barcode, string barcodeType)
        {
            List<BarcodeItem> tempItems = this.itemList;
            foreach (BarcodeItem temp in tempItems)
            {
                if (temp.barcodeNumber.Equals(barcode) && temp.barcodeType.Equals(barcodeType))
                {
                    return true;
                }
            }

            return false;
        }


        public BarcodeItem UpdateCurrentItem(BarcodeItem barcodeItem)
        {
            List<BarcodeItem> tempList = this.itemList;
            foreach (BarcodeItem temp in tempList)
            {
                if (temp.barcodeNumber.Equals(barcodeItem.barcodeNumber) && temp.barcodeType.Equals(barcodeItem.barcodeType))
                {
                    return temp;
                }
            }
            // if not found we return what we were given
            return barcodeItem;

        }

        // Add or update and item and then save it
        public void AddOrUpdate(BarcodeItem barcodeItem)
        {
            if (this.ContainsBarcode(barcodeItem.barcodeNumber, barcodeItem.barcodeType))
            {
                List<BarcodeItem> tempList = this.itemList;
                foreach (BarcodeItem temp in tempList)
                {
                    if (temp.barcodeNumber.Equals(barcodeItem.barcodeNumber) && temp.barcodeType.Equals(barcodeItem.barcodeType))
                    {
                        temp.itemTitle = barcodeItem.itemTitle;
                        temp.quantity = barcodeItem.quantity;
                    }
                }
            }
            else
            {
                this.itemList.Add(barcodeItem);
            }
            
            this.StoreData();
        }

        // Delete the item
        public void DeleteBarcodeItem(BarcodeItem barcodeItem)
        {

            foreach (BarcodeItem temp in this.itemList)
            {
                if (temp.barcodeNumber.Equals(barcodeItem.barcodeNumber) && temp.barcodeType.Equals(barcodeItem.barcodeType))
                {
                    this.itemList.Remove(temp);
                    break;
                }
            }
            
            this.StoreData();
        }




    }
}
