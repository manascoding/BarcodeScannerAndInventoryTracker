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

namespace InvetoryBarcodeScanner
{
    class Item
    {

          
        public static BarcodeItem itemEdit { get; set; }

        private static IsolatedStorageFile appData = IsolatedStorageFile.GetUserStoreForApplication();
        private const string INFO_KEY = "BarcodeItemsData.Info";
        
        // List of the Barcode Items
        private static List<BarcodeItem> _itemList;
        public static List<BarcodeItem> itemList
        {
            get
            {
                if (_itemList == null)
                {
                    _itemList = new List<BarcodeItem>();
                    if (appData.FileExists(INFO_KEY))
                    {
                        using (var stream = new IsolatedStorageFileStream(INFO_KEY, FileMode.OpenOrCreate, FileAccess.Read, appData))
                        {
                            if (stream.Length > 0)
                            {
                                DataContractSerializer ser = new DataContractSerializer(typeof(List<BarcodeItem>));
                                _itemList = ser.ReadObject(stream) as List<BarcodeItem>;
                            }
                        }

                    }
                    else
                    {
                        Item.StoreData(_itemList);

                    }
                }

                return _itemList;
            }
        
            
        }

        // Checks whether or not the barcode items are contained in the list
        public static bool ContainsBarcode(string barcode, string barcodeType)
        {
            List<BarcodeItem> tempItems = Item.itemList;
            foreach (BarcodeItem temp in tempItems)
            {
                if (temp.barcodeNumber.Equals(barcode) && temp.barcodeType.Equals(barcodeType))
                {
                    return true;
                }
            }

            return false;
        }

        //Store the data in the parmanent storage
        public static void StoreData(List<BarcodeItem> itemList)
        {
            try
            {
                _itemList = itemList;
                
                if (appData.FileExists(INFO_KEY))
                {
                    appData.DeleteFile(INFO_KEY);
                }

                using (var stream = new IsolatedStorageFileStream(INFO_KEY, FileMode.OpenOrCreate, FileAccess.Write, appData))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(List<BarcodeItem>));
                    ser.WriteObject(stream, _itemList);
                }

            }

            catch (IsolatedStorageException)
            {
                //Do nothing
            }
        }


        public static BarcodeItem UpdateCurrentItem(BarcodeItem barcodeItem)
        {
            List<BarcodeItem> tempList = Item.itemList;
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
        public static void AddOrUpdate(BarcodeItem barcodeItem)
        {
            if (Item.ContainsBarcode(barcodeItem.barcodeNumber, barcodeItem.barcodeType))
            {
                List<BarcodeItem> tempList = Item.itemList;
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
                itemList.Add(barcodeItem);
            }
            Item._itemList = itemList;
            Item.StoreData(Item.itemList);
        }

        // Delete the item
        public static void DeleteBarcodeItem(BarcodeItem barcodeItem)
        {
            
            foreach (BarcodeItem temp in Item.itemList)
            {
                if (temp.barcodeNumber.Equals(barcodeItem.barcodeNumber) && temp.barcodeType.Equals(barcodeItem.barcodeType))
                {
                    Item.itemList.Remove(temp);
                    break;
                }
            }
            Item._itemList = itemList;
            Item.StoreData(Item.itemList);
        }

    }
}
