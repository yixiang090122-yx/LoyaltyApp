namespace LoyaltyApp.DTOs
{
    public class RewardDTO
    {
        public int RewardsID { get; set; }
        public string? RewardGUID { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public int? Point { get; set; }
        public string? ShortTitle { get; set; }
        public string? Description { get; set; }
        public string? TnC { get; set; }
        public DateTime? RewardCreatedDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Status { get; set; }
    }

    public class RedeemRewardVM
    {
        public string RewardGUID { get; set; } = string.Empty;
        public string MemberPassword { get; set; } = string.Empty;
        public string OutletStaffPin { get; set; } = string.Empty;
    }

    public class CreateRewardVM
    {
        public string FullName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? SubTitle { get; set; }
        public string? ShortTitle { get; set; }
        public string? Description { get; set; }
        public string? TnC { get; set; }
        public int Point { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddYears(1);
        public string Status { get; set; } = "Active";
    }
}

