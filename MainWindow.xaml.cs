using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WK.Libraries.SharpClipboardNS;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace clipboard_process_starter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SharpClipboard clipboard;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the SharpClipboard component which monitors the clipboard for changes.
            clipboard = new SharpClipboard();
            clipboard.ClipboardChanged += ClipboardChanged;
        }

        /// <summary>
        /// Logs something in the main window log text box.
        /// </summary>
        /// <param name="message">The message to be logged</param>
        void log(string message)
        {
            logTextBox.Text += DateTime.Now.ToString() + " | " + message + "\r\n";
        }

        /// <summary>
        /// Called when the clipboard is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            if (e.ContentType == SharpClipboard.ContentTypes.Text)
            {
                log("Clipboard contents were changed.");

                string? content = Clipboard.GetDataObject().GetData(DataFormats.StringFormat).ToString();

                if (content != null)
                {
                    string? uniqueId = extractUniqueId(content);

                    if (uniqueId != null)
                    {
                        log($"Found Unique-ID in clipbaord: '{uniqueId}'");

                        // TODO: File is created but never deleted
                        log("Creating .otr file...");
                        var filename = createOtrFile(uniqueId);
                        log($"Created .otr file: {filename}");

                        log("Launching .otr file...");
                        openFile(filename);
                        log("Launched .otr file");
                    }
                    else
                    {
                        log("No Unique-ID found in clipboard.");
                    }
                }
            }
        }

        /// <summary>
        /// Create an .otr file which will open the object with the given Unique ID.
        /// </summary>
        /// <param name="uniqueId">Unique ID of the object to be opened.</param>
        /// <returns>Path to the .otr file.</returns>
        private static string createOtrFile(string uniqueId)
        {
            // TODO: Should be configurable
            string serverHost = "omnitracker-srv";
            UInt16 serverPort = 5085;

            string fileName = $"{System.IO.Path.GetTempPath()}clipboard-process-starter.{Guid.NewGuid()}.otr";

            string content = $"OTR2/{uniqueId}/0000000000/{serverHost}/{serverPort}/0";

            File.WriteAllText(fileName, content);

            return fileName;
        }

        /// <summary>
        /// Opens a file with the default file handler.
        /// </summary>
        /// <param name="filename"></param>
        private static void openFile(string filename)
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(filename)
                {
                    UseShellExecute = true
                }
            }.Start();
        }

        /// <summary>
        /// Extracts the Unique ID from a given String.
        /// </summary>
        /// <param name="input">String to extract the Unique ID from</param>
        /// <returns>Unique ID</returns>
        private string? extractUniqueId(string input)
        {
            string pattern = @"#Unique-ID: (.*)#";

            log("Searching for pattern in clipboard...");
            MatchCollection matches = Regex.Matches(input, pattern);

            string? serialId = null;

            foreach (Match match in matches)
            {
                log($"Found pattern match'{match.Groups[1].Value}'");
                serialId = match.Groups[1].Value;
            }

            return serialId;
        }
    }
}
