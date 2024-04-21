namespace PhoneSaleAPI.DTO.Bill
{
    public class BillDetailDTO
    {
        public string ProductID { get; set; }
        public string ColorName { get; set; }
        public int StorageGB { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
    }
}
