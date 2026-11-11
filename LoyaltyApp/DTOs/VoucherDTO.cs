namespace LoyaltyApp.DTOs
{
    public class VoucherRuleDTO
    {
        public int VoucherRuleID { get; set; }
        public string? DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }

    public class VoucherDTO
    {
        public int VoucherID { get; set; }
        public string? VoucherGUID { get; set; }
        public string? VoucherName { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? ShortTitle { get; set; }
        public string? Description { get; set; }
        public string? TNC { get; set; }
        public DateTime? VoucherCreatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string? VoucherStatus { get; set; }
        public List<VoucherRuleDTO>? VoucherRules { get; set; }
    }

    public class RedeemVoucherVM
    {
        public string VoucherGUID { get; set; } = string.Empty;
        public string MemberPassword { get; set; } = string.Empty;
        public string OutletStaffPin { get; set; } = string.Empty;
    }

    public class CreateVoucherVM
    {
        public string VoucherName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? ShortTitle { get; set; }
        public string? Description { get; set; }
        public string? TNC { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ExpiredDate { get; set; } = DateTime.Now.AddYears(1);
        public string VoucherRules { get; set; } = "[]";
    }
}

