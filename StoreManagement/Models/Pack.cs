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

        public Pack(string PackID, int OrderPK)
        {
            this.PackID = PackID;
            this.OrderPK = OrderPK;
            this.DateCreated = DateTime.Now;
            this.IsOpened = true;
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
    }
}
