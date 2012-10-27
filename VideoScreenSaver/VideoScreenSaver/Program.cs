using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

namespace VideoScreenSaver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Trace.WriteLine("SS: starting2");

            if (args.Length > 0)
            {
                // Get the 2 character command line argument
                string arg = args[0].ToLower(CultureInfo.InvariantCulture).Trim().Substring(0, 2);
                Trace.WriteLine("SS: Argument: " + arg);
                switch (arg)
                {
                    case "/c":
                        // Show the options dialog
                        ShowOptions();
                        break;
                    case "/p":
                        // Don't do anything for preview
                        //ShowScreenSaver();
                        break;
                    case "/s":
                        // Show screensaver form
                        ShowScreenSaver();
                        break;
                    default:
                        MessageBox.Show("Invalid command line argument :" + arg, "Invalid Command Line Argument", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                Trace.WriteLine("SS: Started, but no command arguments; so leaving....");
                ShowScreenSaver();
            }
        }

        static void DoIt()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ScreenSaver());
        }

        private static void ShowScreenSaver()
        {
            ScreenSaver ss = new ScreenSaver();
            Application.Run(ss); 
        }

        private static void ShowOptions()
        {
            Options options = new Options();
            Application.Run(options); 
        }
    }
}
