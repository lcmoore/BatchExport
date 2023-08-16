using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BatchTools.Models
{
    public class Patient
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Display { get; set; }
        public string? Sex { get; set; }
        public string? Comment { get; private set; }
        public DateTime? CreationDateTime { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public DateTime HistoryDateTime { get; private set; }
        public string? HistoryUserDisplayName { get; private set; }
        public string? HistoryUserName { get; private set; }
        public string? PrimaryOncologistId { get; private set; }
        public string? ExpandedDisplay { get; set; }
        public ObservableCollection<Course>? Courses { get; set; }
    }
}
