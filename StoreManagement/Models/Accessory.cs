namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Accessory")]
    public partial class Accessory
    {
        public Accessory()
        {

        }

        public Accessory(string accessoryID, string accessoryDescription, string item, string art, string color, string comment, int accessoryTypePK, int supplierPK, int customerPK)
        {
            AccessoryID = accessoryID;
            AccessoryDescription = accessoryDescription;
            Item = item;
            Art = art;
            Color = color;
            Comment = comment;
            AccessoryTypePK = accessoryTypePK;
            SupplierPK = supplierPK;
            CustomerPK = customerPK;
            IsActive = true;
            Image = null;
        }

        [Key]
        public int AccessoryPK { get; set; }

        [Required]
        [StringLength(450)]
        public string AccessoryID { get; set; }

        [Required]
        public string AccessoryDescription { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [StringLength(50)]
        public string Item { get; set; }

        [StringLength(50)]
        public string Art { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        public string Comment { get; set; }

        public string Image { get; set; }

        public int AccessoryTypePK { get; set; }

        public int SupplierPK { get; set; }

        public int CustomerPK { get; set; }
    }
}
