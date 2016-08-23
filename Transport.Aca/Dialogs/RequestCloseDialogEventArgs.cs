using System;

namespace Transport.Aca.Dialogs
{
    public class RequestCloseDialogEventArgs : EventArgs
    {
        public RequestCloseDialogEventArgs(bool dialogResult)
        {
            this.DialogResult = dialogResult;
        }

        public bool DialogResult { get; set; }
    }
}
