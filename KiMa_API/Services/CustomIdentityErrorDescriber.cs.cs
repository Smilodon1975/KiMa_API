using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Identity;



namespace KiMa_API.Services
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = "Diese E-Mail-Adresse ist bereits registriert."
            };
        }
    }
}
