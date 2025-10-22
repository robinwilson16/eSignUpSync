using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace eSignUpEBSAPI.Models
{
    [Keyless]
    public class SettingsExportModel
    {
        public string? APIEndPoint { get; set; }
    }
}
