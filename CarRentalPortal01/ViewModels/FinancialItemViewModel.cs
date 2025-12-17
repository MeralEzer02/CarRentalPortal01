using System;

namespace CarRentalPortal01.ViewModels
{
    public class FinancialItemViewModel
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }   
        public DateTime Date { get; set; }    
        public string Type { get; set; }      
        public string ColorClass { get; set; }
        public string Details { get; set; }
    }
}