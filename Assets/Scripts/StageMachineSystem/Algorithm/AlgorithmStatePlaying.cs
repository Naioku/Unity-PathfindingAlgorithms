namespace StageMachineSystem.Algorithm
{
    public class AlgorithmStatePlaying : AlgorithmStateBase
    {
        public override Enums.AlgorithmState Name => Enums.AlgorithmState.Playing;
        
        public AlgorithmStatePlaying(Algorithm algorithm) : base(algorithm) {}

        public override bool Pause()
        {
            algorithm.Pause();
            return true;
        }

        public override bool Step()
        {
            algorithm.Pause();
            return true;
        }

        public override bool Stop()
        {
            algorithm.Stop();
            algorithm.Refresh();
            return true;
        }
    }
}