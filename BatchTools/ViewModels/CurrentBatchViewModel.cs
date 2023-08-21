using BatchExportShell.Core.Events;
using BatchTools.Events;
using BatchTools.Models;
using Microsoft.VisualBasic.FileIO;
//using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace BatchTools.ViewModels
{
    public class CurrentBatchViewModel : BindableBase
    {

        #region Commands
        public DelegateCommand AddPlanCommand { get; private set; }
        public DelegateCommand<object> PlanListViewItemActivateCommand { get; private set; }
        public DelegateCommand RemovePlansCommand { get; private set; }
        public DelegateCommand SaveBatchCommand { get; private set; }
        public DelegateCommand LoadBatchCommand { get; private set; }
        public DelegateCommand ClearCurrentBatchCommand { get; private set; }
        #endregion

        #region Interfaces
        private IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        #endregion

        #region Collections
        private ObservableCollection<UISummary> _summaries;
        public ObservableCollection<UISummary> Summaries { get { return _summaries; } set { SetProperty(ref _summaries, value); } }
        private ObservableCollection<Plan> _currentBatch = new ObservableCollection<Plan>();
        public ObservableCollection<Plan> CurrentBatch
        {
            get { return _currentBatch; }
            set { SetProperty(ref _currentBatch, value); }
        }
        private ObservableCollection<Plan> _selectedPlanItems = new ObservableCollection<Plan>();
        public ObservableCollection<Plan> SelectedPlanItems
        {
            get { return _selectedPlanItems; }
            set { SetProperty(ref _selectedPlanItems, value); }

        }
        #endregion

        #region Constructor
        public CurrentBatchViewModel(IDialogService dialogService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _eventAggregator.GetEvent<AddBatchItem>().Subscribe(AddItem);
            //_eventAggregator.GetEvent<PublishSummaries>().Subscribe(ReceiveSummaries);
            _eventAggregator.GetEvent<AddItemEvent>().Subscribe(ParsePlanString);
            _eventAggregator.GetEvent<ExportLocalEvent>().Subscribe(ExportLocal);
            _eventAggregator.GetEvent<ExportMimEvent>().Subscribe(ExportMim);
            AddPlanCommand = new DelegateCommand(AddPlan);
            PlanListViewItemActivateCommand = new DelegateCommand<object>(PlanListViewItemActivate);
            RemovePlansCommand = new DelegateCommand(RemovePlans);
            SaveBatchCommand = new DelegateCommand(SaveBatch);
            LoadBatchCommand = new DelegateCommand(LoadBatch);
            ClearCurrentBatchCommand = new DelegateCommand(ClearCurrentBatch);
            ReceiveSummaries();

        }


        #endregion

        #region Data Methods
        public void PayloadBuilderJSON(string dir)
        {
            List<string> output_dir = new List<string>();
            foreach (string substring in dir.Split('|')[0].Split('\\'))
            {
                output_dir.Add(substring);
            }

            List<string> args = new List<string>();
            foreach (var arg in dir.Split('|')[1].Split(','))
            {
                args.Add(arg);

            }
            Dictionary<string, List<List<string>>> myDict = new Dictionary<string, List<List<string>>>();


            return;
        }
        private string PayloadBuilder(string dir)
        {


            List<string> file_dir = new List<string>();
            var splitdir = dir.Split('|');
            string[] arr = splitdir[0].Split('\\');
            var args = splitdir[1].Split(',').ToList();
            foreach (string element in arr)
            {
                file_dir.Add(element);
            }
            Dictionary<string, List<List<string>>> payload = new Dictionary<string, List<List<string>>>();
            List<string> listFileDir = new List<string>(file_dir);
            List<List<string>> listlistFileDir = new List<List<string>>();
            listlistFileDir.Add(listFileDir);


            payload.Add("directory", listlistFileDir);

            List<string> listArgs = new List<string>(args);
            List<List<string>> listlistArgs = new List<List<string>>();
            listlistArgs.Add(listArgs);
            payload.Add("args", listlistArgs);


            foreach (Plan plan in CurrentBatch)
            {
                if (!payload.ContainsKey(plan.PatientID))
                {
                    payload[plan.PatientID] = new List<List<string>>();

                }
                List<string> tmpList = new List<string>();
                tmpList.Add(plan.Course.Replace(" ", "($)"));
                tmpList.Add(plan.Name.Replace(" ", "($)"));
                tmpList.Add(plan.CTSeriesUID);
                tmpList.Add(plan.DoseUID);
                tmpList.Add(plan.StructureSetUID);
                tmpList.Add(plan.UID);
                //payload[plan.PatientID].Add(plan.Course);
                //payload[plan.PatientID].Add(plan.Name);
                //payload[plan.PatientID].Add(plan.CTSeriesUID);
                //payload[plan.PatientID].Add(plan.DoseUID);
                //payload[plan.PatientID].Add(plan.StructureSetUID);
                //payload[plan.PatientID].Add(plan.UID);
                payload[plan.PatientID].Add(tmpList);


            }
            string json = JsonConvert.SerializeObject(payload);
            var thisdir = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\myFile.json";



            File.WriteAllText(thisdir, json);
            return thisdir;


            //var payloadstr = "{";
            //foreach (KeyValuePair<string, List<List<string>>> entry in payload)
            //{
            //    if (!payloadstr.Equals("{"))
            //    {
            //        payloadstr += "|";

            //    }
            //    payloadstr += entry.Key + ":"; // key == patientId
            //    payloadstr += "[";
            //    foreach (var element in entry.Value) //entry.Value == List<List<string>> -> element = List<string>(course,plan,data1,data2...)
            //    {
            //        payloadstr += "[";
            //        foreach (string item in element) //item == string
            //        {
            //            if (item != element.Last())
            //            {
            //                payloadstr += item + ",";

            //            }
            //            else
            //            {
            //                payloadstr += item + "]";
            //            }

            //        }
            //        if (element != entry.Value.Last())
            //        {
            //            payloadstr += ",";
            //        }
            //        else
            //        {
            //            //payloadstr += element;
            //            payloadstr += "]";

            //        }

            //    }


            //}


            //payloadstr += "}";
            //return payloadstr;

        }

        private string MimPayloadBuilder(string[] args)
        {

            Dictionary<string, List<string>> payload = new Dictionary<string, List<string>>();
            payload["args"] = new List<string>();
            foreach (string arg in args)
            {
                payload["args"].Add(arg);
            }
            foreach (Plan plan in CurrentBatch)
            {
                if (!payload.ContainsKey(plan.PatientID))
                {
                    payload[plan.PatientID] = new List<string>();

                }

                payload[plan.PatientID].Add(plan.CTSeriesUID);
                payload[plan.PatientID].Add(plan.DoseUID);
                payload[plan.PatientID].Add(plan.StructureSetUID);
                payload[plan.PatientID].Add(plan.UID);


            }

            var payloadstr = "{";
            foreach (KeyValuePair<string, List<string>> entry in payload)
            {
                if (!payloadstr.Equals("{"))
                {
                    payloadstr += "|";

                }
                payloadstr += entry.Key + ":";
                payloadstr += "[";
                foreach (string element in entry.Value)
                {
                    if (element != entry.Value.Last())
                    {
                        payloadstr += element + ",";
                    }
                    else
                    {
                        payloadstr += element;
                        payloadstr += "]";

                    }

                }


            }


            payloadstr += "}";
            return payloadstr;

        }
        #endregion

        #region Command Methods
        private void ExportMim(Object args)
        {
            string argstring = (String)args;
            string[] arguments = argstring.Split(',');
            string payload = MimPayloadBuilder(arguments);
            var thisdir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            var process = Process.Start(thisdir + "\\MimMove\\MimBatchExport.exe", payload);

        }
        private void ExportLocal(Object args)
        {
            string dir = (String)args;
            //PayloadBuilderJSON(dir);
            string payload = PayloadBuilder(dir);

            var thisdir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            var process = Process.Start(thisdir + "\\LocalMove\\EvilDICOMMoveTest.exe", payload);


        }
        private void ClearCurrentBatch()
        {
            CurrentBatch.Clear();
        }
        private void LoadBatch()
        {
            CurrentBatch.Clear();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
                //openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    string json = File.ReadAllText(filePath);
                    var CurrentBatchList = JsonConvert.DeserializeObject<List<Plan>>(json);
                    foreach (Plan plan in CurrentBatchList)
                    {
                        CurrentBatch.Add(plan);
                    }
           
                }

            }


      

        }
        private void SaveBatch()
        {
            // firtst, a save file dialog to find the location to save the file
            SaveFileDialog sd = new SaveFileDialog();
            // save type will be .json, so the filter is set to that
            sd.Filter = "Json files (*.json)|*.json|Text files (*.txt)|*.txt";
            sd.DefaultExt = "json";

            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var current_batch_json = JsonConvert.SerializeObject(CurrentBatch);
                File.WriteAllText(sd.FileName, current_batch_json);
            }



        }
        private void RemovePlans()
        {
            var selectedPlan = _selectedPlanItems.FirstOrDefault();
            CurrentBatch.Remove(selectedPlan);
            _selectedPlanItems.Clear();



        }

        private void PlanListViewItemActivate(object obj)
        {
            SelectedPlanItems.Clear();
            var args = (SelectionChangedEventArgs)obj;
            var source = (System.Windows.Controls.ListBox)args.Source;
            SelectedPlanItems.Add((Plan)source.SelectedItem);

        }
        private void ParsePlanString(string obj)
        {

        }
        //private void ParsePlanString(string obj)
        //{
        //    // check cache here
        //    var arguments = obj.SplitOnWhiteSpace();
        //    var patientID = ArgumentParser.GetPatientId(arguments);
        //    var course = ArgumentParser.GetCourseId(arguments);
        //    var plan = ArgumentParser.GetPlanSetup(arguments);
        //    var tmpPat = new Patient(patientID);
        //    foreach (var patCourse in tmpPat.Courses)
        //    {
        //        if (patCourse.Name.Equals(course))
        //        {
        //            foreach (var patPlan in patCourse.Plans)
        //            {
        //                // TODO: fix
        //                if (patPlan.UID.Equals(plan))
        //                {
        //                    AddItem(patPlan);
        //                    break;

        //                }

        //            }
        //        }
        //    }
        //}

        private void ReceiveSummaries()
        {
            Summaries = new ObservableCollection<UISummary>
            {
                new UISummary() { FirstName = "Test", LastName = "Test", Id = "12345678" }
            };
         


        }

        private void AddPlan()
        {
            var p = new DialogParameters();
            var sendSummaries = Summaries.ToList();
            p.Add("Summaries", sendSummaries);

            _dialogService.Show("AddQueryItemDialog", p, result =>
            {//callback
                if (result.Result == ButtonResult.OK)
                {

                }



            });


        }

        private void AddItem(object obj)
        {
            if (obj is null) { return; };


            if (obj.GetType() == typeof(Plan))
            {
                Plan plan = (Plan)obj;
                CurrentBatch.Add(plan);
            }
            else if (obj.GetType() == typeof(Course))
            {
                Course course = (Course)obj;
                foreach (Plan plan in course.Plans)
                {
                    CurrentBatch.Add(plan);
                }
            }
            else if (obj.GetType() == typeof(Patient))
            {
                Patient patient = (Patient)obj;
                foreach (Course course in patient.Courses)
                {
                    foreach (Plan plan in course.Plans)
                    {
                        CurrentBatch.Add(plan);
                    }
                }
            }




        }

        public ObservableCollection<Plan> GetBatch()
        {
            return CurrentBatch;
        }

        public void ClearBatch()
        {
            CurrentBatch.Clear();
        }

        public void DeleteBatchItem(string item_string)
        {
            foreach (var plan in CurrentBatch.Where(p => p.Name == item_string))
            {
                CurrentBatch.Remove(plan);
            }
        }
        #endregion


    }
}
