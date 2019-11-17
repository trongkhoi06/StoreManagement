namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CountingSession")]
    public partial class CountingSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountingSessionPK { get; set; }

        public int CountedQuantity { get; set; }

        public DateTime ExecutedDate { get; set; }

        public int IdentifiedItemPK { get; set; }

        public string EmployeeCode { get; set; }
        public CountingSession()
        {
        }

        public CountingSession(int identifiedItemPK, int countedQuantity, string employeeCode)
        {
            CountedQuantity = countedQuantity;
            ExecutedDate = DateTime.Now;
            IdentifiedItemPK = identifiedItemPK;
            this.EmployeeCode = employeeCode;
        }
    }
}
