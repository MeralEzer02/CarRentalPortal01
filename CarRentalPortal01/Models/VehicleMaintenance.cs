using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalPortal01.Models
{
    public class VehicleMaintenance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public string Description { get; set; }

        // Bakım Türü (Periyodik, Arıza, Kaza, Lastik Değişimi vb.)
        public string MaintenanceType { get; set; }

        public decimal Cost { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}