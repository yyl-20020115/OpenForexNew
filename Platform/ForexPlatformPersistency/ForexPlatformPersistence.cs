//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data.SQLite;
//using CommonSupport;
//using System.Xml.Serialization;
//using System.IO;
//using System.Data;
//using System.Reflection;
//using System.Runtime.Serialization.Formatters.Binary;

//namespace ForexPlatformPersistence
//{
//    /// <summary>
//    /// Tester helper class.
//    /// </summary>
//    public static class ForexPlatformPersistence
//    {

//        #region Public

//        static public void FullUpdate<ItemType>(ItemType item)
//            where ItemType : class, IDBPersistent, new()
//        {
//        }

//        #endregion

//        static void Main()
//        {

//            //List<RssNewsItem> items = new List<RssNewsItem>();

//            //for (int i = 0; i < 100000; i++)
//            //{
//            //    items.Add(new RssNewsItem() { 
//            //        //Id = i + 1,
//            //        Comments = "Comment1." + i, 
//            //        DateTime = DateTime.Now, 
//            //        Title = "ItemXX" + i, 
//            //        Guid = Guid.NewGuid().ToString(), 
//            //        Author = "I am authora", 
//            //        Description = "Fill this with some long text explaining the power of a solution that has many uses.", 
//            //        Link = new Uri("http://www.myblob.com/takethisfolder/takethispage.html") });
//            //}


//            //Dictionary<string, object> fixedValues = new Dictionary<string,object>();
//            //// Make this values be skipped.
//            //fixedValues.Add("Id", null);
//            //fixedValues.Add("NewsSourceId", "2");

//            //ForexNewsItem item = new ForexNewsItem();
//            //item.Currency = "USD";
//            //item.DateTime = DateTime.Now;
//            //item.TimeSpan = TimeSpan.FromDays(1);
//            //item.Impact = ForexNewsItem.ImpactEnum.High;

//            //ADOPersistenceHelper.Insert(item, fixedValues, null, false);
//            //List<ForexNewsItem> fItems = ADOPersistenceHelper.Select<ForexNewsItem>(null, null, null);

//            //Dictionary<string, string> columnMapping = new Dictionary<string,string>();
//            //columnMapping.Add("Title", "Title");
//            //columnMapping.Add("Description", "Description");
//            //columnMapping.Add("PubDate", "PubDate");

//            //Dictionary<string, object> matchValues = new Dictionary<string,object>();
//            //matchValues.Add("Id", 2);
//            //List<NewsItem> items2 = ADOPersistencyHelper.Select<NewsItem>(_tablesNames[typeof(NewsItem)], null, matchValues, null);

//            //ADOPersistencyHelper.Insert(_tablesNames[typeof(NewsItem)], items, fixedValues, null/*columnMapping*/, false);

//            //ADOPersistencyHelper.Update(_tablesNames[typeof(NewsItem)], items, fixedValues, null/*columnMapping*/, false);

//            //ADOPersistencyHelper.ClearTable(_tablesNames[typeof(NewsItem)]);

//            //ADOPersistencyHelper.Delete(_tablesNames[typeof(NewsItem)], items);

//            //Dictionary<string, object> matchValues = new Dictionary<string,object>();
//            //matchValues.Add("Id", 1);
//            //int value = (int)ADOPersistencyHelper.Count(_tablesNames[typeof(NewsItem)], matchValues);

//            //List<NewsItem> item = DeSerializeFromDB < NewsItem > (new long[] { 1 });

//            return;

//            //SQLiteDataReader reader = mycommand.ExecuteReader();
//            //while (reader.Read())
//            //{
//            //}

//            //using (SQLiteConnection myconnection = new SQLiteConnection(@"dataDelivery source=D:\Projects\OpenForexPlatform\SourceCode\External\ofxp.s3db"))
//            //{
//            //    myconnection.Open();

//            //    using (SQLiteTransaction mytransaction = myconnection.BeginTransaction())
//            //    {
//            //        using (SQLiteCommand mycommand = new SQLiteCommand(myconnection))
//            //        {
//            //            SQLiteParameter myparam = new SQLiteParameter();
//            //            int n;

//            //            mycommand.CommandText = "INSERT INTO [MyTable] ([MyId]) VALUES(?)";
//            //            mycommand.Parameters.Add(myparam);

//            //            for (n = 0; n < 100000; n++)
//            //            {
//            //                myparam.Value = n + 1;
//            //                mycommand.ExecuteNonQuery();
//            //            }
//            //        }
//            //        mytransaction.Commit();
//            //    }

//            //    myconnection.Close();
//            //}


//        }

//    }
//}
