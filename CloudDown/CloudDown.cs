using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CloudDown
{
    public partial class frmCloudDown : Form
    {

        private KeyModifiers shortcutKeyModi = KeyModifiers.Control | KeyModifiers.Alt | KeyModifiers.Shift;
        private Keys shortcutKey = Keys.X;

        public frmCloudDown()
        {
            InitializeComponent();            
        }
        private void frmCloudDown_Load(object sender, EventArgs e)
        {
            RegisterHotKey(this.Handle, HOTKEY_ID, shortcutKeyModi, shortcutKey);
            this.Visible = false;
            this.Hide();
        }
        private void frmCloudDown_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
        }
        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region user32.dll
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        private static IntPtr FindWindow(string titleName)
        {
            Process[] pros = Process.GetProcesses(".");
            foreach (Process p in pros)
                if (p.MainWindowTitle.ToUpper().Contains(titleName.ToUpper()))
                    return p.MainWindowHandle;
            return new IntPtr();
        }

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        #endregion  user32.dll

        const int HOTKEY_ID = 31197; //Any number to use to identify the hotkey instance

        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }

        private const int WM_HOTKEY = 0x0312;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case WM_HOTKEY:
                    Keys key = (Keys)(((int)message.LParam >> 16) & 0xFFFF);
                    KeyModifiers modifier = (KeyModifiers)((int)message.LParam & 0xFFFF);

                    if ( (shortcutKeyModi == modifier && shortcutKey == key))
                    {
                        //IntPtr hWnd = GetForegroundWindow();
                        //IntPtr hWnd = FindWindow("LGE_VPC - Desktop Viewer");
                        IntPtr hWnd = FindWindow(null, "LGE_VPC - Desktop Viewer");

                        if (hWnd != IntPtr.Zero)
                        {
                            //SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                            ShowWindowAsync(hWnd, SW_SHOWMINIMIZED);
                        }
                    }

                    break;
            }
            base.WndProc(ref message);
        }

        private void frmCloudDown_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
