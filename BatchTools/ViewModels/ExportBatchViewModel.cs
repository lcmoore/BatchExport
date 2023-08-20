using BatchExportShell.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchTools.ViewModels
{
    public class ExportBatchViewModel : BindableBase
    {

        #region Declarations
        private IEventAggregator _eventAggregator;
        private IDialogService _dialogService;

        public DelegateCommand ExportLocalCommand { get; set; }
        public DelegateCommand ExportLocalCommandNoCT { get; set; }
        public DelegateCommand ExportMimCommand { get; set; }

        private bool _anonymizeCB;

        public bool AnonymizeCB
        {
            get { return _anonymizeCB; }
            set
            {
                _anonymizeCB = value;
                this.RaisePropertyChanged();
            }
        }


        private string _localAETitle = "Local AE Title";
        public string LocalAETitle
        {
            get { return this._localAETitle; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._localAETitle, value))
                {
                    this._localAETitle = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private string _localIP = "Local IP";
        public string LocalIP
        {
            get { return this._localIP; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._localIP, value))
                {
                    this._localIP = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private string _localPort = "Local Port";
        public string LocalPort
        {
            get { return this._localPort; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._localPort, value))
                {
                    this._localPort = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private string _varianAETitle = "Varian AE Title";
        public string VarianAETitle
        {
            get { return this._varianAETitle; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._varianAETitle, value))
                {
                    this._varianAETitle = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private string _varianIP = "Varian IP";
        public string VarianIP
        {
            get { return this._varianIP; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._varianIP, value))
                {
                    this._varianIP = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private string _varianPort = "Varian Port";
        public string VarianPort
        {
            get { return this._varianPort; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._varianPort, value))
                {
                    this._varianPort = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        #endregion


        #region Constructor
        public ExportBatchViewModel(IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            ExportLocalCommand = new DelegateCommand(ExportLocal);
            ExportLocalCommandNoCT = new DelegateCommand(ExportLocalNoCT);
            ExportMimCommand = new DelegateCommand(ExportMim);



        }
        #endregion

        #region Methods
        private void ExportMim()
        {
            string args = this._varianAETitle + "," + this._varianIP + "," + this._varianPort;

            _eventAggregator.GetEvent<ExportMimEvent>().Publish(args);
        }

        private void ExportLocal()
        {
            string args = this._localAETitle + "," + this._localIP + "," + this._localPort + ",true," + Convert.ToString(this.AnonymizeCB) + "," + this.VarianAETitle + "," +
                 this.VarianIP + "," + this.VarianPort;
            using (var fbd = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _eventAggregator.GetEvent<ExportLocalEvent>().Publish(fbd.SelectedPath + "|" + args);

                }
            }

        }
        private void ExportLocalNoCT()
        {
            string args = this._localAETitle + "," + this._localIP + "," + this._localPort + ",false," + Convert.ToString(this.AnonymizeCB) + "," + this.VarianAETitle + "," +
                 this.VarianIP + "," + this.VarianPort;
            using (var fbd = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _eventAggregator.GetEvent<ExportLocalEvent>().Publish(fbd.SelectedPath + "|" + args);

                }
            }
        }
        #endregion
    }
}
