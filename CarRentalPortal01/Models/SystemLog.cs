using System;
using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal01.Models
{
    public class SystemLog
    {
        [Key]
        public int Id { get; set; }

        public string? UserEmail { get; set; }

        public string ActionType { get; set; }

        public string Description { get; set; }

        public DateTime LogDate { get; set; } = DateTime.Now;

        public string? IpAddress { get; set; }
    }
}