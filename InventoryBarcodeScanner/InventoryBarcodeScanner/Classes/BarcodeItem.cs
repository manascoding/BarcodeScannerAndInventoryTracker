using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZXing;
using ZXing.Common;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace InventoryBarcodeScanner
{
    //Barcode Item needs ot be serializible 
    [DataContract]
    public class BarcodeItem
    {
        [DataMember]
        public string itemTitle { get; set; }
        [DataMember]
        public string barcodeNumber { get; set; }
        [DataMember]
        public string barcodeType { get; set; }
        [DataMember]
        public int? quantity { get; set; }

        public BarcodeItem (string barcodeNumber, string barcodeType)
        {
            this.barcodeNumber = barcodeNumber;
            this.barcodeType = barcodeType;
        }

        public BarcodeItem(string itemTitle, string barcodeNumber, string barcodeType, int? quantity)
        {
            this.itemTitle = itemTitle;
            this.barcodeNumber = barcodeNumber;
            this.barcodeType = barcodeType;
            this.quantity = quantity;
        }


        //Converts the string barcode format to a BarcodeFormat type.
        public BarcodeFormat barcodeTypeHelper()
        {
            if(this.barcodeType.Equals(BarcodeFormat.AZTEC.ToString())){
                return BarcodeFormat.AZTEC;
            }

            if (this.barcodeType.Equals(BarcodeFormat.CODABAR.ToString()))
            {
                return BarcodeFormat.CODABAR;
            }

            if (this.barcodeType.Equals(BarcodeFormat.CODE_128.ToString()))
            {
                return BarcodeFormat.CODE_128;
            }

            if (this.barcodeType.Equals(BarcodeFormat.CODE_39.ToString()))
            {
                return BarcodeFormat.CODE_39;
            }

            if (this.barcodeType.Equals(BarcodeFormat.CODE_93.ToString()))
            {
                return BarcodeFormat.CODE_93;
            }

            if (this.barcodeType.Equals(BarcodeFormat.DATA_MATRIX.ToString()))
            {
                return BarcodeFormat.DATA_MATRIX;
            }

            if (this.barcodeType.Equals(BarcodeFormat.EAN_13.ToString()))
            {
                return BarcodeFormat.EAN_13;
            }

            if (this.barcodeType.Equals(BarcodeFormat.EAN_8.ToString()))
            {
                return BarcodeFormat.EAN_8;
            }

            if (this.barcodeType.Equals(BarcodeFormat.ITF.ToString()))
            {
                return BarcodeFormat.ITF;
            }

            if (this.barcodeType.Equals(BarcodeFormat.MAXICODE.ToString()))
            {
                return BarcodeFormat.MAXICODE;
            }

            if (this.barcodeType.Equals(BarcodeFormat.MSI.ToString()))
            {
                return BarcodeFormat.MSI;
            }

            if (this.barcodeType.Equals(BarcodeFormat.PDF_417.ToString()))
            {
                return BarcodeFormat.PDF_417;
            }

            if (this.barcodeType.Equals(BarcodeFormat.PLESSEY.ToString()))
            {
                return BarcodeFormat.PLESSEY;
            }

            if (this.barcodeType.Equals(BarcodeFormat.QR_CODE.ToString()))
            {
                return BarcodeFormat.QR_CODE;
            }

            if (this.barcodeType.Equals(BarcodeFormat.RSS_14.ToString()))
            {
                return BarcodeFormat.RSS_14;
            }

            if (this.barcodeType.Equals(BarcodeFormat.RSS_EXPANDED.ToString()))
            {
                return BarcodeFormat.RSS_EXPANDED;
            }

            if (this.barcodeType.Equals(BarcodeFormat.UPC_A.ToString()))
            {
                return BarcodeFormat.UPC_A;
            }

            if (this.barcodeType.Equals(BarcodeFormat.UPC_E.ToString()))
            {
                return BarcodeFormat.UPC_E;
            }

            if (this.barcodeType.Equals(BarcodeFormat.UPC_EAN_EXTENSION.ToString()))
            {
                return BarcodeFormat.UPC_EAN_EXTENSION;
            }

            throw new NullReferenceException("You do not have the correct Barcode type.");
        }

    }
}
