using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace UI.PopupPanels.EntryPanels
{
    public class PermittedDirectionsPanel : InputPanel<PermittedDirection[]>
    {
        [SerializeField] private RectTransform content;
        
        private readonly List<EntryPermittedDirection> directionEntries = new();
        private bool inChangingState;
        private EntryPermittedDirection entryToMove;
        
        public override GameObject SelectableOnOpen { get; }
        
        protected override void SetInitialValue(PermittedDirection[] initialValue)
        {
            foreach (PermittedDirection direction in initialValue)
            {
                EntryPermittedDirection entry = AllManagers.Instance.UIManager.UISpawner.CreateObject<EntryPermittedDirection>(Enums.UISpawned.EntryPermittedDirections, content);
                entry.Initialize(direction, HandleOnPress);
                directionEntries.Add(entry);
            }
        }

        protected override void Confirm()
        {
            PermittedDirection[] directions = new PermittedDirection[directionEntries.Count];
            for (var i = 0; i < directionEntries.Count; i++)
            {
                directions[i] = directionEntries[i].Value;
            }

            onConfirm.Invoke(directions);
        }

        private void HandleOnPress(Entry<PermittedDirection, Enums.PermittedDirection> entry)
        {
            EntryPermittedDirection entryPermittedDirection = (EntryPermittedDirection)entry;
            
            inChangingState = !inChangingState;
            if (inChangingState)
            {
                RemoveInput();
                confirmationButton.interactable = false;
                closeButton.interactable = false;
                entry.Selected = true;
                entryToMove = entryPermittedDirection;
            }
            else
            {
                if (entry != entryToMove)
                {
                    int newIndex = directionEntries.IndexOf(entryPermittedDirection);
                    directionEntries.Remove(entryToMove);
                    directionEntries.Insert(newIndex, entryToMove);
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