namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Pack")]
    public partial class Pack
    {
        public Pack()
        {
        }

        public Pack(string packID, int orderPK, string employeeCode)
        {
            PackID = PackID;
            OrderPK = OrderPK;
            DateCreated = DateTime.Now;
            IsOpened = true;
            EmployeeCode = employeeCode;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackPK { get; set; }

        [Required]
        [StringLength(100)]
        public string PackID { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsOpened { get; set; }

        public int OrderPK { get; set; }

        public string EmployeeCode { get; set; }
    }
}
