using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryBarcodeScanner
{
    class SBConstants
    {
        //These constant strings deal with the UPC loook up site "searchUPC".

        //This is the Devleoper Token for UPC search
        // You need to provide your own
        public static readonly string SEARCH_UPC_ID = "";

        // This is the Base URL of the UPC Look up website
        public static readonly string UPC_URL_BASE = "http://www.searchupc.com/handlers/upcsearch.ashx?request_type=1&access_token=";

        //This is the URL UPC argument
        public static readonly string UPC_URL_URL_ARGUMENT = "&upc=";
    }
}
