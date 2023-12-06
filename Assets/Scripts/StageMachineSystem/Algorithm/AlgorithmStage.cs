using CustomInputSystem;
using DefaultNamespace;
using UnityEngine.InputSystem;

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
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Play, Play);
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Pause, Pause);
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Step, Step);
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Stop, Stop);
            inputManager.SetActionMap(Enums.ActionMap.Algorithm);
        }

        private void RemoveInput()
        {
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Play, Play);
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Pause, Pause);
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Step, Step);
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Stop, Stop);
        }
        
        private void Play(InputAction.CallbackContext obj)
        {
            if (algorithmState.Play())
            {
                SwitchAlgorithmState(new AlgorithmStatePlaying(algorithm));
            }
        }


        private void Pause(InputAction.CallbackContext obj)
        {
            if (algorithmState.Pause())
            {
                SwitchAlgorithmState(new AlgorithmStatePaused(algorithm));
            }
        }

        private void Step(InputAction.CallbackContext obj)
        {
            if (algorithmState.Step())
            {
                SwitchAlgorithmState(new AlgorithmStatePaused(algorithm));
            }
        }

        private void Stop(InputAction.CallbackContext obj)
        {
            if (algorithmState.Stop())
            {
                SwitchAlgorithmState(new AlgorithmStateInitial(algorithm));
            }
        }
    }
}