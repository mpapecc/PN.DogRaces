namespace PlayNirvana.Shared.Contracts
{
    public record ProcessRoundBets(int RoundId, IEnumerable<RaceDogResultsRecord> RaceDogResults);
    public record RoundFinished(int RoundId, IEnumerable<RaceDogResultsRecord> RaceDogResults);
}
