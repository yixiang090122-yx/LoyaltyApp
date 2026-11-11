using System.ComponentModel.DataAnnotations;

namespace LoyaltyApp.DTOs
{
    public enum AccountStatus
    {
        Pending,
        Active,
        Inactive
    }

    public class MemberDTO
    {
        public int MemberID { get; set; }
        public string? MemberGUID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? BirthDate { get; set; }
        public string? ReferralCode { get; set; }
        public string? ReferralBy { get; set; }
        public string? Tier { get; set; }
        public string? Image { get; set; }
        public AccountStatus? AccountStatus { get; set; }
        public DateTime? RegisterTime { get; set; }
        public int? Point { get; set; }

        // Compatibility properties
        public string Points => (Point ?? 0).ToString();
        public string TierName => Tier ?? "Standard";
    }

    public class RegisterMemberVM
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Phone number must start with 0 (e.g. 0123456789)")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birth date is required")]
        public string BirthDate { get; set; } = string.Empty; // Format yyyy-MM-dd
    }

    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class ResetPasswordVM
    {
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP is required")]
        public string OTP { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UpdateProfileVM
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? BirthDate { get; set; }
    }
}
