namespace Refleks360.Domain.Benchmark;

public enum BenchmarkSourceType
{
    Mercer = 0,
    WillisTowersWatson = 1,
    KornFerry = 2,
    KPMG = 3,
    Aon = 4,
    Manual = 5,
    Other = 99,
}

public enum BenchmarkMatchType
{
    ExactMatch = 0,
    Approximate = 1,
    Manual = 2,
}
