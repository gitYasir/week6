namespace NorthwindAPI.Models.DTO {
    public class SupplierDTO {
        public SupplierDTO() {
            Products = new List<ProductDTO>();
        }
        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactTitle { get; set; }
        public string? Country { get; set; }
        public int TotalProducts { get; init; }
        public ICollection<ProductDTO> Products { get; set; }
    }
}