namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ClassifyingSession")]
    public partial class ClassifyingSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassifyingSessionPK { get; set; }

        public DateTime ExecutedDate { get; set; }

        public string Comment { get; set; }

        public int ClassifiedItemPK { get; set; }

        public string EmployeeCode { get; set; }

        public ClassifyingSession()
        {
        }

        public ClassifyingSession(string comment, int classifiedItemPK, string employeeCode)
        {
            Comment = comment;
            ClassifiedItemPK = classifiedItemPK;
            EmployeeCode = employeeCode;
            ExecutedDate = DateTime.Now;
        }
    }
}
