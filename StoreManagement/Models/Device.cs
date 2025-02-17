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
        public Device()
        {
        }

        public Device(string deviceCode, string deviceName)
        {
            DeviceCode = deviceCode;
            DeviceName = deviceName;
            DateCreated = DateTime.Now;
        }

        [Key]
        public int DevicePK { get; set; }

        [Required]
        public string DeviceCode { get; set; }

        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateCreated { get; set; }
    }
}
