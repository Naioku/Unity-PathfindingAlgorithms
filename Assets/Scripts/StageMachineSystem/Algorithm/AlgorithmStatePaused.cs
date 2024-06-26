﻿namespace StageMachineSystem.Algorithm
{
    public class AlgorithmStatePaused : AlgorithmStateBase
    {
        public override Enums.AlgorithmState Name => Enums.AlgorithmState.Paused;
        
        public AlgorithmStatePaused(Algorithm algorithm) : base(algorithm) {}

        public override bool Play()
        {
            algorithm.Resume();
            return true;
        }

        public override bool Step()
        {
            algorithm.Step(true);
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