namespace PlayNirvana.Shared.Contracts
{
    public record RoundsForProcess(IEnumerable<int> roundIds);
    public record RoundsFinished(IEnumerable<int> roundIds);
}
