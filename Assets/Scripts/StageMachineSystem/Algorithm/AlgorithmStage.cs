using CustomInputSystem;

namespace StageMachineSystem.Algorithm
{
    public class AlgorithmStage : BaseStage
    {
        private Algorithm algorithm;
        private AlgorithmStateBase algorithmState;
        private InputManager inputManager;

        public AlgorithmStage(Maze maze, Algorithm algorithm) : base(maze)
        {
            this.algorithm = algorithm;
        }

        public override void Enter()
        {
            base.Enter();
            inputManager = AllManagers.Instance.InputManager;
            InitInput();
            algorithm.Initialize(
                maze,
                sharedData.UniqueTilesCoordsLookup[Enums.TileType.Start].Value,
                sharedData.UniqueTilesCoordsLookup[Enums.TileType.Destination].Value,
                () => SwitchAlgorithmState(new AlgorithmStateFinished(algorithm)));
            
            SwitchAlgorithmState(new AlgorithmStateInitial(algorithm));
        }

        public override void Exit()
        {
            base.Exit();
            RemoveInput();
            algorithmState.Stop();
        }
        
        private void SwitchAlgorithmState(AlgorithmStateBase newState) => algorithmState = newState;
        
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