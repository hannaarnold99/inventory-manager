namespace InventoryManager.Models
{
    public class StockProduct
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<StockSublocation> StockSublocations { get; set; } = new List<StockSublocation>();

        public void UpdateQuantity()
        {
            Quantity = StockSublocations.Sum(s => s.SublocationQuantity);
        }
    }


    public class StockSublocation
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Sublocation { get; set; }
        public int SublocationQuantity { get; set; }

        public StockProduct StockProduct { get; set; }  // Navigation property
    }
}
