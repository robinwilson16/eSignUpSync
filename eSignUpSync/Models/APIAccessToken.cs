using System.ComponentModel.DataAnnotations;

namespace eSignUpSync.Models
{
    public class APIAccessToken
    {
        [Key]
        public string? Token { get; set; }

        public int? ExpireIn { get; set; }
    }
}
