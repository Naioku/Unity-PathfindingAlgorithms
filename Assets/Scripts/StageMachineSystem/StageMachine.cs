using System;
using System.Collections;
using StageMachineSystem.Algorithm;
using UnityEngine;
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
            if (!SanityCheck(newStage)) return;
            
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

        private bool SanityCheck(BaseStage newStage)
        {
            if (newStage == null) return true;
            if (newStage.GetType() == typeof(AlgorithmStage))
            {
                if (!AreUniqueTilesSet())
                {
                    // Todo: Create log-to-user system.
                    Debug.LogError("You can't enter Algorithm Stage with Start and Destination tiles not selected.");
                    return false;
                }
            }

            return true;
        }

        private bool AreUniqueTilesSet()
        {
            var values = SharedData.UniqueTilesCoordsLookup.Values;
            foreach (Vector2Int? value in values)
            {
                if (!value.HasValue) return false;
            }

            return true;
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