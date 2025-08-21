namespace PlayNirvana.Shared.Contracts
{
    public record RaceStartWithBetLock(int RoundId, IEnumerable<RaceDogResultsRecord> RaceDogResults);
}
