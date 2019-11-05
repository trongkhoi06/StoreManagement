namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Model")]
    public partial class Model
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModelID { get; set; }

        [Column("Model")]
        [StringLength(50)]
        public string Model1 { get; set; }

        public int? ConceptionCode { get; set; }
    }
}
