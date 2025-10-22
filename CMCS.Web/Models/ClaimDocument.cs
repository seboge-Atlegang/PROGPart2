using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using CMCS.Web.Models;

namespace CMCS.Web.Models
{
    public class ClaimDocument
    {
        

        public int Id { get; set; }

        [ForeignKey(nameof(Claim))]
        public int ClaimId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty; // relative
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ClaimRecord? Claim { get; set; }

        // Navigation property for documents - one ClaimRecord can have many ClaimDocuments
        public ICollection<ClaimDocument>? Documents { get; set; }


        

        public ClaimDocument() { }
        public ICollection<ClaimDocument>? Document { get; internal set; }
        
    }
}



