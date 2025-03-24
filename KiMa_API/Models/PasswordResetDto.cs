namespace KiMa_API.Models.Dto
{
    /// DTO für die Anforderung eines Passwort-Reset-Links.
    public class PasswordResetRequestDto
    {
        public required string Email { get; set; }
    }

    public class PasswordResetDto
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }
}

