namespace LoyaltyApp.DTOs
{
    public enum OutletStatus
    {
        Pending,
        Active,
        Deactive
    }

    public class OutletDTO
    {
        public int OutletID { get; set; }
        public string? OutletGUID { get; set; }
        public string? OutletName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public OutletStatus? OutletStatus { get; set; }
    }

    public class CreateOutletDTO
    {
        public string OutletName { get; set; } = string.Empty;
    }
}

