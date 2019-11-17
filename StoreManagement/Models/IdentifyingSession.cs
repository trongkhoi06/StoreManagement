namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IdentifyingSession")]
    public partial class IdentifyingSession
    {
        public IdentifyingSession()
        {
        }

        public IdentifyingSession(string employeeCode)
        {
            this.ExecutedDate = DateTime.Now;
            EmployeeCode = employeeCode;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdentifyingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }
        [StringLength(50)]
        public string EmployeeCode { get; set; }
    }
}
