namespace Transport.Aca.Dialogs
{
    public interface IWindowDialogService
    {
        bool? ShowDialog(string title, object datacontext);
    }
}
