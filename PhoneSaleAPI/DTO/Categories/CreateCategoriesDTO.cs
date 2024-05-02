namespace PhoneSaleAPI.DTO.Categories
{
    public class CreateCategoriesDTO
    {
        public string CategoryName { get; set; }
        public IFormFile CategoryImage { get; set; }
        public int Status { get; set; }
    }
}
