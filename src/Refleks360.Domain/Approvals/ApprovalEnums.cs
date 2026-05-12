namespace Refleks360.Domain.Approvals;

public enum ApprovalTriggerType
{
    Scenario = 0,
    IndividualRaise = 1,
    NewHire = 2,
    OffCycle = 3,
}

public enum ApprovalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3,
}

public enum ApprovalDecision
{
    Approve = 0,
    Reject = 1,
    Return = 2,
}
