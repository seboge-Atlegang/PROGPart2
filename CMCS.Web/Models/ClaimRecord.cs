using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Web.Models;
public enum ClaimStatus { Pending = 0, Approved = 1, Rejected = 2, Settled = 3 }


public class ClaimRecord
{
    public int Id { get; set; }
    public string LecturerId { get; set; } = string.Empty;
    public string LecturerName { get; set; } = string.Empty;
    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? DocumentId { get; set; }

    public ICollection<ClaimDocument>? Document { get; set; }
   
   // public string Documents { get; internal set; }


    // public string Document {  get; internal set; } 

}

