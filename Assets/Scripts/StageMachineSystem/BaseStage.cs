using System;
using UI;

namespace StageMachineSystem
{
    [Serializable]
    public abstract class BaseStage
    {
        protected SharedData sharedData;

        public SharedData SharedData
        {
            set => sharedData = value;
        }

        public void Initialize(SharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        public virtual void Enter() {}
        public virtual void Tick() {}
        public virtual void Exit() {}
    }
}