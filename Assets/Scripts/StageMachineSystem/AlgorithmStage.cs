using CustomInputSystem;
using DefaultNamespace;
using UnityEngine.InputSystem;

namespace StageMachineSystem
{
    public class AlgorithmStage : BaseStage
    {
        private readonly IAlgorithm algorithm;
        private InputManager inputManager;

        public AlgorithmStage(Maze maze, IAlgorithm algorithm) : base(maze)
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
                sharedData.UniqueTilesCoordsLookup[Enums.TileType.Destination].Value);
        }

        public override void Exit()
        {
            base.Exit();
            RemoveInput();
            algorithm.Stop();
        }
        
        private void InitInput()
        {
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Play, Play);
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Pause, Pause);
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Step, Step);
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Refresh, Refresh);
            inputManager.SetOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Stop, Stop);
            inputManager.SetActionMap(Enums.ActionMap.Algorithm);
        }

        private void RemoveInput()
        {
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Play, Play);
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Pause, Pause);
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Step, Step);
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Refresh, Refresh);
            inputManager.RemoveOnPerformed(Enums.ActionMap.Algorithm, Enums.InputAction.Stop, Stop);
        }

        private void Play(InputAction.CallbackContext obj) => algorithm.Play();
        private void Pause(InputAction.CallbackContext obj) => algorithm.Pause();
        private void Step(InputAction.CallbackContext obj) => algorithm.Step();
        private void Refresh(InputAction.CallbackContext obj) => algorithm.Refresh();
        private void Stop(InputAction.CallbackContext obj)
        {
            algorithm.Refresh();
            algorithm.Stop();
        }
    }
}