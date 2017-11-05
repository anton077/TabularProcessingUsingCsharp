using System;
using Microsoft.AnalysisServices.Tabular;

namespace ConsoleApp1
{
    class ExtractDateHelper
    {
        public static string getColumnNamePartitionedBy(Table t)
        {
            QueryPartitionSource qs = (QueryPartitionSource)t.Partitions[0].Source;
            string partitionQuery = qs.Query.ToString().Replace("\"", "").ToUpper();
            int first = partitionQuery.IndexOf("WHERE", 0) + "where".Length;
            int last = Math.Min(partitionQuery.IndexOf(">", 0) > 0 ? partitionQuery.IndexOf(">", 0) : partitionQuery.Length,
                            partitionQuery.IndexOf("<", 0) > 0 ? partitionQuery.IndexOf("<", 0) : partitionQuery.Length);

            string fullObjName = partitionQuery.Substring(first, last - first);
            first = fullObjName.LastIndexOf(".") + 1;
            last = fullObjName.Length;
            string tabularPartitionBy = fullObjName.Substring(first, last - first);
            return tabularPartitionBy;
        }
        public static Tuple<DateTime, DateTime> getPartitionFromTo(Partition p)
        {
            QueryPartitionSource qs = (QueryPartitionSource)p.Source;
            string partitionQuery = qs.Query.ToString().Replace("\"", "");
            int greaterInd = partitionQuery.IndexOf(">", 0);
            int lessInd = partitionQuery.IndexOf("<", 0);
            string fromStr, toStr;
            if (greaterInd < lessInd && greaterInd > 0 && lessInd > 0)
            {
                fromStr = (System.Text.RegularExpressions.Regex.Replace(partitionQuery.Substring(greaterInd, lessInd - greaterInd), "[^0-9]+", string.Empty));
                toStr = (System.Text.RegularExpressions.Regex.Replace(partitionQuery.Substring(lessInd, partitionQuery.Length - lessInd), "[^0-9]+", string.Empty));
            }

            else if (greaterInd > lessInd && greaterInd > 0 && lessInd > 0)
            {
                toStr = (System.Text.RegularExpressions.Regex.Replace(partitionQuery.Substring(lessInd, greaterInd - lessInd), "[^0-9]+", string.Empty));
                fromStr = (System.Text.RegularExpressions.Regex.Replace(partitionQuery.Substring(greaterInd, partitionQuery.Length - greaterInd), "[^0-9]+", string.Empty));
            }
            else if (greaterInd < 0 && lessInd > 0)
            {
                toStr = (System.Text.RegularExpressions.Regex.Replace(partitionQuery.Substring(lessInd, partitionQuery.Length - lessInd), "[^0-9]+", string.Empty));
                fromStr = XMLreader.minDefaultDate;
            }
            else if (greaterInd > 0 && lessInd < 0)
            {
                fromStr = (System.Text.RegularExpressions.Regex.Replace(partitionQuery.Substring(greaterInd, partitionQuery.Length - greaterInd), "[^0-9]+", string.Empty));
                toStr = XMLreader.maxDefaultDate;
            }

            else
            {
                fromStr =XMLreader.minDefaultDate;
                toStr = XMLreader.maxDefaultDate;
            }
            return Tuple.Create(DateTime.ParseExact(fromStr, "yyyyMMdd", null), DateTime.ParseExact(toStr, "yyyyMMdd", null));
        }
    }
}
