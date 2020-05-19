using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace MapToGrid.Utility
{
    public class DataFileEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public string FileTitle => DataFileManager.GetFileTitle(FileName);

        public DataFileEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }

    /// <summary>
    /// Component that helps manage a data file.
    /// </summary>
    public partial class DataFileManager : Component
    {
        private const string DefaultDefaultExt = "dat";
        private const string DefaultFilter = "All Files (*.*)|*.*";

        public static bool IsFileName(string path) => !string.IsNullOrWhiteSpace(path);
        public static string GetFileTitle(string path) => IsFileName(path) ? Path.GetFileName(path) : "Untitled";

        // Event provides notification the current file has changed
        public event EventHandler<DataFileEventArgs> NewFile;
        public event EventHandler<DataFileEventArgs> OpenFile;
        public event EventHandler<DataFileEventArgs> SaveFile;
        public event EventHandler<DataFileEventArgs> FileChanged;

        /// <summary>
        /// Returns the full path of the current filename or <c>null</c> if
        /// the current file has no name.
        /// </summary>
        [Browsable(false)]
        public string FileName { get; set; }

        /// <summary>
        /// Determines if the current file data has changed.
        /// </summary>
        [Browsable(false)]
        public bool IsModified { get; set; }

        /// <summary>
        /// The default extension added to saved files when no
        /// extension has been specified.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(DefaultDefaultExt)]
        [Description("Default extension when saving files.")]
        public string DefaultExt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        [DefaultValue(DefaultFilter)]
        [Description("File filters used in Open and Save dialog boxes.")]
        public string Filter { get; set; }

        /// <summary>
        /// Indicates if data file currently has a filename
        /// </summary>
        [Browsable(false)]
        public bool HasFileName => IsFileName(FileName);

        /// <summary>
        /// Returns the title of the current file (filename without path) or
        /// "Untitled" if the current file has no name
        /// </summary>
        [Browsable(false)]
        public string FileTitle => GetFileTitle(FileName);


        /// <summary>
        /// Constructs a new <see cref="DataFileManager"/> instance.
        /// </summary>
        public DataFileManager()
        {
            InitializeComponent();
            Initialize();
        }

        /// <summary>
        /// Constructs a new <see cref="DataFileManager"/> instance.
        /// </summary>
        public DataFileManager(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            Initialize();
        }

        //
        protected void Initialize()
        {
            FileName = null;
            IsModified = false;
            DefaultExt = DefaultDefaultExt;
            Filter = DefaultFilter;
        }

        /// <summary>
        /// Clears the current file.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool New()
        {
            if (PromptSaveIfModified())
            {
                return OnNew(new DataFileEventArgs(null));
            }
            return false;
        }

        /// <summary>
        /// Opens a new file.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Open()
        {
            if (PromptSaveIfModified())
            {
                openFileDialog1.DefaultExt = DefaultExt;
                openFileDialog1.Filter = Filter;
                openFileDialog1.FileName = string.Empty;
                openFileDialog1.CheckFileExists = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    return OnLoad(new DataFileEventArgs(openFileDialog1.FileName));
            }
            return false;
        }

        /// <summary>
        /// Opens the specified filename. Does not prompt to save current file.
        /// </summary>
        /// <param name="path">Path of file to open.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Open(string path)
        {
            return OnLoad(new DataFileEventArgs(path));
        }

        /// <summary>
        /// Saves the current file. The Save As dialog box is used if the
        /// current file is not named.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Save()
        {
            return (HasFileName) ? OnSave(new DataFileEventArgs(FileName)) : SaveAs();
        }

        /// <summary>
        /// Saves the current file with a new name.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool SaveAs()
        {
            saveFileDialog1.DefaultExt = DefaultExt;
            saveFileDialog1.Filter = Filter;
            saveFileDialog1.FileName = FileName;
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                return OnSave(new DataFileEventArgs(saveFileDialog1.FileName));
            return false;
        }

        /// <summary>
        /// Prompts to saves the current file if it has been modified.
        /// </summary>
        /// <returns>True if the file was not modified, the user did not want to save the changes, or
        /// if the file was successfully saved. Otherwise, returns false.</returns>
        public bool PromptSaveIfModified()
        {
            if (IsModified)
            {
                DialogResult result = MessageBox.Show("File has been modified. Save changes?", "Save Changes",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    return Save();
                if (result == DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual bool OnNew(DataFileEventArgs e)
        {
            try
            {
                NewFile?.Invoke(this, e);
                FileName = null;
                IsModified = false;
                FileChanged?.Invoke(this, e);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating new file : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual bool OnLoad(DataFileEventArgs e)
        {
            try
            {
                OpenFile?.Invoke(this, e);
                FileName = e.FileName;
                IsModified = false;
                FileChanged?.Invoke(this, e);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading '{FileName}' : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual bool OnSave(DataFileEventArgs e)
        {
            try
            {
                SaveFile?.Invoke(this, e);
                FileName = e.FileName;
                IsModified = false;
                FileChanged?.Invoke(this, e);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving '{FileName}' : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }
    }
}
