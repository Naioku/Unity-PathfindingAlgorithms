using System;
using System.Collections;
using UnityEngine;
using UpdateSystem.CoroutineSystem;

namespace StageMachineSystem
{
    public class StageMachine
    {
        private readonly CoroutineManager.CoroutineCaller coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        private Guid tickCoroutineId;
        private BaseStage currentStage;
        private SharedData SharedData { get; } = new SharedData();

        public StageMachine(Maze maze)
        {
            SharedData.Maze = maze;
        }
        
        /// <summary>
        /// Sets stage as current.
        /// </summary>
        /// <param name="newStage">New stage object to set.</param>
        /// <returns>True if stage has been correctly set.</returns>
        public bool SetStage(BaseStage newStage)
        {
            if (!SanityCheck(newStage)) return false;
            
            AllManagers.Instance.InputManager.StageSelectionMap.Disable();
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
                if (tickCoroutineId == Guid.Empty)
                {
                    tickCoroutineId = coroutineCaller.StartCoroutine(Tick());
                }
            }

            return true;
        }

        private bool SanityCheck(BaseStage newStage)
        {
            if (newStage == null) return true;
            if (newStage.GetType() == typeof(AlgorithmStage))
            {
                if (!AreUniqueTilesSet())
                {
                    AllManagers.Instance.UIManager.OpenInfoPanel("Algorithm", "You can't enter the algorithm with Start and Destination tiles not selected.");
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