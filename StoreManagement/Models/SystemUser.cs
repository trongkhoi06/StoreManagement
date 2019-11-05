namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SystemUser")]
    public partial class SystemUser
    {
        [Key]
        [StringLength(50)]
        public string EmployeeCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public int RoleID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }
    }
}
