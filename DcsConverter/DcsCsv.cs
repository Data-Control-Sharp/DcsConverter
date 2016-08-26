using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Data;
using System.Xml;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace DcsConverter
{

    /// <summary>
    /// This class will perform all operations related to CSV files.
    /// </summary>
    public class DcsCsv
    {
        /// <summary>
        /// Reads a JSON file and parses it to a dynamic object.
        /// </summary>
        /// <param name="fileName">The FULL path of the CSV file to convert.</param>
        /// <returns>A dynamic object, representing the CSV file.
        /// A row from the dynamic object is obtained by calling "rowxcoly" where "x" 
        /// is the number of the row starting at 0 and y is the number of the column starting at 0.</returns>
        public static dynamic ParseCsv(string fileName)
        {
            //TODO: Create more specific try/catch blocks.
            try {
                //*///  WIP XML/JSON IMPLEMENTATION
                char delimiter = ',';
                //TODO: Add support for single-quote, only works for double quotes currently.
                string pattern = delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
                string[] csvLines = File.ReadAllLines(fileName);//TODO catch IO errors
                Dictionary<string, Dictionary<string, string>> rootDict = new Dictionary<string, Dictionary<string, string>>();

                //I am treating the first line as the fields and every subsequent line afterwards as data
                //string[] line, header = csvLines[0].Split(delimiter);//TODO generalize for no header case?
                string[] line, header = Regex.Split(csvLines[0], pattern);
                for (int i = 1; i < csvLines.Length; i++)
                {
                    //line = csvLines[i].Split(delimiter);
                    line = Regex.Split(csvLines[i], pattern);
                    Dictionary<string, string> inner = new Dictionary<string, string>();

                    for (int j = 0; j < line.Length; j++)
                        inner.Add(header[j], line[j]);

                    rootDict.Add("Row" + i, inner);
                }
                return rootDict;
            }
            catch(Exception e)
            {
                Console.WriteLine("Parsing Error: " + e);
                return null;
            }

            //*///  WIP XML IMPLEMENTATION - END


            /*///  Better JSON IMPLEMENTATION?  Doesn't work for XML
            char delimiter = ',';
            string[] csvLines = File.ReadAllLines(fileName);//TODO catch IO errors
            Dictionary<string, List<Dictionary<string, string>>> rootDict = new Dictionary<string, List<Dictionary<string, string>>>();

            //I am treating the first line as the fields and every subsequent line afterwards as data
            string[] line, header = csvLines[0].Split(delimiter);//TODO generalize for no such case?
            List<Dictionary<string, string>> theList = new List<Dictionary<string, string>>();
            for (int i = 1; i < csvLines.Length; i++)
            {
                line = csvLines[i].Split(delimiter);
                Dictionary<string, string> inner = new Dictionary<string, string>();

                for (int j = 0; j < line.Length; j++)
                    inner.Add(header[j], line[j]);

                theList.Add(inner);
            }
            rootDict.Add("ROOT", theList);
            return rootDict;

            //*///  Better JSON IMPLEMENTATION?

        }

        /// <summary>
        /// Loads CSV content and converts it to a C# object.
        /// </summary>
        /// <param name="csvContent">A string containing the CSV content to be parsed.</param>
        /// <returns>A dynamic object, representing the v file.</returns>
        public static dynamic ParseCsvData(string csvContent)
        {
            //TODO: Create more specific try/catch blocks.
            try
            {
                //*///  WIP XML/JSON IMPLEMENTATION
                char delimiter = ',';
                //TODO: Add support for single-quote, only works for double quotes currently.
                string pattern = delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
                string[] csvLines = csvContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None); //TODO catch IO errors
                Dictionary<string, Dictionary<string, string>> rootDict =
                    new Dictionary<string, Dictionary<string, string>>();

                //I am treating the first line as the fields and every subsequent line afterwards as data
                //string[] line, header = csvLines[0].Split(delimiter);//TODO generalize for no header case?
                string[] line, header = Regex.Split(csvLines[0], pattern);
                for (int i = 1; i < csvLines.Length; i++)
                {
                    //line = csvLines[i].Split(delimiter);
                    line = Regex.Split(csvLines[i], pattern);
                    Dictionary<string, string> inner = new Dictionary<string, string>();

                    for (int j = 0; j < line.Length; j++)
                        inner.Add(header[j], line[j]);

                    rootDict.Add("Row" + i, inner);
                }
                return rootDict;
            }
            catch (Exception e)
            {
                Console.WriteLine("Parsing Error: " + e);
                return null;
            }
        }

        /// <summary>
        /// This parses the data in JSON array form.  Details are shown at http://www.convertcsv.com/csv-to-json.htm
        /// </summary>
        /// <param name="fileName">The FULL path of the CSV file to convert.</param>
        /// <returns>A dynamic object, representing the CSV file.</returns>
        public static dynamic parseCSV_AsJsonArray(string fileName)
        {
            //TODO: Create more specific try/catch blocks.
            try
            {
                char delimiter = ',';
                //TODO: Add support for single-quote, only works for double quotes currently.
                string pattern = delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
                string[] csvTxt = File.ReadAllLines(fileName);//TODO catch IO errors
                List<string[]> csvIntermediate = new List<string[]>();
                foreach (string line in csvTxt)
                    csvIntermediate.Add(Regex.Split(line, pattern));
                string jsonTxt = new JavaScriptSerializer().Serialize(csvIntermediate);
                return JsonConvert.DeserializeObject(jsonTxt);
            }
            catch(Exception e)
            {
                Console.WriteLine("Parsing Error: " + e);
                return null;
            }
        }


        /// <summary>
        /// Writes a C# object to a CSV file.  See http://www.codeproject.com/Tips/565920/Create-CSV-from-JSON-in-Csharp for implementation data.
        /// This article, along with any associated source code and files, is licensed under The Code Project Open License (CPOL).
        /// </summary>
        /// <param name="obj">The object to write to file.</param>
        /// <param name="filePath">The FULL path of the file to write.</param>
        /// <param name="header">Whether or not the CSV data contains a header. True by default.</param>
        /// <param name="delimeter">A specified delimeter to seperate the values. Comma by default.</param>
        /// <param name="encoding">The specified character encoding. UTF8 by default.</param>
        public static bool OutputCsv(dynamic obj, string filePath, bool header = true, string delimeter = ",", string encoding = "UTF8")
        {

            if (obj == null)
            {
                return false;
            }

            //Recursive parsing test
            //http://www.codeproject.com/Tips/565920/Create-CSV-from-JSON-in-Csharp]
            //This article, along with any associated source code and files, is licensed under The Code Project Open License (CPOL)
            try
            {
                string json = JsonConvert.SerializeObject(obj);//TODO woefully inefficient step here. Converting back to XML to use their convenient methods.  See if we can rework in Newtonsoft.

                XmlNode xml;
                try
                {
                    xml = JsonConvert.DeserializeXmlNode(json);
                }
                catch (JsonSerializationException)// Typically indication that a root node does not exist when creating xmlData
                {
                    xml = JsonConvert.DeserializeXmlNode(json, "ROOT");
                }
                catch (InvalidOperationException)
                {
                    xml = JsonConvert.DeserializeXmlNode(json, "ROOT");
                }

                XmlDocument xmldoc = new XmlDocument();
                //Create XmlDoc Object
                xmldoc.LoadXml(xml.InnerXml);
                //Create XML Steam 
                var xmlReader = new XmlNodeReader(xmldoc);
                DataSet dataSet = new DataSet();
                //Load Dataset with Xml
                dataSet.ReadXml(xmlReader);
                //return single table inside of dataset
                string csv = dataSet.Tables[0].ToMyCsv(delimeter, header);

                FileStream fs = null;
                StreamWriter sw = null;

                try {
                    //Set encoding of the output.
                    if (encoding == "UTF8")
                    {
                        fs = new FileStream(filePath, FileMode.Create);
                        sw = new StreamWriter(fs, Encoding.UTF8);
                    }
                    else if (encoding == "ASCII")
                    {
                        fs = new FileStream(filePath, FileMode.Create);
                        sw = new StreamWriter(fs, Encoding.ASCII);
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid encoding. Supported encodings are UTF8 and ASCII.");
                        return false;
                    }
                    sw.Write(csv);
                }
                catch(Exception e2)
                {
                    Console.WriteLine("Output Writing Error: " + e2);
                    return false;
                }
                finally
                {
                    sw.Close();
                    fs.Close();                 
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Output Error: " + e);
                return false;
            }
        }
    }

    /// <summary>
    /// Extensions class for outputCSV
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// A CSV creator based on http://www.codeproject.com/Tips/565920/Create-CSV-from-JSON-in-Csharp.
        /// This article, along with any associated source code and files, is licensed under The Code Project Open License (CPOL)
        /// </summary>
        /// <param name="table">data table</param>
        /// <param name="delimiter">delimiter</param>
        /// <param name="header">Whether or not the CSV data contains a header. True by default.</param>
        /// <returns></returns>
        public static string ToMyCsv(this DataTable table, string delimiter, bool header = true)
        {
            var result = new StringBuilder();
            if (header)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(table.Columns[i].ColumnName);
                    result.Append(i == table.Columns.Count - 1 ? "\n" : delimiter);
                }
            }
            
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i].ToString());
                    result.Append(i == table.Columns.Count - 1 ? "\n" : delimiter);
                }
            }
            return result.ToString().TrimEnd(new char[] { '\r', '\n' });
            //return result.ToString();
        }
    }

}

