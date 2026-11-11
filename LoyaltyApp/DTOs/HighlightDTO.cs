namespace LoyaltyApp.DTOs
{
    public class HighlightDTO
    {
        public int HighlightID { get; set; }
        public string? HighlightGUID { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Description { get; set; }
        public string? TnC { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? Status { get; set; }
    }

    public class CreateHighlightVM
    {
        public string Title { get; set; } = string.Empty;
        public string? SubTitle { get; set; }
        public string? Description { get; set; }
        public string? TnC { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddYears(1);
    }
}

