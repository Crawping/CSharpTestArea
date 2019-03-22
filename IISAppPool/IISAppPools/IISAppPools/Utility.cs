using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
//using System.Web.Configuration;
using System.Windows;


namespace IISAppPools
{
    class Utility
    {
        public static string DeBase64(string text)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(text));
        }

        public static string EnBase64(string text)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(text));
        }

        public static string GetDataSourceFromConnStr(string connStr)
        {
            string str = string.Empty;
            //string pattern = @"data\s*source\s*=\s*.*\s*,\s*\d+";
            string pattern = @"data\s*source\s*=(?:[^;]*)";
            Match match1 = Regex.Match(connStr, pattern, RegexOptions.IgnoreCase);
            
            if (match1.Length > 0)
            {
                Match match2 = Regex.Match(match1.Value, @"(?<=\=).*", RegexOptions.IgnoreCase);
                if (match2.Length > 0)
                     str = match2.Value;
                return str;
            }

            return str;
        }

        public static string GetDatabaseFromConnStr(string connStr)
        {
            string result = string.Empty;
            //string pattern = @"data\s*source\s*=\s*.*\s*,\s*\d+";
            string pattern = @"database\s*=(?:[^;]*)";
            Match match1 = Regex.Match(connStr, pattern, RegexOptions.IgnoreCase);

            if (match1.Length > 0)
            {
                Match match2 = Regex.Match(match1.Value, @"(?<=\=).*", RegexOptions.IgnoreCase);
                if (match2.Length > 0)
                    result = match2.Value;
                return result;
            }
            return result;
        }

        public static string SetNewConnStr(string connStr, string dataSource, string database)
        {
            string result = string.Empty;
            string dataSourcePattern1 = @"data\s*source\s*=(?:[^;]*)";
            string databasePattern1 = @"database\s*=(?:[^;]*)";
            Match dataSourceMatch1 = Regex.Match(connStr, dataSourcePattern1, RegexOptions.IgnoreCase);
            Match databaseMatch1 = Regex.Match(connStr, databasePattern1, RegexOptions.IgnoreCase);
            if (dataSourceMatch1.Length > 0 && databaseMatch1.Length > 1)
            {
                string dataSourceValue1 = dataSourceMatch1.Value;
                string dataSourcePattern2 = @"(?<=\=).*";
                Match dataSourceMatch2 = Regex.Match(dataSourceValue1, dataSourcePattern2, RegexOptions.IgnoreCase);

                string databaseValue1 = databaseMatch1.Value;
                string databasePattern2 = @"(?<=\=).*";
                Match databaseMatch2 = Regex.Match(databaseValue1, databasePattern2, RegexOptions.IgnoreCase);

                if (dataSourceMatch2.Length > 0 && databaseMatch2.Length > 0)
                {
                    string dataSourceNewValue = dataSourceValue1.Replace(dataSourceMatch2.Value, string.Format("{0}", (object) dataSource));
                    string databaseNewValue = databaseValue1.Replace(databaseMatch2.Value, string.Format("{0}", (object) database));
                    //newValue = newValue.Replace()
                    result = connStr.Replace(dataSourceValue1, dataSourceNewValue);
                    result = result.Replace(databaseValue1, databaseNewValue);
                }
            }

            return result;
        }
    }
}