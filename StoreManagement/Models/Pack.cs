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

        public Pack(string packID, int orderPK, string userID)
        {
            PackID = packID;
            OrderPK = orderPK;
            DateCreated = DateTime.Now;
            IsOpened = true;
            UserID = userID;
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

        public string UserID { get; set; }
    }
}
