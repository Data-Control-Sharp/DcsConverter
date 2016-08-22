using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DCS_Converter
{
    public partial class Form1 : Form
    {
        private string _openFileDialogFilter = "All files (*.*)|*.*|CSV File (.csv)|*.csv|JSON File (.json)|*.json|XML File (.xml)|*.xml";

        /// <summary>
        /// Initializes the form and assigns any default values.
        /// </summary>
        public Form1(string[] args)
        {
            //Initialize Form
            InitializeComponent();
            this.AcceptButton = button2;
            //saveFileDialog1.Filter = "CSV File (.csv)|*.csv|XML File (.xml)|*.xml|JSON File (.json)|*.json";
        }

        /// <summary>
        /// Responds to any action on button1, the "Browse" button.
        /// </summary>
        /// <param name="sender">Contains information about the event initiator.</param>
        /// <param name="e">Contains event data.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            FileBrowser();
        }

        /// <summary>
        /// Responds to any action on button2, the "Convert" button.
        /// </summary>
        /// <param name="sender">Contains information about the event initiator.</param>
        /// <param name="e">Contains event data.</param>
        private void button2_Click(object sender, EventArgs e)
        {

            //Ensure a file is input.
            if (textBox1.Text.Length > 0)
            {
                errorProvider1.Clear();
                bool download = false;
                string fileLoc = textBox1.Text;
                string fileName = fileLoc.Split('/').Last();

                //Download from URL
                if (textBox1.Text.Contains("http://") || textBox1.Text.Contains("https://"))
                {
                    download = true;
                    //Creates webclient to get the download.
                    using (var client = new WebClient())
                    {
                        try
                        {
                            client.DownloadFile(fileLoc, fileName);
                        }
                        catch (Exception k)
                        {
                            MessageBox.Show("Unable to download file at given URL. More Info: " + k);
                        }
                    }
                }

                bool outputResult = false;
                dynamic parsed = DcsAll.ParseFile(textBox1.Text);

                //Delete downloaded file
                if (download)
                {
                    File.Delete(fileName);
                }

                if (parsed == null)
                {
                    MessageBox.Show("Error: Issue parsing file. Please refer to debug messages.");
                    return;
                }

                //Ensure a file type is selected.
                //CSV
                if (comboBox1.SelectedIndex == 0)
                {
                    errorProvider2.Clear();

                    //CSV output test
                    saveFileDialog1.Filter = "CSV File (.csv)|*.csv";
                    string saveFile = SaveBrowser();
                    outputResult = CustomOutputFile(saveFile, parsed);
                }
                //JSON
                else if (comboBox1.SelectedIndex == 1)
                {
                    errorProvider2.Clear();

                    //JSON output test
                    saveFileDialog1.Filter = "JSON File (.json)|*.json";
                    string saveFile = SaveBrowser();
                    outputResult = CustomOutputFile(saveFile, parsed);
                }
                //XML
                else if(comboBox1.SelectedIndex == 2)
                {
                    errorProvider2.Clear();

                    //XML output test
                    saveFileDialog1.Filter = "XML File (.xml)|*.xml";
                    string saveFile = SaveBrowser();
                    outputResult = CustomOutputFile(saveFile, parsed);
                }
                else
                {
                    errorProvider2.SetError(comboBox1, "No file type specified");
                }

                //Display parsed content
                if (outputResult)
                {
                    DataView dv = new DataView();
                    DataTable dt = DcsAll.ObjToDataTable(parsed);
                    dv.dataGridView1.DataSource = dt;
                    dv.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Error outputing file. Please refer to debugging messages.");
                }

            }
            else
            {
                errorProvider1.SetError(textBox1, "No file specified");
            }

            //Release resources held by save dialog.
            openFileDialog1.FileName = "";
            saveFileDialog1.FileName = "";

            //Conversion complete
        }

        /// <summary>
        /// This function will take in a dynamic object and output a specified file type.
        /// </summary>
        /// <param name="saveFileName">The full path of the new save file.</param>
        /// <param name="fileType">The file type of the new save file.</param>
        /// <param name="parsed">The parsed content to be output to file.</param>
        public bool CustomOutputFile(string saveFileName, dynamic parsed)
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
                    bool result = DcsCsv.OutputCsv(parsed, saveFileName,
                        this.checkBox3.Checked, this.textBox2.Text,
                        this.comboBox2.GetItemText(this.comboBox2.SelectedItem));
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
                    bool result = DcsJson.OutputJson(parsed, saveFileName, this.comboBox3.GetItemText(this.comboBox3.SelectedItem), 
                        this.checkBox6.Checked, this.checkBox5.Checked, this.checkBox7.Checked);
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
                    bool result = DcsXml.OutputXml(parsed, saveFileName, this.textBox3.Text,
                        this.checkBox2.Checked, this.checkBox4.Checked,
                        this.checkBox1.Checked);
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

        /// <summary>
        /// This function displays a file browser to the user, allowing them to select a file. 
        /// The file name is then placed in textBox1.
        /// </summary>
        /// <returns>File name of the selected file.</returns>
        public string FileBrowser()
        {
            string file = null;
            DialogResult result = openFileDialog1.ShowDialog(); //Show the dialog.
            if (result == DialogResult.OK) //Test result.
            {
                file = openFileDialog1.FileName;
                textBox1.Text = file;
            }
            Console.WriteLine(result); //Used for debugging purposes
            return file;
        }

        /// <summary>
        /// This function displays a file save browser to the user, allowing them to select a file. 
        /// </summary>
        /// <returns>File name of the selected file.</returns>
        public string SaveBrowser()
        {
            string file = null;
            DialogResult result = saveFileDialog1.ShowDialog(); //Show the dialog.
            if (result == DialogResult.OK) //Test result.
            {
                file = saveFileDialog1.FileName;
                
            }
            Console.WriteLine(result); //Used for debugging purposes
            return file;
        }

        /// <summary>
        /// Actions that occur before the form is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = _openFileDialogFilter;
            saveFileDialog1.FileName = "";

            Dictionary<string, string> combo = new Dictionary<string, string>();
            combo.Add("1", "UTF8");
            combo.Add("2", "ASCII");
            comboBox2.DataSource = new BindingSource(combo, null);
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
            comboBox2.SelectedIndex = 0;

            Dictionary<string, string> combo2 = new Dictionary<string, string>();
            combo2.Add("1", "ISO");
            combo2.Add("2", "MS");
            comboBox3.DataSource = new BindingSource(combo2, null);
            comboBox3.DisplayMember = "Value";
            comboBox3.ValueMember = "Key";
            comboBox3.SelectedIndex = 0;

            optionPanel1.Hide();
            optionPanel2.Hide();
            panel3.Hide();
            optionPanel2.Location = optionPanel1.Location;
            panel3.Location = optionPanel1.Location;
        }

        /// <summary>
        /// Event that occurs when the comboBox selection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text == "CSV")
            {
                optionPanel2.Hide();
                panel3.Hide();
                optionPanel1.Show();        
            }
            else if(comboBox1.Text == "XML")
            {
                optionPanel1.Hide();
                panel3.Hide();
                optionPanel2.Show();                
            }
            else if(comboBox1.Text == "JSON")
            {
                optionPanel1.Hide();
                optionPanel2.Hide();
                panel3.Show();
            }
            else
            {
                optionPanel1.Hide();
                optionPanel2.Hide();
                panel3.Hide();
            }
        }
    }



}
