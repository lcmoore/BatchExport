using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BatchTools.Models
{
    public class Course
    {
        public string? Name { get; set; }
        public string? PatientId { get; set; }
        public string? ExpandedDisplay { get; set; }

        public ObservableCollection<Plan>? Plans { get; set; }
        public string? ClinicalStatus { get; private set; }
        public string? Comment { get; private set; }
        public DateTime? CompletedDateTime { get; private set; }
        public string? Diagnoses { get; private set; }
        public DateTime HistoryDateTime { get; private set; }
        public string? HistoryDisplayName { get; private set; }
        public string? HistoryUserName { get; private set; }
        public string? Id { get; private set; }
        public string?   Intent { get; private set; }
        public DateTime? StartDateTime { get; private set; }
    }

}
