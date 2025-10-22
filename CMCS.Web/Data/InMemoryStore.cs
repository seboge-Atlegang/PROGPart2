using System.Data;
using CMCS.Web.Models;


namespace CMCS.Web.Data;


public class InMemoryStore
{
    // Thread-safe simple in-memory lists
    public List<AppUser> Users { get; } = new List<AppUser>();
    public List<ClaimRecord> Claims { get; } = new List<ClaimRecord>();
    public List<ClaimDocument> Documents { get; } = new List<ClaimDocument>();



    private int _claimId = 1;
    private int _docId = 1;


    public InMemoryStore()
    {
        // Seed users (username/password)
        Users.Add(new AppUser { Id = "u1", UserName = "lecturer1@cmcs.local", Password = "password", FullName = "Lecturer One", Role = Roles.Lecturer });
        Users.Add(new AppUser { Id = "u2", UserName = "coordinator1@cmcs.local", Password = "password", FullName = "Coordinator One", Role = Roles.Coordinator });
        Users.Add(new AppUser { Id = "u3", UserName = "manager1@cmcs.local", Password = "password", FullName = "Manager One", Role = Roles.Manager });
        Users.Add(new AppUser { Id = "admin", UserName = "admin@cmcs.local", Password = "Admin123!", FullName = "Administrator", Role = Roles.Admin });
    }


    public int NextClaimId() => Interlocked.Increment(ref _claimId);
    public int NextDocId() => Interlocked.Increment(ref _docId);
}
