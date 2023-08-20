using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchTools.Models
{
    public class Plan
    {
        public string? UID { get; set; }
        public string? PatientID { get; set; }
        public string? ApprovalStatus { get; private set; }
        public string? Comment { get; private set; }
        public string? CreationDateTime { get; private set; }
        public string? CreationUserName { get; private set; }
        public string? HistoryDateTime { get; private set; }
        public string? HistoryUserDisplayName { get; private set; }
        public string? HistoryUserName { get; private set; }
        public string? PlanIntent { get; private set; }
        public string? PlanningApprovalDate { get; private set; }
        public string? PlanningApprover { get; private set; }
        public string? PlanningApproverDisplayName { get; private set; }
        public string? TreatmentApprovalDate { get; private set; }
        public string? TreatmentApprover { get; private set; }
        public string? TreatmentApproverDisplayName { get; private set; }
        public string? Name { get; set; }
        public string? Course { get; set; }
        public string? DoseUID { get; set; }
        public string? StructureSetUID { get; set; }
        public string? CTSeriesUID { get; set; }
        public string? PlanType { get; set; }
        public string? ExpandedDisplay { get; set; }
        public string?   BulkDataString { get; set; }

        public Plan()
        {
        }

        public Plan(string name)
        {
            Name = name.Trim();
        }
    }

}
