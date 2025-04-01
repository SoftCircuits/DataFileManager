// Copyright (c) 2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using SoftCircuits.DataFileManager;
using System;
using System.IO;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataFileManager1.New();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!dataFileManager1.PromptSaveIfModified())
                e.Cancel = true;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region File Commands

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFileManager1.New();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFileManager1.Open();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFileManager1.Save();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFileManager1.SaveAs();
        }

        #endregion

        #region File Handlers

        private void DataFileManager1_NewFile(object sender, DataFileEventArgs e)
        {
            textBox1.Text = string.Empty;
        }

        private void DataFileManager1_OpenFile(object sender, DataFileEventArgs e)
        {
            textBox1.Text = File.ReadAllText(e.FileName);
        }

        private void DataFileManager1_SaveFile(object sender, DataFileEventArgs e)
        {
            File.WriteAllText(e.FileName, textBox1.Text);
        }

        #endregion

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            dataFileManager1.IsModified = true;
        }

        private void DataFileManager1_FileChanged(object sender, DataFileEventArgs e)
        {
            Text = $"{e.FileTitle} - Test App";
        }
    }
}
