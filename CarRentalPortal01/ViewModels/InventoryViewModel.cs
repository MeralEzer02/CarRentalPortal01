namespace CarRentalPortal01.ViewModels
{
    public class InventoryItemViewModel
    {
        public string Brand { get; set; }        
        public string Model { get; set; }        
        public string Color { get; set; }        
        public string FuelType { get; set; }     
        public string GearType { get; set; }     


        public int TotalCount { get; set; }      
        public int AvailableCount { get; set; }  
        public int RentedCount { get; set; }     
    }
}