using System;
using Newtonsoft.Json;
using System.Xml;
//TODO clean print statements
//TODO exception handling

namespace DcsConverter
{

    /// <summary>
    /// This class is used for all XML related conversion within <c>DCS_Converter</c>
    /// </summary>
    public class DcsXml
    {
        //TODO test thoroughly

        /// <summary>
        /// Loads an XML file and converts it to a C# object.
        /// </summary>
        /// <param name="filePath">The FULL path of the file to convert.</param>
        /// <returns>A dynamic object, representing the XML file.</returns>
        public static dynamic ParseXml(string filePath)
        {
            try
            {
                //import XML file
                XmlDocument xmlData = new XmlDocument();
                xmlData.Load(filePath);

                //Strip any XML declaration data from the file -- it messes with root node conversion
                foreach (XmlNode n in xmlData)
                    if (n.NodeType == XmlNodeType.XmlDeclaration)
                        xmlData.RemoveChild(n);

                //Convert to JSON (temporary workaround)
                string jsonData = JsonConvert.SerializeXmlNode(xmlData);

                //Convert to Object type
                return JsonConvert.DeserializeObject(jsonData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Parsing Error: " + e);
                return null;
            }
        }

        //TODO test thoroughly

        /// <summary>
        /// Loads XML content and converts it to a C# object.
        /// </summary>
        /// <param name="xmlContent">A string containing the XML content to be parsed.</param>
        /// <returns>A dynamic object, representing the XML file.</returns>
        public static dynamic ParseXmlData(string xmlContent)
        {
            try
            {
                //import XML file
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(xmlContent);

                //Strip any XML declaration data from the file -- it messes with root node conversion
                foreach (XmlNode n in xmlData)
                    if (n.NodeType == XmlNodeType.XmlDeclaration)
                        xmlData.RemoveChild(n);

                //Convert to JSON (temporary workaround)
                string jsonData = JsonConvert.SerializeXmlNode(xmlData);

                //Convert to Object type
                return JsonConvert.DeserializeObject(jsonData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Parsing Error: " + e);
                return null;
            }
        }

        /// <summary>
        /// Writes a C# object to an XML file.  If there are multiple root nodes in the object, 
        /// a ROOT node will be written to surround the file.
        /// </summary>
        /// <param name="obj">The object to write to file.</param>
        /// <param name="filePath">The FULL path of the file to write.</param>
        /// <param name="rootNode">Name of the root node.  "ROOT" by default.</param>
        /// <param name="stripIndent">Boolean option to strip indentation whitespace in the file.  False by default</param>
        /// <param name="stripNewline">Boolean option to strip newline whitespace in the file.  False by default</param>
        /// <param name="stripDeclaration">Boolean option to strip the xml declaration header in the file.  True by default</param>
        public static bool OutputXml(dynamic obj, string filePath, string rootNode = "ROOT", bool stripIndent = false, bool stripNewline = false, bool stripDeclaration = true)
        {

            if (obj == null)
            {
                return false;
            }

            try {
                //Once again I go through the JSON converter
                string jsonString = JsonConvert.SerializeObject(obj);
                XmlDocument xmlData;
                try
                {
                    xmlData = JsonConvert.DeserializeXmlNode(jsonString);
                }
                catch (JsonSerializationException)// Typically indication that a root node does not exist when creating xmlData
                {
                    xmlData = JsonConvert.DeserializeXmlNode(jsonString, rootNode);
                }
                catch (InvalidOperationException)
                {
                    xmlData = JsonConvert.DeserializeXmlNode(jsonString, rootNode);
                }
                //Creating settings for the XML document based on function parameters
                XmlWriterSettings xwSettings = new XmlWriterSettings();
                xwSettings.Indent = !stripIndent;
                if (stripNewline) xwSettings.NewLineChars = string.Empty;
                xwSettings.OmitXmlDeclaration = stripDeclaration;

                //saving the document
                using (XmlWriter xWriter = XmlWriter.Create(filePath, xwSettings))
                {
                    xmlData.Save(xWriter);
                    xWriter.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Output Error: " + e);
                return false;
            }
        }
    }
}
