using System;
using System.Collections;
using UI;
using UI.Localization;
using UnityEngine;
using UpdateSystem.CoroutineSystem;

namespace StageMachineSystem
{
    public class StageMachine
    {
        private readonly CoroutineManager.CoroutineCaller coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        private Guid tickCoroutineId;
        private BaseStage currentStage;
        private readonly LocalizedContentCache localizedContentCache;
        
        private SharedData SharedData { get; } = new();

        public Maze.Maze Maze
        {
            set => SharedData.Maze = value;
        }

        public StageMachine(Maze.Maze maze)
        {
            SharedData.Maze = maze;
            localizedContentCache = new LocalizedContentCache
            (
                Enums.PopupText.AlgorithmCannotEnterHeader,
                Enums.PopupText.AlgorithmCannotEnterMessage
            );
        }
        
        /// <summary>
        /// Sets stage as current.
        /// </summary>
        /// <param name="newStage">New stage object to set.</param>
        /// <returns>True if stage has been correctly set.</returns>
        public bool SetStage(BaseStage newStage)
        {
            if (!SanityCheck(newStage)) return false;
            
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
                    AllManagers.Instance.UIManager.OpenPopupInfo
                    (
                        localizedContentCache.GetValue(Enums.PopupText.AlgorithmCannotEnterHeader),
                        localizedContentCache.GetValue(Enums.PopupText.AlgorithmCannotEnterMessage)
                    );
                    Debug.LogError("You can't enter Algorithm Stage with Start and Destination tiles not selected.");
                    return false;
                }
            }

            return true;
        }

        private bool AreUniqueTilesSet()
        {
            var values = SharedData.Maze.UniqueTilesCoordsLookup.Values;
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