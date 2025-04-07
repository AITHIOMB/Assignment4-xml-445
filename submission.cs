using System;
using System.Xml.Schema;
using System.Xml;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class Program
    {
        // These URLs will be read by my gradescope
        public static string xmlURL = "";
        public static string xmlErrorURL = "";
        public static string xsdURL = "";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

         
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Method to verify the  XML and the XSD 
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            StringBuilder errorMessages = new StringBuilder();
            bool validationError = false;

            try
            {
                // Set up the validation settings process
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                // Add schema to settings
                using (WebClient client = new WebClient())
                {
                    string xsdContent = client.DownloadString(xsdUrl);
                    XmlSchema schema = XmlSchema.Read(new StringReader(xsdContent), null);
                    settings.Schemas.Add(schema);
                }

                // Set validation event handler process
                settings.ValidationEventHandler += (sender, e) => {
                    validationError = true;
                    errorMessages.AppendLine($"Line {e.Exception.LineNumber}, Position {e.Exception.LinePosition}: {e.Message}");
                };

                // Validate XML against schema process
                using (WebClient client = new WebClient())
                {
                    string xmlContent = client.DownloadString(xmlUrl);
                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlContent), settings))
                    {
                        // Read through the entire XML and trigger validation
                        while (reader.Read()) { }
                    }
                }

                return validationError ? errorMessages.ToString() : "No Error";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        // This is the method to convert XML to JSON 
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                // Download XML content process
                string xmlContent;
                using (WebClient client = new WebClient())
                {
                    xmlContent = client.DownloadString(xmlUrl);
                }

                // Load XML process
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                // Convert to JSON process
                string jsonText = JsonConvert.SerializeXNode(doc, Formatting.Indented, true);
                return jsonText;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}