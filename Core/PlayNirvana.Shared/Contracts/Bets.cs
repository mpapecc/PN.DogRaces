namespace PlayNirvana.Shared.Contracts
{
    public record ProcessBets(int RoundId, IEnumerable<RaceDogResultsRecord> RaceDogResults);

}
