using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace param.sfo.editor
{
    public partial class Form1 : Form
    {
        private ParamSfo paramSfo;
        private ParamSfoEntry titleEntry;

        public Form1()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.O):
                    browseButton_Click(null, null);
                    return true;
                case (Keys.Control | Keys.S):
                    saveButton_Click(null, null);
                    return true;
                case (Keys.Control | Keys.R):
                    reloadButton_Click(null, null);
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
       
        private void reloadButton_Click(object sender, EventArgs e)
        {
            saveButton.Enabled = false;
            titleBox.TextChanged -= titleBox_TextChanged;

            if (string.IsNullOrEmpty(filenameBox.Text))
                return;

            if (!File.Exists(filenameBox.Text))
            {
                MessageBox.Show("Please check the path to param.sfo", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var stream = File.Open(filenameBox.Text, FileMode.Open, FileAccess.Read, FileShare.Read))
                paramSfo = ParamSfo.ReadFrom(stream);
            titleEntry = paramSfo.Items.FirstOrDefault(i => i.Key == "TITLE");
            if (titleEntry == null)
            {
                paramSfo = null;
                MessageBox.Show("Title entry not found", "Unsupported param.sfo file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            titleBox.Text = titleEntry.StringValue;

            titleBox.TextChanged += titleBox_TextChanged;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filenameBox.Text = openFileDialog1.FileName;
                reloadButton_Click(sender, e);
            }
        }

        private void titleBox_TextChanged(object sender, EventArgs e)
        {
            saveButton.Enabled = File.Exists(filenameBox.Text);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!saveButton.Enabled)
                return;

            var filename = filenameBox.Text;
            titleEntry.StringValue = titleBox.Text;

            var bakFilename = filename + ".bak";
            if (!File.Exists(bakFilename))
                File.Copy(filename, bakFilename);

            using (var stream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
                paramSfo.WriteTo(stream);

            saveButton.Enabled = false;
        }

        private void filenameBox_TextChanged(object sender, EventArgs e)
        {
            saveButton.Enabled = false;
            paramSfo = null;
            titleEntry = null;
        }
    }
}
