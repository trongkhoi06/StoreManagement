namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Role")]
    public partial class Role
    {
        public Role()
        {
        }

        [Key]
        [StringLength(50)]
        public string RoleName { get; set; }

        public string RoleInformation { get; set; }
    }
}
