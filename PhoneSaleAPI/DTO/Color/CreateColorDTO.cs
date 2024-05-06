namespace PhoneSaleAPI.DTO.Color
{
    public class CreateColorDTO
    {
        public string ColorName { get; set; } = null!;
        public IFormFile ColorImage { get; set; }
        public int? ColorPrice { get; set; }
    }
}
