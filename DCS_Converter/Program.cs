using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using CommandLine.Text;

namespace DCS_Converter
{
    static class Program
    {

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int AttachParentProcess = -1;

        /// <summary>
        /// The class created to receieve parsed values from the CL Parser.
        /// </summary>
        public class Options
        {

            [Option('l', "delimeter", DefaultValue = ",", Required = false,
              HelpText = "Delimeter to be used for CSV Output.")]
            public string Delimeter { get; set; }

            [Option('e', "encoding", DefaultValue = "UTF8", Required = false,
              HelpText = "Encoding to be performed on CSV output.")]
            public string Encoding { get; set; }

            [Option('h', "header", Required = false,
              HelpText = " Whether or not headers are used in CSV output.")]
            public bool Header { get; set; }

            [Option('r', "root", DefaultValue = "ROOT", Required = false,
              HelpText = "The root node to be used for XML output.")]
            public string Root { get; set; }

            [Option('i', "stripindent", Required = false,
              HelpText = "Whether or not to strip indent in XML/JSON output.")]
            public bool StripIndent { get; set; }

            [Option('n', "stripnewline", Required = false,
              HelpText = "Whether or not to strip new line in XML output.")]
            public bool StripNewLine { get; set; }

            [Option('d', "stripdeclaration", Required = false,
              HelpText = "Whether or not to strip declaration in XML output.")]
            public bool StripDeclaration { get; set; }

            [Option('u', "stripnull", Required = false,
              HelpText = "Whether or not to strip null values in JSON output.")]
            public bool StripNull { get; set; }

            [Option('a', "stripnonascii", Required = false,
              HelpText = "Whether or not to strip non ASCII values in JSON output.")]
            public bool StripNonAscii { get; set; }

            [Option('f', "dateformat", DefaultValue = "ISO", Required = false,
              HelpText = "Format to be used for dates in JSON (ISO or MICROSOFT).")]
            public string DateFormat { get; set; }



            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            AttachConsole(AttachParentProcess);

            //If no arguments are specified, launch windows form.
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(null));
            }
            else
            {
                //If argument length is greater than or equal to two
                if (args.Length >= 2)
                {
                    //Parse Options
                    var options = new Options();
                    if (CommandLine.Parser.Default.ParseArguments(args, options))
                    {
                        //Parse files
                        dynamic parsed = DcsAll.ParseFile(args[0]);
                        if (parsed != null)
                        {
                            //Output file
                            bool result = CustomOutputFile(args[1], parsed, options);
                            if (result)
                            {
                                //Complete
                                Console.WriteLine("Conversion complete.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid argument count.");
                }
                Application.Exit();
            }
        }

        /// <summary>
        /// This function will take in a dynamic object and output a specified file type.
        /// </summary>
        /// <param name="saveFileName">The full path of the new save file.</param>
        /// <param name="fileType">The file type of the new save file.</param>
        /// <param name="parsed">The parsed content to be output to file.</param>
        public static bool CustomOutputFile(string saveFileName, dynamic parsed, Options options)
        {

            string fileType = null;

            //Extract file extension.
            if (saveFileName != null)
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
                    bool result = DcsCsv.OutputCsv(parsed, saveFileName, options.Header, options.Delimeter, options.Encoding);
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
                    bool result = DcsJson.OutputJson(parsed, saveFileName, options.DateFormat,
                        options.StripIndent, options.StripNull, options.StripNonAscii);
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
                    bool result = DcsXml.OutputXml(parsed, saveFileName, options.Root, 
                        options.StripIndent, options.StripNewLine, options.StripDeclaration);
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

    }
}
