namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IdentifiedItem_ArrangingSession
    {
        public IdentifiedItem_ArrangingSession()
        {
        }

        [Key]
        public int IdentifiedItem_ArrangingSessionPK { get; set; }

        public int IdentifiedItemPK { get; set; }

        public int ArrangingSessionPK { get; set; }
    }
}
