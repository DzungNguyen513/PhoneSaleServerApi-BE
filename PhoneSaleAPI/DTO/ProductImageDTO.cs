namespace PhoneSaleAPI.DTO
{
    public class ProductImageDTO
    {
        public IFormFile ImageFile { get; set; }
        public bool IsPrimary { get; set; }
        public String? ColorName { get; set; }
    }

}
