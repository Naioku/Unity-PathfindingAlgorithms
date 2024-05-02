using CustomInputSystem;
using CustomInputSystem.ActionMaps;
using SpawningSystem;
using StageMachineSystem.Algorithm;
using UI.HUDPanels;

namespace StageMachineSystem
{
    public class AlgorithmStage : BaseStage
    {
        private readonly SpawnManager<Enums.UISpawned> uiSpawner = AllManagers.Instance.UIManager.UISpawner;
        private readonly HUDControllerAlgorithm hudController;
        private InputManager inputManager;
        private ActionMap.ActionData inputOnPlayData;
        private ActionMap.ActionData inputOnPauseData;
        private ActionMap.ActionData inputOnStepData;
        private ActionMap.ActionData inputOnStopData;
        
        private Algorithm.Algorithm algorithm;
        private AlgorithmStateBase algorithmState;

        public AlgorithmStage()
        {
            InitInput();
            hudController = uiSpawner.CreateObject<HUDControllerAlgorithm>(Enums.UISpawned.HUDAlgorithm);
            hudController.Initialize
            (
                new ButtonData{ Action = ExitStage, Binding = inputOnBackData.Binding },
                new ButtonDataTagged<Enums.AlgorithmAction>[]
                {
                    new(){ Tag = Enums.AlgorithmAction.Play, Action = Play, Binding = inputOnPlayData.Binding },
                    new(){ Tag = Enums.AlgorithmAction.Pause, Action = Pause, Binding = inputOnPauseData.Binding },
                    new(){ Tag = Enums.AlgorithmAction.Step, Action = Step, Binding = inputOnStepData.Binding },
                    new(){ Tag = Enums.AlgorithmAction.Stop, Action = Stop, Binding = inputOnStopData.Binding }
                }
            );
        }
        
        public void ChangeAlgorithm(Algorithm.Algorithm algorithm)
        {
            algorithmState?.Stop();
            this.algorithm = algorithm;
        }

        public override void Enter()
        {
            base.Enter();
            AddInput();
            hudController.Show();
            algorithm.Initialize
            (
                sharedData.Maze,
                () => SwitchAlgorithmState(new AlgorithmStateFinished(algorithm))
            );
            SwitchAlgorithmState(new AlgorithmStateInitial(algorithm));
        }

        public override void Exit()
        {
            base.Exit();
            hudController.Hide();
            RemoveInput();
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
            inputOnPlayData.Performed += InputPlay;
            inputOnPauseData.Performed += InputPause;
            inputOnStepData.Performed += InputStep;
            inputOnStopData.Performed += InputStop;

            inputManager.AlgorithmMap.Enable();
        }

        private void RemoveInput()
        {
            inputManager.AlgorithmMap.Disable();

            inputOnPlayData.Performed -= InputPlay;
            inputOnPauseData.Performed -= InputPause;
            inputOnStepData.Performed -= InputStep;
            inputOnStopData.Performed -= InputStop;
        }
        
        private void InputPlay()
        {
            Play();
            hudController.SelectButton(Enums.AlgorithmAction.Play);
        }
        
        private void InputPause()
        {
            Pause();
            hudController.SelectButton(Enums.AlgorithmAction.Pause);
        }
        
        private void InputStep()
        {
            Step();
            hudController.SelectButton(Enums.AlgorithmAction.Step);
        }
        
        private void InputStop()
        {
            Stop();
            hudController.SelectButton(Enums.AlgorithmAction.Stop);
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