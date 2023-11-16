using System;
using System.Collections.Generic;
using UnityEngine;

namespace InteractionSystem
{
    public class Interaction
    {
        private readonly Dictionary<Enums.InteractionType, Dictionary<Enums.InteractionState, Action<InteractionDataArgs>>> interactionTypeLookup =
            new Dictionary<Enums.InteractionType, Dictionary<Enums.InteractionState, Action<InteractionDataArgs>>>();
        private Transform interactionCollider;

        public Interaction(
            Transform owner,
            Vector3 size,
            float tileLenght)
        {
            CreateInteractionCollider(owner, size, tileLenght);
            InitializeActions();
        }
    
        private void CreateInteractionCollider(Transform owner, Vector3 size, float tileLenght)
        {
            interactionCollider = new GameObject("Interaction").transform;
            interactionCollider.parent = owner;
            interactionCollider.gameObject.AddComponent<BoxCollider>();
            interactionCollider.localScale = size;
            interactionCollider.position = 0.5f * tileLenght * size;
        }
    
        private void InitializeActions()
        {
            foreach (Enums.InteractionType actionType in Enum.GetValues(typeof(Enums.InteractionType)))
            {
                var initialActions = new Dictionary<Enums.InteractionState, Action<InteractionDataArgs>>
                { 
                    {Enums.InteractionState.EnterType, null},
                    {Enums.InteractionState.Tick, null},
                    {Enums.InteractionState.ExitType, null},
                    {Enums.InteractionState.EnterInteraction, null},
                    {Enums.InteractionState.ExitInteraction, null}
                };
                
                interactionTypeLookup.Add(actionType, initialActions);
            }
        }
    
        public void SetAction(
            Enums.InteractionType interactionType,
            Enums.InteractionState interactionState,
            Action<InteractionDataArgs> action)
        {
            interactionTypeLookup[interactionType][interactionState] = action;
        }
    
        public void Interact(InteractionDataSystem interactionDataSystem, InteractionDataArgs interactionDataArgs)
        {
            if (!interactionTypeLookup.ContainsKey(interactionDataSystem.InteractionType)) return;

            var actions = interactionTypeLookup[interactionDataSystem.InteractionType];
            if (!actions.ContainsKey(interactionDataSystem.InteractionState)) return;
            
            actions[interactionDataSystem.InteractionState]?.Invoke(interactionDataArgs);
        }
    
        public struct InteractionDataSystem
        {
            public Enums.InteractionType InteractionType { get; set; }
            public Enums.InteractionState InteractionState { get; set; }
        }
        
        public struct InteractionDataArgs
        {
            public RaycastHit HitInfo { get; set; }
        }
    }
}