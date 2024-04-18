namespace PhoneSaleAPI.DTO.Bill
{
    public class BillCreateDTO
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryAddress { get; set; }
        public String CustomerPhone { get; set; }
        public string Note { get; set; }
        public List<BillDetailDTO> BillDetails { get; set; } = new List<BillDetailDTO>();
    }
}
