using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DCS_Converter
{

    /// <summary>
    /// This class performs actions that involve DCS_CSV, DCS_JSON, and DCS_XML
    /// </summary>
    public class DCS_ALL
    {
        /// <summary>
        /// This function will parse a data file.
        /// </summary>
        /// <param name="fileName">The full file  name of the file to parse.</param>
        /// <returns>Dynamic parsed object.</returns>
        public static dynamic parseFile(string fileName)
        {

            string fileType = null;

            //Extract file extension.
            fileType = fileName.Split('.').Last();

            //Parse Data based on file type
            dynamic parsed = null;
            if (fileType.ToUpper() == "CSV")
            {
                parsed = DCS_CSV.parseCSV(fileName);
            }
            else if (fileType.ToUpper() == "JSON")
            {
                parsed = DCS_JSON.parseJSON(fileName);
            }
            else if (fileType.ToUpper() == "XML")
            {
                parsed = DCS_XML.parseXML(fileName);
            }
            else
            {
                Console.WriteLine("Error: Invalid file extension. Please use a new input file.");
                return null;
            }
            return parsed;
        }

        /// <summary>
        /// This function will take in a dynamic object and output a specified file type.
        /// </summary>
        /// <param name="saveFileName">The full path of the new save file.</param>
        /// <param name="fileType">The file type of the new save file.</param>
        /// <param name="parsed">The parsed content to be output to file.</param>
        public static bool outputFile(string saveFileName, dynamic parsed)
        {

            string fileType = null;

            //Extract file extension.
            if(saveFileName != null)
            {
                fileType = saveFileName.Split('.').Last();
            }
            else
            {
                return false;
            }

            if (fileType.ToUpper() == "CSV")
            {
                if (saveFileName != null)
                {
                    bool result = DCS_CSV.outputCSV(parsed, saveFileName);
                    return result;
                }
                else
                {
                    Console.WriteLine("Error: Null file name.");
                    return false;
                }
            }
            else if (fileType.ToUpper() == "JSON")
            {
                if (saveFileName != null)
                {
                    bool result = DCS_JSON.outputJSON(parsed, saveFileName);
                    return result;
                }
                else
                {
                    Console.WriteLine("Error: Null file name.");
                    return false;
                }
            }
            else if (fileType.ToUpper() == "XML")
            {
                if (saveFileName != null)
                {
                    bool result = DCS_XML.outputXML(parsed, saveFileName);
                    return result;
                }
                else
                {
                    Console.WriteLine("Error: Null file name.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Error: Invalid file type specified.");
                return false;
            }
        }

        //TODO: Include all tables. This is an issue for CSV testing.
        public static DataTable objToDataTable(dynamic obj)
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

            //Return merged tables from the dataset            
            DataTable allDTs = new DataTable();
            
            foreach (DataTable table in dataSet.Tables)
            {
                int rowCount = allDTs.Rows.Count - 1;
                foreach (DataRow row in table.Rows)
                {
                    rowCount = rowCount + 1;
                    allDTs.Rows.Add();

                    int colCount = -1;
                    foreach (DataColumn col in table.Columns)
                    {
                        colCount = colCount + 1;
                        if (colCount > allDTs.Columns.Count - 1) allDTs.Columns.Add(col.ColumnName);
                        allDTs.Rows[rowCount][colCount] = row[col];
                    }
                }
            }
            return allDTs;
        }
    }
}
