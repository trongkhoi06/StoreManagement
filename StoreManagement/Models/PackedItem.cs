namespace StoreManagement.Models
{
    using StoreManagement.Class;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PackedItem")]
    public partial class PackedItem
    {
        public PackedItem()
        {

        }

        public PackedItem(int packPK, Client_OrderedItemPK_PackedQuantity_Comment temp)
        {
            PackPK = packPK;
            PackedQuantity = temp.PackedQuantity;
            Comment = temp.Comment;
            OrderedItemPK = temp.OrderedItemPK;
            IsClassified = false;
            ContractNumber = "";
        }

        [Key]
        public int PackedItemPK { get; set; }

        public double PackedQuantity { get; set; }

        public bool IsClassified { get; set; }

        public string Comment { get; set; }

        public string ContractNumber { get; set; }

        public int OrderedItemPK { get; set; }

        public int PackPK { get; set; }
    }
}
