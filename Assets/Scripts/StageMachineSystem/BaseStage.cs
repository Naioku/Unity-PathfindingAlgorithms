using System;
using CustomInputSystem.ActionMaps;

namespace StageMachineSystem
{
    [Serializable]
    public abstract class BaseStage
    {
        protected SharedData sharedData;
        protected ActionMap.ActionData inputOnBackData;

        public void Initialize(SharedData sharedData)
        {
            this.sharedData = sharedData;
            inputOnBackData = AllManagers.Instance.InputManager.GlobalMap.OnBackData;
        }

        public virtual void Enter()
        {
            inputOnBackData.Performed += ExitStage;
        }

        public virtual void Tick() {}

        public virtual void Exit()
        {
            inputOnBackData.Performed -= ExitStage;
        }

        protected void ExitStage()
        {
            AllManagers.Instance.GameManager.ExitStage();
        }
    }
}