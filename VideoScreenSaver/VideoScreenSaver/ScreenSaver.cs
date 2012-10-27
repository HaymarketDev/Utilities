using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace VideoScreenSaver
{
    public partial class ScreenSaver : Form
    {
        string videosLocation = "";
        List<FileInfo> videosToPlay;
        AxWMPLib._WMPOCXEvents_MouseMoveEvent _lastPoint;
        bool _initialized = false;

        public ScreenSaver()
        {
            InitializeComponent();
        }

        private void ScreenSaver_Load(object sender, EventArgs e)
        {
            Cursor.Hide();

            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.Black;
            this.TopMost = true;

            this.Width = Screen.AllScreens.Sum(s => s.Bounds.Width) * Screen.AllScreens.Count();
            this.Height = Screen.AllScreens.Sum(s => s.Bounds.Height) * Screen.AllScreens.Count();

            this.Location = new Point(Screen.AllScreens.Min(s => s.Bounds.X),
                Screen.AllScreens.Min(s => s.Bounds.Y));

            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.enableContextMenu = false;
            axWindowsMediaPlayer1.Left = Screen.PrimaryScreen.Bounds.X - this.Location.X;
            axWindowsMediaPlayer1.Top = Screen.PrimaryScreen.Bounds.Y - this.Location.Y;
            axWindowsMediaPlayer1.Width = Screen.PrimaryScreen.Bounds.Width;
            axWindowsMediaPlayer1.Height = Screen.PrimaryScreen.Bounds.Height;

            axWindowsMediaPlayer1.MouseMoveEvent += new AxWMPLib._WMPOCXEvents_MouseMoveEventHandler(axWindowsMediaPlayer1_MouseMoveEvent);
            axWindowsMediaPlayer1.KeyDownEvent += new AxWMPLib._WMPOCXEvents_KeyDownEventHandler(axWindowsMediaPlayer1_KeyDownEvent);
            axWindowsMediaPlayer1.Focus();

            RegistryKey screenSaver = Registry.CurrentUser.OpenSubKey("DPLScreenSaver");
            if (screenSaver != null)
            {
                videosLocation = screenSaver.GetValue("VideoPath").ToString();
                videosToPlay = GetVideos(videosLocation);
                screenSaver.Close();

                //start playing the 'next video' even though current index = -1
                SetupPlaylistAndPlay();
            }
        }

        void axWindowsMediaPlayer1_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
        {
            switch (e.nKeyCode)
            {
                case (short)Keys.N:
                    axWindowsMediaPlayer1.Ctlcontrols.next();
                    break;
                case (short)Keys.B:
                    axWindowsMediaPlayer1.Ctlcontrols.previous();
                    break;
                case (short)Keys.Oemcomma:
                    axWindowsMediaPlayer1.Ctlcontrols.fastReverse();
                    break;
                case (short)Keys.OemPeriod:
                    axWindowsMediaPlayer1.Ctlcontrols.fastForward();
                    break;
                case (short)Keys.S:
                    axWindowsMediaPlayer1.settings.rate =
                        axWindowsMediaPlayer1.settings.rate == 0.5 ? axWindowsMediaPlayer1.settings.rate = 1 : axWindowsMediaPlayer1.settings.rate = 0.5;
                    break;
                case (short)Keys.M:
                    axWindowsMediaPlayer1.settings.volume =
                        axWindowsMediaPlayer1.settings.volume == 0 ? 100 : 0;
                    break;
                case (short)Keys.Enter:
                    if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
                        axWindowsMediaPlayer1.Ctlcontrols.pause();
                    else
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                    break;
                case (short)Keys.OemQuestion:
                    MessageBox.Show("B - Back\r\nN - Next\r\n, - Rewind\r\n. - Fast Forward\r\nS - Slow Motion\r\nM - Mute\\Unmute\r\nEnter - Play\\Pause\r\n? - Help");
                    break;
                default:
                    this.Close();
                    break;
            }
        }

        void axWindowsMediaPlayer1_MouseMoveEvent(object sender, AxWMPLib._WMPOCXEvents_MouseMoveEvent e)
        {
            if (_initialized)
            {
                if (Math.Abs(e.fX - _lastPoint.fX) > 5 ||
                    Math.Abs(e.fY - _lastPoint.fY) > 5)
                {
                    this.Close();
                }
            }
            _lastPoint = e;
            _initialized = true;
        }

        private void ScreenSaver_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Show();
        }

        private List<FileInfo> GetVideos(string directory)
        {
            if (Directory.Exists(directory))
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                FileInfo[] fi = di.GetFiles("*.wmv");
                fi = fi.Union(di.GetFiles("*.mp4")).ToArray();
                List<FileInfo> result;
                if (fi != null && fi.Length > 0)
                    result = fi.ToList();
                else
                    result = new List<FileInfo>();
                return result;
            }
            else
            {
                return new List<FileInfo>();
            }
        }

        private void SetupPlaylistAndPlay()
        {
            axWindowsMediaPlayer1.settings.setMode("shuffle", true);
            if (videosToPlay.Count <= 0)
            {
                WMPLib.IWMPMedia m1 = axWindowsMediaPlayer1.newMedia(@"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv");
                axWindowsMediaPlayer1.currentPlaylist.appendItem(m1);
            }
            else
            {
                foreach (var file in videosToPlay.OrderBy(m => (new Random()).Next()))
                {
                    WMPLib.IWMPMedia m1 = axWindowsMediaPlayer1.newMedia(file.FullName);
                    axWindowsMediaPlayer1.currentPlaylist.appendItem(m1);
                }
            }
            axWindowsMediaPlayer1.Ctlcontrols.play();
            axWindowsMediaPlayer1.Ctlcontrols.next();//WMP won't shuffle the first video, so we'll skip that one.
        }
    }
}
