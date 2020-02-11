namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Issue")]
    public partial class Issue
    {
        public Issue()
        {

        }

        public Issue(int demandPK, string userID, string issueID)
        {
            IssueID = issueID;
            ExecutedDate = DateTime.Now;
            DemandPK = demandPK;
            UserID = userID;
            IsStorebacked = false;
            IsConfirmed = false;
        }

        [Key]
        public int IssuePK { get; set; }

        [StringLength(100)]
        public string IssueID { get; set; }

        public DateTime ExecutedDate { get; set; }

        public bool IsStorebacked { get; set; }

        public bool IsConfirmed { get; set; }

        public int DemandPK { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }
    }
}
