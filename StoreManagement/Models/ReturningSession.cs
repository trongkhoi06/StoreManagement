namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReturningSession")]
    public partial class ReturningSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReturningSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int FailedItemPK { get; set; }
        public string EmployeeCode { get; set; }
        public ReturningSession()
        {
        }

        public ReturningSession(int failedItemPK, string employeeCode)
        {
            FailedItemPK = failedItemPK;
            EmployeeCode = employeeCode;
            ExecutedDate = DateTime.Now;
        }
    }
}
