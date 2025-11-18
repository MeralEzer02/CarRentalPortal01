using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalPortal01.Models
{
    public class Rental
    {
        public int Id { get; set; }

        //Kiralama
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }

        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; }
    }
}
