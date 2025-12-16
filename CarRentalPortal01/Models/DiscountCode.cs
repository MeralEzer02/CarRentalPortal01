using System;
using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal01.Models
{
    public class DiscountCode
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kampanya kodu zorunludur.")]
        [StringLength(20, ErrorMessage = "Kod en fazla 20 karakter olabilir.")]
        public string Code { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "İndirim oranı %1 ile %100 arasında olmalıdır.")]
        public int DiscountRate { get; set; } 

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);

        public bool IsActive { get; set; } = true;
    }
}