﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DcsConverter
{
    /// <summary>
    /// A class containing extensions to assist in displaying parsed data in a datatable.
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Takes in a datatable as input, and outputs a CSV file.
        /// </summary>
        /// <param name="table">The table to be converted to CSV.</param>
        /// <param name="delimator">The delimeter used in the CSV file.</param>
        /// <returns></returns>
        public static string ToCsv(this DataTable table, string delimator)
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].ColumnName);
                result.Append(i == table.Columns.Count - 1 ? "\n" : delimator);
            }
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i].ToString());
                    result.Append(i == table.Columns.Count - 1 ? "\n" : delimator);
                }
            }
            return result.ToString().TrimEnd(new char[] { '\r', '\n' });
            //return result.ToString();
        }
    }
}
