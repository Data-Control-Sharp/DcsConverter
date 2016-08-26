using System;
using Newtonsoft.Json;

namespace DcsConverter
{

    /// <summary>
    /// This class is used for all JSON related conversion within <c>DCS_Converter</c>
    /// </summary>
    //TODO finish testing
    public class DcsJson
    {
        //TODO add JSON conversion methods

        /// <summary>
        /// Reads a JSON file and parses it to a dynamic object.
        /// </summary>
        /// <param name="filePath">The FULL path of the JSON file to convert.</param>
        /// <returns>A dynamic object, representing the JSON file.</returns>
        public static dynamic ParseJson(string filePath)
        {
            // read file into a string and deserialize JSON to a type
            try {
                dynamic obj = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(filePath));
                return obj;
            }
            catch(Exception e)
            {
                Console.WriteLine("Parsing Error: " + e);
                return null;
            }
        }

        /// <summary>
        /// Loads JSON content and converts it to a C# object.
        /// </summary>
        /// <param name="jsonContent">A string containing the JSON content to be parsed.</param>
        /// <returns>A dynamic object, representing the JSON file.</returns>
        public static dynamic ParseJsonData(string jsonContent)
        {
            // read file into a string and deserialize JSON to a type
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(jsonContent);
                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine("Parsing Error: " + e);
                return null;
            }
        }

        /// <summary>
        /// Writes a C# object to a JSON file.
        /// </summary>
        /// <param name="obj">The object to write to file.</param>
        /// <param name="filePath">The FULL path of the file to write.</param>
        /// <param name="dateFormat">The date format. ISO by default.</param>
        /// <param name="stripIndent">Whether or not to strip indents. False by default.</param>
        /// <param name="stripNull">Whether or not to strip null values. True by default.</param>
        /// <param name="stripNonAscii">Whether or not to strip non-ascii values. False by default.</param>
        public static bool OutputJson(dynamic obj, string filePath, string dateFormat = "ISO", bool stripIndent = false, bool stripNull = true, bool stripNonAscii = false)
        {

            if (obj == null)
            {
                return false;
            }

            try {
                //Additional options
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.Indented;
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.StringEscapeHandling = StringEscapeHandling.Default;
                if (stripIndent) settings.Formatting = Formatting.None;
                if (!stripNull) settings.NullValueHandling = NullValueHandling.Include;
                if (stripNonAscii) settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                if (dateFormat.ToUpper() == "ISO") settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                else if (dateFormat.ToUpper() == "MICROSOFT") settings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                else Console.WriteLine("Unsorported date format, supported formats are ISO and MICROSOFT");

                // serialize dynamic object to JSON
                string json = JsonConvert.SerializeObject(obj, settings);
                //Console.WriteLine(json);
                System.IO.File.WriteAllText(filePath, json);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Output Error: " + e);
                return false;
            }
        }
    }
}
