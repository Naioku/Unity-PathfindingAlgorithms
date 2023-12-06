namespace StageMachineSystem.Algorithm
{
    public abstract class AlgorithmStateBase
    {
        protected Algorithm algorithm;
        
        protected AlgorithmStateBase(Algorithm algorithm)
        {
            this.algorithm = algorithm;
        }

        public virtual bool Play() => false;
        public virtual bool Pause() => false;
        public virtual bool Step() => false;
        public virtual bool Stop() => false;
        
        public virtual void Enter() {}
        public virtual void Exit() {}
    }
}