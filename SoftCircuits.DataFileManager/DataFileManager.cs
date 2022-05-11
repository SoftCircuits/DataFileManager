// Copyright (c) 2022 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace SoftCircuits.DataFileManager
{
    /// <summary>
    /// Component that helps manage a data file.
    /// </summary>
    public partial class DataFileManager : Component
    {
        private const string DefaultFileName = null;
        private const bool DefaultIsModified = false;
        private const string DefaultDefaultExt = "dat";
        private const string DefaultFilter = "All Files (*.*)|*.*";
        private const string DefaultSaveFilePrompt = "File has been modified. Save changes?";
        private const string DefaultSaveFileTitle = "Save Changes";

        #region Static methods

        /// <summary>
        /// Returns true if <paramref name="path"/> is not null or empty.
        /// </summary>
        /// <param name="path">File path and name to test.</param>
        public static bool IsFileName(string path) => !string.IsNullOrWhiteSpace(path);

        /// <summary>
        /// Returns just the file name of the given path, or "Untitled" if the path is
        /// empty.
        /// </summary>
        /// <param name="path">File path and name to transform.</param>
        public static string GetFileTitle(string path) => IsFileName(path) ? Path.GetFileName(path) : "Untitled";

        #endregion

        #region Events

        [Description("Occurs when data should be cleared for a new file.")]
        public event EventHandler<DataFileEventArgs> NewFile;
        [Description("Occurs when a new file needs to be opened.")]
        public event EventHandler<DataFileEventArgs> OpenFile;
        [Description("Occurs when the file needs saving.")]
        public event EventHandler<DataFileEventArgs> SaveFile;
        [Description("Occurs whenever the current file name has changed.")]
        public event EventHandler<DataFileEventArgs> FileChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the default extension added to files when saving a file and
        /// no extension has been specified.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(DefaultDefaultExt)]
        [Description("The default extension added to saved files when no extension has been specified.")]
        public string DefaultExt { get; set; }

        /// <summary>
        /// Gets or sets the file filters used in Open and Save dialog boxes.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(DefaultFilter)]
        [Description("File filters used in Open and Save dialog boxes.")]
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the text displayed by PromptSaveIfModified() method.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(DefaultSaveFilePrompt)]
        [Description("Gets or sets the text displayed by PromptSaveIfModified() method.")]
        public string SaveFilePrompt { get; set; }

        /// <summary>
        /// Gets or sets the title displayed by PromptSaveIfModified() method.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(DefaultSaveFileTitle)]
        [Description("Gets or sets the title displayed by PromptSaveIfModified() method.")]
        public string SaveFileTitle { get; set; }

        /// <summary>
        /// Gets the full path of the current filename or <c>null</c> if the current file
        /// has no name.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(DefaultFileName)]
        public string FileName { get; private set; }

        /// <summary>
        /// Gets or sets whether the current file data has changed.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(DefaultIsModified)]
        public bool IsModified { get; set; }

        /// <summary>
        /// Indicates if data file currently has a filename.
        /// </summary>
        [Browsable(false)]
        public bool HasFileName => IsFileName(FileName);

        /// <summary>
        /// Returns the title of the current file (filename without path) or
        /// "Untitled" if the current file has no name.
        /// </summary>
        [Browsable(false)]
        public string FileTitle => GetFileTitle(FileName);

        #endregion

        #region Construction and initialization

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

        /// <summary>
        /// Common initialization.
        /// </summary>
        protected void Initialize()
        {
            FileName = DefaultFileName;
            IsModified = DefaultIsModified;
            DefaultExt = DefaultDefaultExt;
            Filter = DefaultFilter;
            SaveFilePrompt = DefaultSaveFilePrompt;
            SaveFileTitle = DefaultSaveFileTitle;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Clears the current file.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool New()
        {
            if (PromptSaveIfModified())
            {
                return OnNew();
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
                    return OnLoad(openFileDialog1.FileName);
            }
            return false;
        }

        /// <summary>
        /// Opens the specified filename. Does not prompt to save current file. Sets
        /// <paramref name="path"/> as the new current file name. Call this method
        /// to override the normal component logic.
        /// </summary>
        /// <param name="path">Path of file to open.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Open(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            return OnLoad(path);
        }

        /// <summary>
        /// Saves the current file. The Save As dialog box is used if the
        /// current file is not named.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Save()
        {
            return (HasFileName) ? OnSave(FileName) : SaveAs();
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
                return OnSave(saveFileDialog1.FileName);
            return false;
        }

        /// <summary>
        /// Saves the current file with the specified name. Sets <paramref name="path"/> as
        /// the new current file name. Call this method to override the normal component
        /// logic.
        /// </summary>
        /// <param name="path">Path to save the current file.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool SaveAs(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            return OnSave(path);
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
                DialogResult result = MessageBox.Show(SaveFilePrompt, SaveFileTitle,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    return Save();
                if (result == DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        #endregion

        #region Virtual methods

        /// <summary>
        /// Raises the <see cref="NewFile"/> event to implement creating a new file.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        protected virtual bool OnNew()
        {
            try
            {
                DataFileEventArgs e = new(null);
                NewFile?.Invoke(this, e);
                FileName = e.FileName;
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
        /// Raises the <see cref="OpenFile"/> event to implement opening a file.
        /// </summary>
        /// <param name="path">Full path name of file to load.</param>
        /// <returns>True if successful, false otherwise.</returns>
        protected virtual bool OnLoad(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            try
            {
                DataFileEventArgs e = new(path);
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
        /// Raises the <see cref="SaveFile"/> event to implement saving a file.
        /// </summary>
        /// <param name="path">Full path name to save to.</param>
        /// <returns>True if successful, false otherwise.</returns>
        protected virtual bool OnSave(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            try
            {
                DataFileEventArgs e = new(path);
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

        #endregion

    }

    /// <summary>
    /// Event args class.
    /// </summary>
    public class DataFileEventArgs : EventArgs
    {
        /// <summary>
        /// Returns the full path for the current file name, or <c>null</c>
        /// if the file is unnamed.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Returns just the name portion of the current file name, or "Untitled"
        /// if the file is unnamed.
        /// </summary>
        public string FileTitle => DataFileManager.GetFileTitle(FileName);

        /// <summary>
        /// Constructs a new <see cref="DataFileEventArgs"/> instance.
        /// </summary>
        /// <param name="fileName"></param>
        public DataFileEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}
