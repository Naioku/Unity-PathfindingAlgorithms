using System;
using DefaultNamespace;
using UpdateSystem;

namespace StageMachineSystem
{
    public class StageMachine
    {
        private BaseStage currentStage;
        private CoroutineManager coroutineManager;
        private CoroutineManager.CoroutineCaller coroutineCaller;
        private Guid tickCoroutineId;

        public StageMachine(BaseStage initialStage)
        {
            coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
            SetStage(initialStage);
        }

        public void SetStage(BaseStage newStage)
        {
            currentStage?.Exit();
            currentStage = newStage;
            if (currentStage == null)
            {
                coroutineCaller.StopCoroutine(ref tickCoroutineId);
                tickCoroutineId = Guid.Empty;
            }
            else
            {
                currentStage.Enter();
                tickCoroutineId = coroutineCaller.StartCoroutine(Tick);
            }
        }

        private void Tick()
        {
            currentStage.Tick();
        }
    }
}