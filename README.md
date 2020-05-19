# DataFileManager

[![NuGet version (SoftCircuits.DataFileManager)](https://img.shields.io/nuget/v/SoftCircuits.DataFileManager.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.DataFileManager/)

## Introduction

DataFileManager is a WinForms component that helps manage an application's data files.

This component assists with tracking whether or not the current file has been saved, what the current file's name is, whether or not changes have been made, and helps ensure the user doesn't exit or load a file without having a chance to save the current file.

## Using the Library

Start by adding the component to your form.

#### Implementing File Commands

To implement the file commands, direct your file menu commands to the component.

```cs
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
```

#### Implement Application-Specific File Handling

Next, you need to add event handlers to perform your application-specific file reading and writing. Note that you don't need to worry about File Open or File Save dialogs, and you also don't need to implement exception handling. These are all done for you by the library.

```cs
private void dataFileManager1_NewFile(object sender, MapToGrid.Utility.DataFileEventArgs e)
{
    textBox1.Text = string.Empty;
}

private void dataFileManager1_OpenFile(object sender, MapToGrid.Utility.DataFileEventArgs e)
{
    textBox1.Text = File.ReadAllText(e.FileName);
}

private void dataFileManager1_SaveFile(object sender, MapToGrid.Utility.DataFileEventArgs e)
{
    File.WriteAllText(e.FileName, textBox1.Text);
}
```

#### Tracking Modifications

An important element in ensuring the user always gets a chance to save any changes made is to ensure the current file is marked as modified whenever a change is made. In our example, all the data is in a `TextBox` control, and so we add a handler for the `TextChanged` event. You would need to ensure you set the `IsModified` flag in response to any changes made.

```cs
private void textBox1_TextChanged(object sender, EventArgs e)
{
    dataFileManager1.IsModified = true;
}
```

Now when you call the `New()` or `Open()` methods, the library will automatically prompt to save the current document if it has been modified. If the user cancels that prompt, or if there is an error saving the file, the `New()` and `Open()` methods will abort to avoid losing changes to the current document.

One case you also need to handle is when the user closes your form. In this case, you can call the `PromptSaveIfModified()` method directly. This method returns `true` if the current file has not been modified, if the user chose not to save the changes, or if the changes were successfully saved. Otherwise, this method returns false. In the example below, the code cancels the form closing if `PromptSaveIfModified()` returns false.

```cs
private void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    if (!dataFileManager1.PromptSaveIfModified())
        e.Cancel = true;
}
```

#### Displaying the Current File

Most applications set the title bar text to something like *Current Document - AppName*. The DataFileManager library makes it easy to ensure your title bar is always current by providing the `FileChanged` event.

The `DataFileEventArgs` class includes the `FileName` property, which is the full file name of the current file or `null` if the file has no name. The `FileTitle` property returns just the name portion of `FileName` or `"Unititled"` if the file has no name.

```cs
private void dataFileManager1_FileChanged(object sender, MapToGrid.Utility.DataFileEventArgs e)
{
    Text = $"{e.FileTitle} - Test App";
}
```

#### Customizing the File Open and File Save Dialog Boxes

Finally, there are a couple of component properties you can set in the designer. The `DefaultExt` and `Filter` properties correspond to the `DefaultExt` and `Filter` properties of the `OpenFileDialog` and `SaveFileDialog` components. These properties determine the default file extension added when saving a file and no extension was specified, and control the choices that appear in the *Files of type* or *Save as file type* box in the dialog box.
