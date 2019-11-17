namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ArrangingSession")]
    public partial class ArrangingSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArrangingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int StartBoxPK { get; set; }

        public int DestinationBoxPK { get; set; }

        [StringLength(50)]
        public string EmployeeCode { get; set; }

        public ArrangingSession()
        {
        }

        public ArrangingSession(DateTime executedDate, int startBoxPK, int destinationBoxPK, string employeeCode)
        {
            ExecutedDate = executedDate;
            StartBoxPK = startBoxPK;
            DestinationBoxPK = destinationBoxPK;
            EmployeeCode = employeeCode;
        }
    }
}
