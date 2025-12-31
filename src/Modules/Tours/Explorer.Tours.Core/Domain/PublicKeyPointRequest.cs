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
    public PublicKeyPoint PublicKeyPoint { get; set; } = null!;

    private PublicKeyPointRequest() { }

    public PublicKeyPointRequest(long publicKeyPointId, long authorId)
    {
        ValidatePublicKeyPointId(publicKeyPointId);
        ValidateAuthorId(authorId);

        PublicKeyPointId = publicKeyPointId;
        AuthorId = authorId;
        Status = PublicKeyPointRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve(long adminId)
    {
        ValidateCanBeProcessed();
        ValidateAdminId(adminId);

        Status = PublicKeyPointRequestStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
        ProcessedByAdminId = adminId;
    }

    public void Reject(long adminId, string? reason = null)
    {
        ValidateCanBeProcessed();
        ValidateAdminId(adminId);

        Status = PublicKeyPointRequestStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
        ProcessedByAdminId = adminId;
        RejectionReason = reason?.Trim();
    }

    public bool IsPending() => Status == PublicKeyPointRequestStatus.Pending;
    public bool IsApproved() => Status == PublicKeyPointRequestStatus.Approved;
    public bool IsRejected() => Status == PublicKeyPointRequestStatus.Rejected;

    private static void ValidatePublicKeyPointId(long publicKeyPointId)
    {
        if (publicKeyPointId <= 0)
            throw new ArgumentException("Invalid PublicKeyPointId.", nameof(publicKeyPointId));
    }

    private static void ValidateAuthorId(long authorId)
    {
        if (authorId == 0)
            throw new ArgumentException("Invalid AuthorId.", nameof(authorId));
    }

    private static void ValidateAdminId(long adminId)
    {
        if (adminId == 0)
            throw new ArgumentException("Invalid AdminId.", nameof(adminId));
    }

    private void ValidateCanBeProcessed()
    {
        if (Status != PublicKeyPointRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be processed.");
    }
    public void ResetToPending()
    {
        if (Status != PublicKeyPointRequestStatus.Rejected)
            throw new InvalidOperationException("Only rejected requests can be reset to pending.");

        Status = PublicKeyPointRequestStatus.Pending;
        ProcessedAt = null;
        ProcessedByAdminId = null;
        RejectionReason = null;
    }
}