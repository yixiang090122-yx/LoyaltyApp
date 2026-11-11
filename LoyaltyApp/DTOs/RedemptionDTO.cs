namespace LoyaltyApp.DTOs
{
    public class RewardAssignDTO
    {
        public int RewardRecordID { get; set; }
        public long? MemberRedeemDateTime { get; set; }
        public string? OutletName { get; set; }
        public string? OutletStaffName { get; set; }
        public string? MemberName { get; set; }
        public string? MemberPhoneNumber { get; set; }
        public string? RewardTitle { get; set; }
        public string? RewardName { get; set; }
        public string? RewardDescription { get; set; }

        public DateTime? RedeemDate => MemberRedeemDateTime.HasValue 
            ? DateTimeOffset.FromUnixTimeMilliseconds(MemberRedeemDateTime.Value).LocalDateTime 
            : null;
    }

    public class VoucherAssignDTO
    {
        public int VoucherRecordID { get; set; }
        public string? VoucherName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? MemberName { get; set; }
        public string? MemberPhoneNumber { get; set; }
        public string? OutletName { get; set; }
        public string? OutletStaffName { get; set; }
        public long? MemberRedeemDateTime { get; set; }

        public DateTime? RedeemDate => MemberRedeemDateTime.HasValue 
            ? DateTimeOffset.FromUnixTimeMilliseconds(MemberRedeemDateTime.Value).LocalDateTime 
            : null;
    }
}

