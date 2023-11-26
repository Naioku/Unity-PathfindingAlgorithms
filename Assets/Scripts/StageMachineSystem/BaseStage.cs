namespace StageMachineSystem
{
    public abstract class BaseStage
    {
        protected readonly Maze maze;
        protected SharedData sharedData;

        protected BaseStage(Maze maze)
        {
            this.maze = maze;
        }

        public void Initialize(SharedData sharedData)
        {
            this.sharedData = sharedData;
        }

        public virtual void Enter() {}
        public virtual void Tick() {}
        public virtual void Exit() {}
    }
}