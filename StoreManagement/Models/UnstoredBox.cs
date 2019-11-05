namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UnstoredBox")]
    public partial class UnstoredBox
    {
        public UnstoredBox()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UnstoredBoxPK { get; set; }

        public int BoxPK { get; set; }
    }
}
