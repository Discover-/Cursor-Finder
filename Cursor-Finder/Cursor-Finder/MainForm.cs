using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cursor_Finder.Properties;

namespace Cursor_Finder
{
    public partial class MainForm : Form
    {
        private bool forcedExit = false;

        public MainForm()
        {
            InitializeComponent();

            listViewKeysPressed.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            notifyIcon.Icon = Icon;

            if (Settings.Default.SetCursorPos)
            {
                textBoxCoordinateX.Text = Settings.Default.CursorX.ToString();
                textBoxCoordinateY.Text = Settings.Default.CursorY.ToString();
                GoToNotifyIcon();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            KeyboardHook keyboardHook = new KeyboardHook();
            keyboardHook.KeyPressed += new EventHandler<KeyPressedEventArgs>(keyboardhook_KeyPressed);
            keyboardHook.RegisterHotKey(Cursor_Finder.ModifierKeys.Control, Keys.D1);
        }

        void keyboardhook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (!Settings.Default.SetCursorPos)
            {
                MessageBox.Show("No coordinates were set. Please fill out the textboxes.", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AddListViewItem(e.Modifier.ToString() + " + " + e.Key.ToString());

            if (e.Modifier.ToString() + " + " + e.Key.ToString() == "Control + D1")
                Cursor.Position = new Point(Settings.Default.CursorX, Settings.Default.CursorY);
        }

        private void AddListViewItem(string keyCode)
        {
            listViewKeysPressed.Items.Add(DateTime.Now.ToString()).SubItems.Add(keyCode);
            listViewKeysPressed.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewKeysPressed.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewKeysPressed.EnsureVisible(listViewKeysPressed.Items.Count - 1);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                GoToNotifyIcon();
        }

        private void GoToNotifyIcon()
        {
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Hide();
            notifyIcon.Visible = true;
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
            ShowInTaskbar = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (forcedExit)
                return;

            Hide();
            notifyIcon.Visible = true;
            e.Cancel = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            forcedExit = true;
            Application.Exit();
        }

        private void textBoxCoordinateX_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxCoordinateY_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxCoordinateX.Text) || String.IsNullOrWhiteSpace(textBoxCoordinateY.Text))
            {
                MessageBox.Show("One or both of the coordinate textbox values were left empty.", "Something went wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int coordinateX = 0, coordinateY = 0;

            if (!Int32.TryParse(textBoxCoordinateX.Text, out coordinateX))
            {
                MessageBox.Show("The X coordinate is invalid. Make sure it's an unsigned integer (> 0) and there are no characters in the textbox.", "Something went wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Int32.TryParse(textBoxCoordinateY.Text, out coordinateY))
            {
                MessageBox.Show("The Y coordinate is invalid. Make sure it's an unsigned integer (> 0) and there are no characters in the textbox.", "Something went wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Settings.Default.CursorX = coordinateX;
            Settings.Default.CursorY = coordinateY;
            Settings.Default.SetCursorPos = true;
            Settings.Default.Save();
        }
    }
}
