using System;
using System.Collections;
using DefaultNamespace;
using UpdateSystem.CoroutineSystem;

namespace StageMachineSystem
{
    public class StageMachine
    {
        private readonly CoroutineManager.CoroutineCaller coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        private Guid tickCoroutineId;
        private BaseStage currentStage;
        private SharedData SharedData { get; set; } = new SharedData();

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
                tickCoroutineId = coroutineCaller.StartCoroutine(Tick());
            }
        }

        private IEnumerator Tick()
        {
            while (true)
            {
                currentStage.Tick();
                yield return null;
            }
        }
    }
}