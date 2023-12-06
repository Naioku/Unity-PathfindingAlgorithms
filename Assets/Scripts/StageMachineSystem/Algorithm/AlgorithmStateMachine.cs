namespace StageMachineSystem.Algorithm
{
    public class AlgorithmStateMachine
    {
        private AlgorithmStateBase algorithmState;

        public AlgorithmStateMachine(AlgorithmStateBase initialState)
        {
            algorithmState = initialState;
        }

        public void SwitchState(AlgorithmStateBase newState) => algorithmState = newState;

        // public void Play() => algorithmState.Play();
        // public void Pause() => algorithmState.Pause();
        // public void Step() => algorithmState.Step();
        // public void Stop() => algorithmState.Stop();
    }
}