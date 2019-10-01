namespace StoreManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Device")]
    public partial class Device
    {
        public int DeviceID { get; set; }

        [Required]
        [StringLength(50)]
        public string DeviceName { get; set; }

        public string DeviceInformation { get; set; }
    }
}
