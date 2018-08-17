using System;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;


namespace CommonSupport
{
    /// <summary>
    /// Static class automates the operation with Bit.ly API.
    /// API based on http://sites.google.com/site/bitlyapp/
    /// </summary>
    public static class BitLyAPI
    {
        /// <summary>
        /// Single central method.
        /// </summary>
        /// <param name="username">BitLy username.</param>
        /// <param name="apiKey">The BitLy API key (see website to obtain).</param>
        /// <param name="input">Input URI.</param>
        /// <param name="type">Type of operation.</param>
        /// <returns>OUtput URI.</returns>
        public static String Bit(string username, string apiKey, string input, string type)
        {
            String Btype;
            String Xtype;
            String Itype;

            switch (type)
            {
                case "Shorten":
                    Btype = "shorten";
                    Xtype = "shortUrl";
                    Itype = "longUrl";
                    break;
                case "MetaH":
                    Btype = "info";
                    Xtype = "htmlMetaDescription";
                    Itype = "hash";
                    break;
                case "MetaU":
                    Btype = "info";
                    Xtype = "htmlMetaDescription";
                    Itype = "shortUrl";
                    break;
                case "ExpandH":
                    Btype = "expand";
                    Xtype = "longUrl";
                    Itype = "hash";
                    break;
                case "ExpandU":
                    Btype = "expand";
                    Xtype = "longUrl";
                    Itype = "shortUrl";
                    break;
                case "ClicksU":
                    Btype = "stats";
                    Xtype = "clicks";
                    Itype = "shortUrl";
                    break;
                case "ClicksH":
                    Btype = "stats";
                    Xtype = "clicks";
                    Itype = "hash";
                    break;
                case "UserU":
                    Btype = "info";
                    Xtype = "shortenedByUser";
                    Itype = "shortUrl";
                    break;
                case "UserH":
                    Btype = "info";
                    Xtype = "shortenedByUser";
                    Itype = "hash";
                    break;
                default:
                    return string.Empty;
            }


            StringBuilder url = new StringBuilder();  //Build a new string
            url.Append("http://api.bit.ly/");   //Add base URL
            url.Append(Btype);
            url.Append("?version=2.0.1");             //Add Version
            url.Append("&format=xml");
            url.Append("&");
            url.Append(Itype);
            url.Append("=");
            url.Append(input);                         //Append longUrl from input
            url.Append("&login=");                    //Add login "Key"
            url.Append(username);                     //Append login from input
            url.Append("&apiKey=");                   //Add ApiKey "Key"
            url.Append(apiKey);                       //Append ApiKey from input

            // Prepare web request.
            try
            {
                WebRequest request = WebRequest.Create(url.ToString());
                StreamReader responseStream = new StreamReader(request.GetResponse().GetResponseStream()); //prepare responese holder
                String response = responseStream.ReadToEnd(); //fill up response
                responseStream.Close(); //Close stream

                string data = response.ToString(); //Turn it into a string
                string newdata = XmlParse_general(data, Xtype); //parse the XML
                if (newdata == "Error")
                {
                    return string.Empty;
                }
                else
                {
                    return newdata;
                }
            }
            catch(Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Helper.
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string XmlParse_general(string Url, string type)    //XML parse Function
        {
            System.Xml.XmlTextReader xmlrt1 = new XmlTextReader(new StringReader(Url));
            while (xmlrt1.Read())
            {
                string strNodeType = xmlrt1.NodeType.ToString();
                string strName = xmlrt1.Name;

                // Get the clicks.
                if (strNodeType == "Element" && strName == type) 
                {
                    xmlrt1.Read();
                    return xmlrt1.Value;
                }
            }

            return string.Empty;
        }
    }
}

