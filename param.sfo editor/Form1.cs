using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace param.sfo.editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

            ParamSfo paramSfo;
            using (var stream = File.Open(filenameBox.Text, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                paramSfo = ParamSfo.ReadFrom(stream);
            var title = paramSfo.Items.FirstOrDefault(i => i.Key == "TITLE");
            if (title == null)
            {
                MessageBox.Show("Title entry not found", "Unsupported param.sfo file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            titleBox.Text = title.StringValue;

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
    }
}
