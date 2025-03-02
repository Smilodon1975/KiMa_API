namespace KiMa_API.Models.Dto
{
    public class PasswordResetRequestDto
    {
        public string Email { get; set; }
    }

    public class PasswordResetDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}

