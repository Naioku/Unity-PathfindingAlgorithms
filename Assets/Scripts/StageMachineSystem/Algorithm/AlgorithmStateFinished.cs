namespace StageMachineSystem.Algorithm
{
    public class AlgorithmStateFinished : AlgorithmStateBase
    {
        public AlgorithmStateFinished(Algorithm algorithm) : base(algorithm) {}

        public override bool Stop()
        {
            algorithm.Refresh();
            return true;
        }
    }
}