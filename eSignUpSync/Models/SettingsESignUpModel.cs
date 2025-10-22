using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSignUpSync.Models
{
    [Keyless]
    public class SettingsESignUpModel
    {
        public string? Endpoint { get; set; }
        public string? Client { get; set; }
        public string? Secret { get; set; }
        public string? APIKey { get; set; }
    }
}
