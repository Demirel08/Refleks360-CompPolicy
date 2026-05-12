namespace Refleks360.Domain.Scenarios;

public enum ScenarioType
{
    GeneralRaise = 0,
    DepartmentRaise = 1,
    PositionRaise = 2,
    TargetedBudget = 3,
    MeritMatrix = 4,
    Simulation = 5,
}

public enum ScenarioStatus
{
    Draft = 0,
    Calculated = 1,
    Applied = 2,
    Cancelled = 3,
}
