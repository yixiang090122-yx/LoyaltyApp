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

    public class OutletStaffDTO
    {
        public int OutletStaffID { get; set; }
        public string? OutletStaffName { get; set; }
        public string? Pin { get; set; }
        public string? Role { get; set; }
        public int? OutletID { get; set; }
        public OutletDTO? Outlet { get; set; }
    }

    public class CreateOutletStaffDTO
    {
        public string OutletStaffName { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int OutletID { get; set; }
    }
}
