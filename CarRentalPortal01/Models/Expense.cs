using System;
using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal01.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Gider başlığı zorunludur.")]
        public string Title { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tutar 0'dan küçük olamaz.")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string? Description { get; set; }
    }
}