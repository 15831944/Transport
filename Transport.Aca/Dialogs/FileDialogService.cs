using System;
using Microsoft.Win32;

namespace Transport.Aca.Dialogs
{
    public class FileDialogService : IFileDialogService
    {
        public string OpenFileDialog()
        {
            var dlg = new OpenFileDialog();
            var result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
                return dlg.FileName;
            return String.Empty;
        }
    }
}
