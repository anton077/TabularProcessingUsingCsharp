using System;
using Oracle.DataAccess.Client;
using System.Data.SqlClient;
using System.Data.Common;

namespace ConsoleApp1
{
    class DWHconnection
    {
       public DateTime minDate, maxDate;
       public DWHconnection(DbConnection conn,string tableName,string columnName,int isOra)
        {
            DbCommand command;
            if (isOra == 1)
            {
                 command = new OracleCommand("SELECT min(" + columnName + ") as maxx, max(" + columnName + ") as minn FROM " + tableName, (OracleConnection)conn);

            }
            else
            {
                 command = new SqlCommand("SELECT min(" + columnName + ") as maxx, max(" + columnName + ") as minn FROM " + tableName, (SqlConnection)conn);

            }

            try
            {
                var reader = command.ExecuteReader();//query db and get min and max date of column that tabular table partitioned by
                while (reader.Read())
                {
                    minDate = DateTime.Parse(reader[0].ToString());
                    maxDate = DateTime.Parse(reader[1].ToString());
                }
            }

            catch(Exception e) { SimpleLogger.Error(e.ToString()); }
            
        }

 

    }
}
