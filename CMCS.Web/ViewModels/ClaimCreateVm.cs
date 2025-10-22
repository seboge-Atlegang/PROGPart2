using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CMCS.Web.ViewModels
{
    public class ClaimCreateVm
    {
        [Required, Range(0.1, 10000)]
        public decimal HoursWorked { get; set; }

        [Required, Range(0.01, 100000)]
        public decimal HourlyRate { get; set; }

        public string Notes { get; set; }

        // optional file
        public IFormFile File { get; set; }
    }
}




