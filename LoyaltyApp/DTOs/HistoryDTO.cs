namespace LoyaltyApp.DTOs
{
    public enum HistoryType
    {
        Voucher,
        Reward,
        Point
    }

    public enum DateRange
    {
        All,
        Today,
        ThisWeek,
        ThisMonth,
        ThisYear
    }

    public class HistoryRecordDTO
    {
        public int RecordID { get; set; }
        public string? RecordGUID { get; set; }
        public int? MemberID { get; set; }
        public int? OutletStaffID { get; set; }
        public bool? Claim { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public HistoryType RecordType { get; set; }
        public int? Points { get; set; }
        public string? Remark { get; set; }
        public string? Status { get; set; }
        public VoucherDTO? VoucherDetail { get; set; }
        public RewardDTO? RewardDetail { get; set; }
        public PointRecordDTO? PointDetail { get; set; }
    }
}

