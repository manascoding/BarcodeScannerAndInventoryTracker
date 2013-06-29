using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.Generic;

namespace InventoryBarcodeScanner    
{
    public class UPCInformation
    {
        private string UPC;
        private HttpWebRequest webRequest;
        private PhoneApplicationPage callbackPage;


        //Create the web look class. 
        public UPCInformation(string UPC, PhoneApplicationPage callback)
        {
            this.UPC = UPC;
            this.callbackPage = callback;
        }

        // Generate the UPC code search
        public void generateRequest()
        {
            //Generate the request string.
            String requestText = SBConstants.UPC_URL_BASE + SBConstants.SEARCH_UPC_ID + SBConstants.UPC_URL_URL_ARGUMENT + this.UPC;
            this.webRequest = (HttpWebRequest)WebRequest.Create(requestText);
            this.webRequest.BeginGetResponse(new AsyncCallback(GetResponseCallback), this.webRequest);
        }

        // Get the data from the server. 
        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {

            HttpWebRequest requests = (HttpWebRequest)asynchronousResult.AsyncState;
            string responseString = null;
            // End the operation
            try
            {
                HttpWebResponse response = (HttpWebResponse)requests.EndGetResponse(asynchronousResult);

                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                responseString = streamRead.ReadToEnd();

                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse
                response.Close();

                this.ParseCSVResponse(responseString);

            }
            catch (WebException exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.ToString());
            }

        }

        //Parses the CVS input from the server
        private void ParseCSVResponse(string response)
        {
            string[] parts = response.Split('\n');
            ItemInfo info = new ItemInfo();
            for (var x = 0; x < parts.Length; x++)
            {
                if (x == 0)
                {
                    continue;
                }

                string[] splitResponse = parts[x].Split(',');
                if (splitResponse.Length == 7)
                {
                    info.productNames.Add(this.removeParenthesis(splitResponse[0]));
                    info.imageURLs.Add(this.removeParenthesis(splitResponse[1]));
                    info.productLinks.Add(this.removeParenthesis(splitResponse[2]));
                    info.productPrices.Add(this.removeParenthesis(splitResponse[3]));
                    info.productCurrency.Add(this.removeParenthesis(splitResponse[4]));
                    info.productSalePrice.Add(this.removeParenthesis(splitResponse[5]));
                    info.productStore.Add(this.removeParenthesis(splitResponse[6]));
                }


            }
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                (this.callbackPage as ProductInfoView).DisplayInformation(info);
            });
        }


        //Remove parenthesis from the string.
        private string removeParenthesis(string text)
        {
            if (text == null || text.Length <= 2)
            {
                return "";
            }
            text = text.Trim();
            if (text.ToCharArray()[0] == '"' && text.ToCharArray()[text.Length - 1] == '"')
            {
                text = text.Substring(1, text.Length - 2);
            }
            return text;
        }
        //Class that contains all the information that will need to be returned to the sender. 
        public class ItemInfo
        {
            public List<string> productNames { get; set; }
            public List<string> imageURLs { get; set; }
            public List<string> productLinks { get; set; }
            public List<string> productPrices { get; set; }
            public List<string> productCurrency { get; set; }
            public List<string> productSalePrice { get; set; }
            public List<string> productStore { get; set; }

            public ItemInfo()
            {
                this.productNames = new List<string>();
                this.imageURLs = new List<string>();
                this.productLinks = new List<string>();
                this.productPrices = new List<string>();
                this.productCurrency = new List<string>();
                this.productSalePrice = new List<string>();
                this.productStore = new List<string>();
            }


        }
    }
}
