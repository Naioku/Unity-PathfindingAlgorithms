namespace StageMachineSystem
{
    public abstract class BaseStage
    {
        protected Maze maze;

        protected BaseStage(Maze maze)
        {
            this.maze = maze;
        }

        public virtual void Enter() {}
        public virtual void Tick() {}
        public virtual void Exit() {}
    }
}