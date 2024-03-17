using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class StageSelectionMap : ActionMap, Controls.IStageSelectionActions
    {
        public ActionData OnMazeModificationData { get; }
        public ActionData OnBFSData { get; }
        public ActionData OnAStarData { get; }
        
        public StageSelectionMap(Controls.StageSelectionActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);
            
            OnMazeModificationData = new ActionData(actionMap.MazeModification);
            OnBFSData = new ActionData(actionMap.BFS);
            OnAStarData = new ActionData(actionMap.AStar);
        }

        public void OnMazeModification(InputAction.CallbackContext context) => OnMazeModificationData.Invoke(context.phase);
        public void OnBFS(InputAction.CallbackContext context) => OnBFSData.Invoke(context.phase);
        public void OnAStar(InputAction.CallbackContext context) => OnAStarData.Invoke(context.phase);
    }
}