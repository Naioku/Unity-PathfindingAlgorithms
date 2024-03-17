using System;
using CustomInputSystem.ActionMaps;

namespace StageMachineSystem
{
    [Serializable]
    public abstract class BaseStage
    {
        protected SharedData sharedData;
        protected ActionMap.ActionData inputOnExitStageData;

        public void Initialize(SharedData sharedData)
        {
            this.sharedData = sharedData;
            inputOnExitStageData = AllManagers.Instance.InputManager.GlobalMap.OnExitStageData;
        }

        public virtual void Enter()
        {
            inputOnExitStageData.Performed += ExitStage;
        }

        public virtual void Tick() {}

        public virtual void Exit()
        {
            inputOnExitStageData.Performed -= ExitStage;
        }

        protected void ExitStage()
        {
            AllManagers.Instance.GameManager.ExitStage();
        }
    }
}