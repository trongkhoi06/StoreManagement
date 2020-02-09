namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GroupItem_ArrangingSession
    {
        [Key]
        public int GroupItem_ArrangingSessionPK { get; set; }

        public int ItemPK { get; set; }

        public bool IsRestored { get; set; }

        public int ArrangingSessionPK { get; set; }
    }
}
