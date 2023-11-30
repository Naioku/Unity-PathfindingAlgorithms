namespace UpdateSystem.CoroutineSystem
{
    public class WaitUntil : IWait
    {
        public delegate bool WaitUntilCondition();

        private readonly WaitUntilCondition condition;

        public WaitUntil(WaitUntilCondition condition)
        {
            this.condition = condition;
        }

        public bool CanPerformNow()
        {
            return condition.Invoke();
        }
    }
}