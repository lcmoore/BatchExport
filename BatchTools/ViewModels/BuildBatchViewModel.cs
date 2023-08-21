using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchExportShell.Core.Events;
using System.Windows.Controls;
using System.Windows.Input;
using BatchTools.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.IO;
using System.Text.Json;
using Microsoft.Windows.Themes;
using System.Windows;

namespace BatchTools.ViewModels
{
    public class BuildBatchViewModel : BindableBase
    {
        #region Declarations
        private ObservableCollection<object> _selectedItem = new ObservableCollection<object>();

        public ObservableCollection<object> SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public object _treeViewSelectionChanged { get; set; }
        private string _addItemText = "Loading Summaries...";

        public string AddItemText
        {
            get { return _addItemText; }
            set { SetProperty(ref _addItemText, value); }
        }

        private ObservableCollection<UISummary> _summaries;
        public ObservableCollection<UISummary> Summaries { get { return _summaries; } set { SetProperty(ref _summaries, value); } }


        // collections declaration
        private ObservableCollection<Patient> _currentResults = new ObservableCollection<Patient>();
        public ObservableCollection<Patient> CurrentResults
        {
            get { return _currentResults; }
            set { SetProperty(ref _currentResults, value); }
        }

        private ObservableCollection<Patient> _tmpCurrentResults = new ObservableCollection<Patient>();

        public ObservableCollection<Patient> TmpCurrentResults
        {
            get { return _tmpCurrentResults; }
            set { SetProperty(ref _tmpCurrentResults, value); }
        }


        private readonly IDialogService _dialogService;

        private bool _summariesLoaded = false;

        public bool SummariesLoaded
        {
            get { return _summariesLoaded; }
            set
            {
                SetProperty(ref _summariesLoaded, value);
                //AddQueryItemCommand.RaiseCanExecuteChanged();
                //_eventAggregator.GetEvent<PublishSummaries>().Publish(Summaries);
                //_eventAggregator.GetEvent<CloseLoadingDialog>().Publish("Close");

            }
        }

        //private ObservableCollection<string> _currentText;
        private IEventAggregator _eventAggregator;

        //public ObservableCollection<string> CurrentText
        //{
        //    get { return _currentText; }
        //    set { SetProperty(ref _currentText, value); }
        //}

        #endregion
        #region Command Declarations

        public DelegateCommand ImportQueryCommand { get; private set; }
        public DelegateCommand SendQueryCommand { get; private set; }
        public DelegateCommand EclipseQueryCommand { get; private set; }
        public DelegateCommand AddQueryItemCommand { get; private set; }
        public DelegateCommand GetPatientsCommand { get; private set; }
        public DelegateCommand<object> TextInputChanged { get; private set; }
        public DelegateCommand<object> TreeViewSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> StackPanelClickCommand { get; private set; }
        public DelegateCommand RemovePatientCommand { get; private set; }
        public DelegateCommand ResetCurrentResultsCommand { get; private set; }
        public DelegateCommand AddSelectionToBatchCommand { get; private set; }
        #endregion
        #region Constructor

        public BuildBatchViewModel(IDialogService dialogService, IEventAggregator eventAggregator)
        {
            ImportQueryCommand = new DelegateCommand(ImportQuery).ObservesCanExecute(() => SummariesLoaded);
            AddQueryItemCommand = new DelegateCommand(AddQueryItem, CanAddQueryItem);

            StackPanelClickCommand = new DelegateCommand<object>(StackPanelClick);

            ResetCurrentResultsCommand = new DelegateCommand(ResetCurrentResults);
            AddSelectionToBatchCommand = new DelegateCommand(AddSelectionToBatch);

            _eventAggregator = eventAggregator;

            _dialogService = dialogService;


            LoadSummaries();

        }
        #endregion
        #region Methods

        private void AddSelectionToBatch()
        {

            _eventAggregator.GetEvent<AddBatchItem>().Publish(SelectedItem.FirstOrDefault());


        }

        private void ResetCurrentResults()
        {
            CurrentResults.Clear();
        }



        private void StackPanelClick(object obj)
        {

            var args = (MouseButtonEventArgs)obj;
            var tv = (TreeView)args.Source;
            SelectedItem.Clear();
            SelectedItem.Add(tv.SelectedItem);


        }




        private bool CanAddQueryItem()
        {
            if (SummariesLoaded)
            {
                AddItemText = "Add Item";
            }
            return SummariesLoaded;
        }


        private async void LoadSummaries()
        {

            //await AppComThread.Instance.InvokeAsync(async () =>
            //{

            //    Summaries = new ObservableCollection<UISummary>(await AppComThread.Instance.GetValueAsync(sac =>
            //    {
            //        return sac.Application.PatientSummaries.Select(p => new UISummary()
            //        {
            //            Id = p.Id,
            //            FirstName = p.FirstName,
            //            LastName = p.LastName,


            //        }).ToList();
            //    }));
            //    SummariesLoaded = true;



            //});
            Summaries = new ObservableCollection<UISummary>
            {
                new UISummary() { FirstName = "Test", LastName = "Test", Id = "12345678" }
            };
            SummariesLoaded = true;




        }

        private void AddQueryItem()
        {
            var p = new DialogParameters();
            p.Add("Summaries", Summaries.ToList());
            // _dialogService.Show(~) would allow users to continue to interact with the base view 
            // while dialog is open
            _dialogService.Show("AddQueryItemDialog", p, result =>
            {//callback
                if (result.Result == ButtonResult.OK)
                {

                }



            });

        }



        private void ImportQuery()
        {
            //// create a test patient
            //Patient testPatient = new Patient("12345678");
            //testPatient.Courses.Add(new Course("12345678", "Test Course"));
            //List<Patient> pat_list = new List<Patient>();
            //pat_list.Add(testPatient);
            //string json_test = JsonSerializer.Serialize(pat_list);
            //File.WriteAllText(@"./test.json", json_test);

            //// read in the json file and create a list of patients
            //string json_input = File.ReadAllText(@"./output.json");
            //List<Patient> patients = JsonSerializer.Deserialize<List<Patient>>(json_input)!;





            //List<string> iDs = new List<string>();
            //List<string> Courses = new List<string>();
            //List<string> Plans = new List<string>();
            // create request dictionary, mapping Ids, top a dictionary of courses, mapping course names to a list of plans

            // if the output file already exists, delete it
            if (File.Exists(@"PythonScripts/tmp/output.json"))
            {
                File.Delete(@"PythonScripts/tmp/output.json");
            }
            Dictionary<string, Dictionary<string, List<string>>> request = new Dictionary<string, Dictionary<string, List<string>>>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {

                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    foreach (string line in lines)
                    {
                        string course = null;
                        string plan = null;
           

                        string[] columns = line.Split(',');
                

                        string id = columns.ElementAt(0);
                        // add leading 0 if necessary, since csv removes leading 0s
                        if (columns.ElementAt(0).Length == 7)
                        {
                            id = "0" + columns.ElementAt(0);
                        }
                        // check if columns length is > 1
                        if (columns.Length > 1)
                        {
                            course = columns.ElementAt(1);
                        }
    
                        // check if columns length is > 2
                        if (columns.Length > 2)
                        {
                            plan = columns.ElementAt(2);
                        }
                       // if id not in requests keys, create a new object
                       if (!request.ContainsKey(id))
                        {
                            request.Add(id, new Dictionary<string, List<string>>());
                        }
                        // if course not in requests[id] keys, create a new object
                        if (course != null && !request[id].ContainsKey(course))
                        {
                            request[id].Add(course, new List<string>());
                        }
                        // if plan not in requests[id][course] keys, add it
                        if (course != null && plan != null && !request[id][course].Contains(plan))
                        {
                            request[id][course].Add(plan);
                        }   
  


                   

                       
    
                   

                    }
                    List<Patient> patient_requests = new List<Patient>();
                    foreach (KeyValuePair<string, Dictionary<string, List<string>>> entry in request)
                    {
                        Patient thisPatient = new Patient(entry.Key, entry.Value);
                        patient_requests.Add(thisPatient);


                        
                    }
                    // convert request dictionary to json
                    string json = JsonSerializer.Serialize(patient_requests);
                    // write json to file
                    File.WriteAllText(@"PythonScripts/tmp/request.json", json);
                    // run python script
                    System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
                    //python interprater location
                    start.FileName = "PythonScripts/venv/Scripts/python.exe";
                    //argument with file name and input parameters
                    start.Arguments = string.Format("{0}", "PythonScripts/GetPatients.py");
                    start.UseShellExecute = false;// Do not use OS shell
                    start.CreateNoWindow = true; // We don't need new window
                    start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
                    start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
                    start.LoadUserProfile = true;
                    using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                            string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                            

                        }
                    }

                    // read in the json file and create a list of patients
                    string json_input = File.ReadAllText(@"PythonScripts/tmp/output.json");
                    List<Patient> patients = JsonSerializer.Deserialize<List<Patient>>(json_input)!;
                    // replace the CurrentResults with the new results
                    CurrentResults = new ObservableCollection<Patient>(patients);



                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }



            }





            

        }
        #endregion

    }
}
