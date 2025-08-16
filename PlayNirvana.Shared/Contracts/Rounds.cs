namespace PlayNirvana.Shared.Contracts
{
    public record RoundsForProcess(IEnumerable<RoundOutcome> RoundOutcomes);
    public record RoundsFinished(IEnumerable<int> roundIds);
    public record RoundsStarted(IEnumerable<int> roundIds);
    public record RoundOutcome(int RoundId, IEnumerable<RaceDogResultsRecord> RaceDogResults);

}
