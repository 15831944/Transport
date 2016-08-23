using System;

namespace Transport.Aca.Dialogs
{
    public interface IDialogVewModel
    {
        event EventHandler<RequestCloseDialogEventArgs> RequestCloseDialog;
    }
}
