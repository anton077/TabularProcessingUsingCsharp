using System.Xml;
using System;
using System.Reflection;
using System.IO;



namespace ConsoleApp1
{
    public static class XMLreader
    {
        public static string currentTabularDB;
        public static string tabularConn;
        public static string dbOracle;
        public static string dbSQLserver;
        public static string stagingSchemaName;
        public static string[] tablesToProcess;
        public static string filepath ="Config.xml";
        public static string minDefaultDate;
        public static string maxDefaultDate;

        public static void XMLinit()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filepath);
                currentTabularDB = doc.DocumentElement.SelectSingleNode("/root/CurrentTabularDB").InnerText;
                tabularConn = doc.DocumentElement.SelectSingleNode("/root/TabularConnectionString").ChildNodes.Item(0).Value;
                try
                {
                    dbOracle = doc.DocumentElement.SelectSingleNode("/root/DBOracleConnectionString").ChildNodes.Item(0).Value;
                                    }
                catch(NullReferenceException nre) { dbOracle = null;  };

                try
                {
                    dbSQLserver = doc.DocumentElement.SelectSingleNode("/root/DBSQLserverConnectionString").ChildNodes.Item(0).Value;
                }
                catch (NullReferenceException nre) { dbSQLserver = null; };


                 stagingSchemaName = doc.DocumentElement.SelectSingleNode("/root/DWHStagingSchemaName").InnerText;
                 minDefaultDate = doc.DocumentElement.SelectSingleNode("/root/defaultdate").Attributes["min"].InnerText;
                 maxDefaultDate = doc.DocumentElement.SelectSingleNode("/root/defaultdate").Attributes["max"].InnerText;
                //run over all tables nodes
                XmlNodeList lst = doc.DocumentElement.SelectNodes("/root/tables/table");
                tablesToProcess = new string[lst.Count];
                int i = 0;
                foreach (XmlNode node in lst)
                {
                    tablesToProcess[i] = node.Attributes["name"].InnerText; //or loop through its children as well
                    i++;
                }


            }
            
            catch (Exception e)
            {
                SimpleLogger.Error(e.ToString());
            }
           
            

        }

        public static  string printData()
        {
            return "\nTabular DB: "+currentTabularDB+ "\nTabular Connection: "+ tabularConn+ "\nOracle Connection: " + dbOracle+ "\nSQL Server Connection: " +
                "\nStaging Schema: " + stagingSchemaName+"\nMin Default Date: "+ minDefaultDate+ "\nMax Default Date: " + maxDefaultDate;
        }
    }
}
