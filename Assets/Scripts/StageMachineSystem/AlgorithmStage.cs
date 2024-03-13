using CustomInputSystem;
using StageMachineSystem.Algorithm;
using UI.HUDPanels;

namespace StageMachineSystem
{
    public class AlgorithmStage : BaseStage
    {
        private readonly Algorithm.Algorithm algorithm;
        private readonly HUDControllerAlgorithm hudControllerAlgorithm;
        private AlgorithmStateBase algorithmState;
        private InputManager inputManager;

        public AlgorithmStage(HUDControllerAlgorithm hudControllerAlgorithm, Algorithm.Algorithm algorithm)
        {
            this.algorithm = algorithm;
            this.hudControllerAlgorithm = hudControllerAlgorithm;
        }

        public override void Enter()
        {
            base.Enter();
            inputManager = AllManagers.Instance.InputManager;
            InitInput();
            algorithm.Initialize(
                sharedData.Maze,
                sharedData.UniqueTilesCoordsLookup[Enums.TileType.Start].Value,
                sharedData.UniqueTilesCoordsLookup[Enums.TileType.Destination].Value,
                () => SwitchAlgorithmState(new AlgorithmStateFinished(algorithm)));
            
            SwitchAlgorithmState(new AlgorithmStateInitial(algorithm));
            hudControllerAlgorithm.Initialize
            (
                Play,
                Pause,
                Step,
                Stop,
                sharedData.OnBack
            );
            hudControllerAlgorithm.Show();
        }

        public override void Exit()
        {
            base.Exit();
            hudControllerAlgorithm.Hide();
            hudControllerAlgorithm.Deinitialize();
            RemoveInput();
            algorithmState.Stop();
        }
        
        private void SwitchAlgorithmState(AlgorithmStateBase newState)
        {
            algorithmState = newState;
            hudControllerAlgorithm.UpdateCurrentStateLabel(algorithmState.Name);
        }

        private void InitInput()
        {
            inputManager.AlgorithmMap.OnPlayData.Performed += Play;
            inputManager.AlgorithmMap.OnPauseData.Performed += Pause;
            inputManager.AlgorithmMap.OnStepData.Performed += Step;
            inputManager.AlgorithmMap.OnStopData.Performed += Stop;

            inputManager.AlgorithmMap.Enable();
        }

        private void RemoveInput()
        {
            inputManager.AlgorithmMap.Disable();

            inputManager.AlgorithmMap.OnPlayData.Performed -= Play;
            inputManager.AlgorithmMap.OnPauseData.Performed -= Pause;
            inputManager.AlgorithmMap.OnStepData.Performed -= Step;
            inputManager.AlgorithmMap.OnStopData.Performed -= Stop;
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