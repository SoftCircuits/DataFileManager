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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region File Commands

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFileManager1.New();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFileManager1.Open();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFileManager1.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataFileManager1.SaveAs();
        }

        #endregion

        #region File Handlers

        private void dataFileManager1_NewFile(object sender, DataFileEventArgs e)
        {
            textBox1.Text = string.Empty;
        }

        private void dataFileManager1_OpenFile(object sender, DataFileEventArgs e)
        {
            textBox1.Text = File.ReadAllText(e.FileName);
        }

        private void dataFileManager1_SaveFile(object sender, DataFileEventArgs e)
        {
            File.WriteAllText(e.FileName, textBox1.Text);
        }

        #endregion

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dataFileManager1.IsModified = true;
        }

        private void dataFileManager1_FileChanged(object sender, DataFileEventArgs e)
        {
            Text = $"{e.FileTitle} - Test App";
        }
    }
}
