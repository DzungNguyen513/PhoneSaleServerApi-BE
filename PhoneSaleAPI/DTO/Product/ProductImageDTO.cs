namespace PhoneSaleAPI.DTO.Product
{
    public class ProductImageDTO
    {
        public IFormFile ImageFile { get; set; }
        public bool IsPrimary { get; set; }
        public string? ColorName { get; set; }
    }

}
