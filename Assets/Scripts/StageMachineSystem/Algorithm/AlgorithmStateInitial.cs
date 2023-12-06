namespace StageMachineSystem.Algorithm
{
    public class AlgorithmStateInitial : AlgorithmStateBase
    {
        public AlgorithmStateInitial(Algorithm algorithm) : base(algorithm) {}

        public override bool Play()
        {
            algorithm.Play();
            return true;
        }

        public override bool Step()
        {
            algorithm.Step(false);
            return true;
        }
    }
}