namespace StageMachineSystem.Algorithm
{
    public class AlgorithmStateFinished : AlgorithmStateBase
    {
        public override Enums.AlgorithmState Name => Enums.AlgorithmState.Finished;
        
        public AlgorithmStateFinished(Algorithm algorithm) : base(algorithm) {}

        public override bool Stop()
        {
            algorithm.Refresh();
            return true;
        }
    }
}