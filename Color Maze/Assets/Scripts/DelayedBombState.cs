public class DelayedBombState
{
    public const int DefaultFuseMoves = 3;

    public int MovesRemaining { get; private set; }
    public bool HasExploded { get; private set; }

    public DelayedBombState(
        int fuseMoves = DefaultFuseMoves)
    {
        MovesRemaining = fuseMoves > 0 ? fuseMoves : 1;
    }

    public bool AdvanceMove()
    {
        if (HasExploded)
        {
            return false;
        }

        MovesRemaining -= 1;

        if (MovesRemaining <= 0)
        {
            MovesRemaining = 0;
            HasExploded = true;
            return true;
        }

        return false;
    }
}
