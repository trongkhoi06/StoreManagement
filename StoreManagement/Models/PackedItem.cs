namespace StoreManagement.Models
{
    using StoreManagement.Class;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PackedItem")]
    public partial class PackedItem : IEquatable<PackedItem>
    {
        public PackedItem()
        {
        }

        public PackedItem(int packPK, Client_OrderedItemPK_PackedQuantity_Comment temp)
        {
            PackedQuantity = temp.PackedQuantity;
            IsClassified = false;
            Comment = temp.Comment;
            OrderedItemPK = temp.OrderedItemPK;
            PackPK = packPK;
        }

        [Key]
        public int PackedItemPK { get; set; }

        public double PackedQuantity { get; set; }

        public bool IsClassified { get; set; }

        public string Comment { get; set; }

        public string ContractNumber { get; set; }

        public int OrderedItemPK { get; set; }

        public int PackPK { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PackedItem);
        }

        public bool Equals(PackedItem other)
        {
            return other != null &&
                   PackedItemPK == other.PackedItemPK &&
                   PackedQuantity == other.PackedQuantity &&
                   IsClassified == other.IsClassified &&
                   Comment == other.Comment &&
                   ContractNumber == other.ContractNumber &&
                   OrderedItemPK == other.OrderedItemPK &&
                   PackPK == other.PackPK;
        }

        public override int GetHashCode()
        {
            var hashCode = -1104046818;
            hashCode = hashCode * -1521134295 + PackedItemPK.GetHashCode();
            hashCode = hashCode * -1521134295 + PackedQuantity.GetHashCode();
            hashCode = hashCode * -1521134295 + IsClassified.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Comment);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContractNumber);
            hashCode = hashCode * -1521134295 + OrderedItemPK.GetHashCode();
            hashCode = hashCode * -1521134295 + PackPK.GetHashCode();
            return hashCode;
        }
    }
}
