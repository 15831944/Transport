using System;
using System.Windows;

namespace Transport.Aca.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogView.xaml
    /// </summary>
    public partial class DialogView : Window
    {
        private bool _isClosed;

        public DialogView()
        {
            InitializeComponent();
            DialogPresenter.DataContextChanged += DialogPresenterDataContextChanged;
            Closed += DialogWindowClosed;
        }

        private void DialogWindowClosed(object sender, EventArgs e)
        {
            _isClosed = true;
        }

        private void DialogPresenterDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dlg = e.NewValue as IDialogVewModel;
            if (dlg == null) return;

            dlg.RequestCloseDialog += new EventHandler<RequestCloseDialogEventArgs>(DialogResultTrueEvent).MakeWeak(eh => dlg.RequestCloseDialog -= eh);
        }

        private void DialogResultTrueEvent(object sender, RequestCloseDialogEventArgs eventArgs)
        {
            if (_isClosed) return;

            DialogResult = eventArgs.DialogResult;
        }
    }
}
