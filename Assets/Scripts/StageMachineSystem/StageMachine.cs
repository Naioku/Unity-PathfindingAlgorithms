using System;
using DefaultNamespace;
using UpdateSystem;

namespace StageMachineSystem
{
    public class StageMachine
    {
        private readonly CoroutineManager.CoroutineCaller coroutineCaller;
        private Guid tickCoroutineId;
        private BaseStage currentStage;
        private SharedData SharedData { get; set; } = new SharedData();

        public StageMachine()
        {
            coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        }
        
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
                currentStage.Initialize(SharedData);
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