namespace Transport.Aca.Dialogs
{
    public class WindowDialogService : IWindowDialogService
    {
        public bool? ShowDialog(string title, object datacontext)
        {
            var dlg = new DialogView {DataContext = datacontext};

            return dlg.ShowDialog();
        }
    }
}
