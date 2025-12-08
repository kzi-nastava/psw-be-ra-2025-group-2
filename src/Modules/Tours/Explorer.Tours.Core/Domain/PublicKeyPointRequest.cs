using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public enum PublicKeyPointRequestStatus
{
    Pending,
    Approved,
    Rejected
}

public class PublicKeyPointRequest : Entity
{
    public long PublicKeyPointId { get; private set; }
    public long AuthorId { get; private set; }
    public PublicKeyPointRequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public long? ProcessedByAdminId { get; private set; }
    public string? RejectionReason { get; private set; }

    public PublicKeyPoint? PublicKeyPoint { get; private set; }

    private PublicKeyPointRequest() { }

    public PublicKeyPointRequest(long publicKeyPointId, long authorId)
    {
        if (publicKeyPointId < 0) 
            throw new ArgumentException("PublicKeyPointId must not be negative.", nameof(publicKeyPointId));
        if (authorId <= 0)
            throw new ArgumentException("AuthorId must be positive.", nameof(authorId));

        PublicKeyPointId = publicKeyPointId;
        AuthorId = authorId;
        Status = PublicKeyPointRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve(long adminId)
    {
        if (Status != PublicKeyPointRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved.");

        Status = PublicKeyPointRequestStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
        ProcessedByAdminId = adminId;
    }

    public void Reject(long adminId, string? reason = null)
    {
        if (Status != PublicKeyPointRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected.");

        Status = PublicKeyPointRequestStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
        ProcessedByAdminId = adminId;
        RejectionReason = reason;
    }
}