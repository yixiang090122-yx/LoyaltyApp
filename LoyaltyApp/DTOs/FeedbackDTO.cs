namespace LoyaltyApp.DTOs
{
    public class FeedbackDTO
    {
        public int FeedbackID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Rating { get; set; }
        public string? Category { get; set; }
        public int? MemberID { get; set; }
        public string? MemberGUID { get; set; }
        public string? Location { get; set; }
    }

    public class CreateFeedbackVM
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Rating { get; set; } = "5";
        public string Category { get; set; } = "Service";
        public string Location { get; set; } = string.Empty;
    }
}

