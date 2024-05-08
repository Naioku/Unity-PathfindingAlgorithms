using System.Collections.Generic;
using System.Linq;
using UI.PopupPanels.EntryPanels;
using UnityEngine;

namespace UI.PopupPanels
{
    public class PermittedDirectionsPanel : InputPanel<Enums.PermittedDirection[]>
    {
        [SerializeField] private RectTransform content;
        
        private List<Enums.PermittedDirection> directions;
        private bool inChangingState;
        private Entry<Enums.PermittedDirection> entryToMove;
        
        public override GameObject SelectableOnOpen { get; }
        
        protected override void SetInitialValue(Enums.PermittedDirection[] initialValue)
        {
            directions = initialValue.ToList();
            foreach (Enums.PermittedDirection direction in directions)
            {
                Entry<Enums.PermittedDirection> entry = AllManagers.Instance.UIManager.UISpawner.CreateObject<Entry<Enums.PermittedDirection>>(Enums.UISpawned.EntryPermittedDirections, content);
                entry.Initialize(direction, HandleOnPress);
            }
        }

        protected override void Confirm() => onConfirm.Invoke(directions.ToArray());

        private void HandleOnPress(Entry<Enums.PermittedDirection> entry)
        {
            inChangingState = !inChangingState;
            if (inChangingState)
            {
                RemoveInput();
                confirmationButton.interactable = false;
                closeButton.interactable = false;
                entry.Selected = true;
                entryToMove = entry;
            }
            else
            {
                if (entry != entryToMove)
                {
                    int newIndex = directions.IndexOf(entry.Value);
                    directions.Remove(entryToMove.Value);
                    directions.Insert(newIndex, entryToMove.Value);
                    entryToMove.transform.SetSiblingIndex(newIndex);
                }
                
                entryToMove.Selected = false;
                entryToMove = null;
                
                AddInput();
                confirmationButton.interactable = true;
                closeButton.interactable = true;
            }
        }
    }
}