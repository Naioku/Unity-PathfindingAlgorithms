﻿using System.Collections.Generic;
using CustomInputSystem;
using CustomInputSystem.ActionMaps;
using StageMachineSystem.Algorithm;
using UI.HUDPanels;

namespace StageMachineSystem
{
    public class AlgorithmStage : BaseStage
    {
        private readonly Algorithm.Algorithm algorithm;
        private readonly HUDControllerAlgorithm hudController;
        private AlgorithmStateBase algorithmState;
        private InputManager inputManager;
        private ActionMap.ActionData inputOnPlayData;
        private ActionMap.ActionData inputOnPauseData;
        private ActionMap.ActionData inputOnStepData;
        private ActionMap.ActionData inputOnStopData;

        public AlgorithmStage(HUDControllerAlgorithm hudController, Algorithm.Algorithm algorithm)
        {
            this.algorithm = algorithm;
            this.hudController = hudController;
        }

        public override void Enter()
        {
            base.Enter();
            InitInput();
            AddInput();
            algorithm.Initialize(
                sharedData.Maze,
                sharedData.UniqueTilesCoordsLookup[Enums.TileType.Start].Value,
                sharedData.UniqueTilesCoordsLookup[Enums.TileType.Destination].Value,
                () => SwitchAlgorithmState(new AlgorithmStateFinished(algorithm)));
            
            SwitchAlgorithmState(new AlgorithmStateInitial(algorithm));
            hudController.Initialize(new List<BaseHUDController.ButtonData>
            {
                new BaseHUDController.ButtonData{ Action = Play, Label = $"Play ({inputOnPlayData.Binding})"},
                new BaseHUDController.ButtonData{ Action = Pause, Label = $"Pause ({inputOnPauseData.Binding})"},
                new BaseHUDController.ButtonData{ Action = Step, Label = $"Step ({inputOnStepData.Binding})"},
                new BaseHUDController.ButtonData{ Action = Stop, Label = $"Stop ({inputOnStopData.Binding})"},
                new BaseHUDController.ButtonData{ Action = ExitStage, Label = $"Back ({inputOnExitStageData.Binding})"}
            });
            hudController.Show();
        }

        public override void Exit()
        {
            base.Exit();
            hudController.Hide();
            hudController.Deinitialize();
            RemoveInput();
            algorithmState.Stop();
        }

        private void SwitchAlgorithmState(AlgorithmStateBase newState)
        {
            algorithmState = newState;
            hudController.UpdateCurrentStateLabel(algorithmState.Name);
        }

        private void InitInput()
        {
            inputManager = AllManagers.Instance.InputManager;

            inputOnPlayData = inputManager.AlgorithmMap.OnPlayData;
            inputOnPauseData = inputManager.AlgorithmMap.OnPauseData;
            inputOnStepData = inputManager.AlgorithmMap.OnStepData;
            inputOnStopData = inputManager.AlgorithmMap.OnStopData;
        }

        private void AddInput()
        {
            inputOnPlayData.Performed += Play;
            inputOnPauseData.Performed += Pause;
            inputOnStepData.Performed += Step;
            inputOnStopData.Performed += Stop;

            inputManager.AlgorithmMap.Enable();
        }

        private void RemoveInput()
        {
            inputManager.AlgorithmMap.Disable();

            inputOnPlayData.Performed -= Play;
            inputOnPauseData.Performed -= Pause;
            inputOnStepData.Performed -= Step;
            inputOnStopData.Performed -= Stop;
        }
        
        private void Play()
        {
            if (algorithmState.Play())
            {
                SwitchAlgorithmState(new AlgorithmStatePlaying(algorithm));
            }
        }


        private void Pause()
        {
            if (algorithmState.Pause())
            {
                SwitchAlgorithmState(new AlgorithmStatePaused(algorithm));
            }
        }

        private void Step()
        {
            if (algorithmState.Step())
            {
                SwitchAlgorithmState(new AlgorithmStatePaused(algorithm));
            }
        }

        private void Stop()
        {
            if (algorithmState.Stop())
            {
                SwitchAlgorithmState(new AlgorithmStateInitial(algorithm));
            }
        }
    }
}