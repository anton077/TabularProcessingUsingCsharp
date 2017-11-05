using System;
using Microsoft.AnalysisServices.Tabular;
using Oracle.DataAccess.Client;
using System.Data.SqlClient;
using System.Data.Common;

/*********************
Author: Anton Shapiro
*********************/

namespace ConsoleApp1
{
    class Program
    {
    static void Main(string[] args)
        {
            Console.WriteLine("Started");
            Boolean success = false;
            XMLreader.XMLinit();//XML load and parameters initialization 
            string connStringTabular = XMLreader.tabularConn;
            string stagingSchemaName = XMLreader.stagingSchemaName;
            string tabularDB = XMLreader.currentTabularDB;
            string connectionStringORA = XMLreader.dbOracle;
            string connectionStringSQLserver = XMLreader.dbSQLserver;
            string[] tableArr = XMLreader.tablesToProcess;
            string midDefautldate = XMLreader.minDefaultDate;
            string maxdefautldate = XMLreader.maxDefaultDate;

            DbConnection dbConn;
            int isOracle=0;
            SimpleLogger.Info(XMLreader.printData());
            DWHconnection[] tableInfoArr = new DWHconnection[tableArr.Length];//get the list of tables need processing
            var svr = new Microsoft.AnalysisServices.Tabular.Server();
            try
            {
                svr.Connect(connStringTabular);
                Microsoft.AnalysisServices.Tabular.Database db = svr.Databases[tabularDB];
                Model m = db.Model;
                SimpleLogger.Info("Connected to DB: " + db.Name.ToString() + " MODEL: " + m.Name.ToString());
                // Check dwh db type
                if (connectionStringORA is null && !(connectionStringSQLserver is null))
                {
                    dbConn = new SqlConnection();
                    dbConn.ConnectionString = connectionStringSQLserver;


                }
                else if (!(connectionStringORA is null) && connectionStringSQLserver is null)
                {
                    dbConn = new OracleConnection();
                    dbConn.ConnectionString = connectionStringORA;
                    isOracle = 1;

                }
                else throw new Exception ("You must provide only one DB connection");
                dbConn.Open();
                int iterator = 0;
                Table t = null;
                string partitionedBy;
                foreach (string tbl in tableArr)
                {
                    t = db.Model.Tables[tbl];
                    partitionedBy = ExtractDateHelper.getColumnNamePartitionedBy(t);//get name of column tabular table partitioned by
                    SimpleLogger.Info("Started working on " +t.Name.ToString());
                    tableInfoArr[iterator] = new DWHconnection(dbConn, stagingSchemaName+'.'+tbl, partitionedBy,isOracle);
                    SimpleLogger.Info("Table " + (iterator+1) + " name: " + tbl+ " Partitioned by: "+ partitionedBy +" \nmindate: " + tableInfoArr[iterator].minDate + " \nmaxdate: " + tableInfoArr[iterator].maxDate);
                    foreach (Partition partition in t.Partitions)//for each partitions extract its range and compare with range of new arrived data
                    {
                        DateTime partitionFrom = ExtractDateHelper.getPartitionFromTo(partition).Item1;
                        DateTime partitionTo = ExtractDateHelper.getPartitionFromTo(partition).Item2;

                        SimpleLogger.Info("\nPartition FROM TO: " + ExtractDateHelper.getPartitionFromTo(partition).Item1 + " " + ExtractDateHelper.getPartitionFromTo(partition).Item2);
                        if (!(partitionTo <= tableInfoArr[iterator].minDate || partitionFrom >= tableInfoArr[iterator].maxDate))//process partitions that overlap new data range
                        {
                            SimpleLogger.Info("starting working on " + partition.Name.ToString() + " that last processed at " + partition.RefreshedTime);
                            partition.RequestRefresh(RefreshType.Full);
                            db.Model.SaveChanges();
                            SimpleLogger.Info("finished working on " + partition.Name.ToString());
                        }
                    }
                    iterator++;

                }
                success = true;
            }

            
            catch (Exception e)
            {
                SimpleLogger.Error(e.ToString());
            }
            finally
            {
                if (svr != null)
                {
                    svr.Disconnect();
                    svr.Dispose();
                }
            }
            if (success)
            {

                SimpleLogger.Info("Finished Successfully");
            }
            else
            {
                Console.ReadLine();

            }

        }


    }
}
