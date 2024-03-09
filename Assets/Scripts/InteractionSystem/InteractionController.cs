﻿using System;
using System.Collections;
using CustomInputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UpdateSystem.CoroutineSystem;

namespace InteractionSystem
{
    [Serializable]
    public class InteractionController
    {
        [SerializeField] private float interactionRange = 99;

        private Camera mainCamera;
        private CoroutineManager.CoroutineCaller coroutineCaller;
        private IInteractable currentInteraction;
        private RaycastHit currentHitInfo;
        private Guid performInteractionId;
            
        /// <summary>
        /// Interaction type, which should be currently used like: Hover, Click, Key. Should be set by input.
        /// </summary>
        private Enums.InteractionType currentInteractionType = DefaultInteractionType;
        private const Enums.InteractionType DefaultInteractionType = Enums.InteractionType.Hover;

        public void Initialize(Camera camera)
        {
            mainCamera = camera;
            coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
            InitializeInput();
            StartInteracting();
        }

        public void Destroy()
        {
            StopInteracting();
        }

        private void StartInteracting()
        {
            performInteractionId = coroutineCaller.StartCoroutine(PerformInteraction());
        }

        private void StopInteracting()
        {
            coroutineCaller.StopCoroutine(ref performInteractionId);
        }

        private void InitializeInput()
        {
            InputManager inputManager = AllManagers.Instance.InputManager;
            inputManager.SetOnPerformed(Enums.ActionMap.Global, Enums.InputAction.ClickInteraction, StartClickInteraction);
            inputManager.SetOnCanceled(Enums.ActionMap.Global, Enums.InputAction.ClickInteraction, StopClickInteraction);
        }

        private void StartClickInteraction(InputAction.CallbackContext obj) => SwitchInteractionType(Enums.InteractionType.Click);
        private void StopClickInteraction(InputAction.CallbackContext obj) => SwitchInteractionType(DefaultInteractionType);

        private IEnumerator PerformInteraction()
        {
            while (true)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                var currentlyCheckedInteraction =
                    !Physics.Raycast(ray, out currentHitInfo, interactionRange) ? null : currentHitInfo.transform.parent.GetComponent<IInteractable>();
            
                if (currentlyCheckedInteraction != currentInteraction)
                {
                    SwitchInteraction(currentlyCheckedInteraction);
                }
            
                Interact(Enums.InteractionState.Tick);
                yield return null;
            }
        }

        private void SwitchInteraction(IInteractable currentlyCheckedInteraction)
        {
            Interact(Enums.InteractionState.ExitInteraction);
            currentInteraction = currentlyCheckedInteraction;
            if (currentInteraction != null)
            {
                Interact(Enums.InteractionState.EnterInteraction);
            }
        }

        private void SwitchInteractionType(Enums.InteractionType interactionType)
        {
            Interact(Enums.InteractionState.ExitType);
            currentInteractionType = interactionType;
            Interact(Enums.InteractionState.EnterType);
        }
        
        private void Interact(Enums.InteractionState interactionState)
        {
            if (currentInteraction == null) return;

            Interaction.InteractionDataSystem interactionDataSystem = new Interaction.InteractionDataSystem
            {
                InteractionType = currentInteractionType,
                InteractionState = interactionState
            };
            
            Interaction.InteractionDataArgs interactionDataArgs = new Interaction.InteractionDataArgs
            {
                HitInfo = currentHitInfo,
            };
            
            currentInteraction.Interact(interactionDataSystem, interactionDataArgs);

            if (interactionState == Enums.InteractionState.ExitInteraction)
            {
                currentInteraction = null;
            }
        }
    }
}