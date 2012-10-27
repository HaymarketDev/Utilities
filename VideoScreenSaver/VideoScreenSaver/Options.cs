using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoScreenSaver.Properties;
using Microsoft.Win32;
using System.IO;

namespace VideoScreenSaver
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            RegistryKey screenSaver = Registry.CurrentUser.OpenSubKey("DPLScreenSaver");
            if (screenSaver != null)
            {
                textBox1.Text = screenSaver.GetValue("VideoPath").ToString();
                screenSaver.Close(); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text))
            {
                using (RegistryKey screenSaver = Registry.CurrentUser.CreateSubKey("DPLScreenSaver"))
                {
                    screenSaver.SetValue("VideoPath", textBox1.Text);
                }
                this.Close(); 
            }
        }
    }
}
