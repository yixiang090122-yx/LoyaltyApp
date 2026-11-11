namespace LoyaltyApp.DTOs
{
    public enum PointRecordStatus
    {
        Pending,
        Approve,
        Rejected
    }

    public class AssignPointVM
    {
        public int Point { get; set; }
        public string? Code { get; set; }
    }

    public class ApprovePointVM
    {
        public int Point { get; set; }
        public string OutletGUID { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public PointRecordStatus PointRecordStatus { get; set; } = PointRecordStatus.Approve;
        public string Code { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }

    public class OutletAssignRecordVM
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public int Point { get; set; }
        public int OutletID { get; set; }
        public string Pin { get; set; } = string.Empty;
    }

    public class PointRecordDTO
    {
        public int PointRecordID { get; set; }
        public string? PointRecordGUID { get; set; }
        public int? Point { get; set; }
        public string? Description { get; set; }
        public string? Code { get; set; }
        public DateTime DateAssign { get; set; }
        public PointRecordStatus PointRecordStatus { get; set; }
        public string? Remark { get; set; }
        public MemberDTO? Member { get; set; }
        public OutletStaffDTO? OutletStaff { get; set; }
    }

    public class RejectPointVM
    {
        public string Id { get; set; } = string.Empty; // PointRecordGUID
        public OutletAssignRecordVM AssignPoint { get; set; } = new();
    }
}

