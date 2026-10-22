public class LevelRunState
{
    public int StepLimit { get; }
    public int StepsRemaining { get; private set; }

    public bool IsExhausted => StepsRemaining <= 0;

    public LevelRunState(int stepLimit)
    {
        StepLimit = stepLimit > 0 ? stepLimit : 1;
        StepsRemaining = StepLimit;
    }

    public bool TrySpendStep()
    {
        if (StepsRemaining <= 0)
        {
            return false;
        }

        StepsRemaining -= 1;
        return true;
    }
}
