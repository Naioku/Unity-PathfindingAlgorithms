using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpdateSystem
{
    public class UpdateManager : MonoBehaviour
    {
        private readonly List<Action> objectsToUpdate = new List<Action>();
        private readonly List<Action> objectsToFixedUpdate = new List<Action>();
        private readonly List<Action> objectsToLateUpdate = new List<Action>();
        private int currentIndex;

        // Todo: Add execution priority management - use List<> and Sort(). After that You must handle adding 
        public void RegisterOnUpdate(Action updatableObj, int priority = 0) => AddAction(updatableObj, objectsToUpdate);
        public void RegisterOnFixedUpdate(Action updatableObj, int priority = 0) => AddAction(updatableObj, objectsToFixedUpdate);
        public void RegisterOnLateUpdate(Action updatableObj, int priority = 0) => AddAction(updatableObj, objectsToLateUpdate);

        public void UnregisterFromUpdate(Action updatableObj) => RemoveAction(updatableObj, objectsToUpdate);
        public void UnregisterFromFixedUpdate(Action updatableObj) => RemoveAction(updatableObj, objectsToFixedUpdate);
        public void UnregisterFromLateUpdate(Action updatableObj) => RemoveAction(updatableObj, objectsToLateUpdate);

        public void Update() => InvokeActions(objectsToUpdate);
        public void FixedUpdate() => InvokeActions(objectsToFixedUpdate);
        public void LateUpdate() => InvokeActions(objectsToLateUpdate);

        private void AddAction(Action what, List<Action> where)
        {
            where.Add(what);
            int index = where.IndexOf(what);
            if (index <= currentIndex)
            {
                currentIndex++;
            }
        }
        
        private void RemoveAction(Action what, List<Action> from)
        {
            int index = from.IndexOf(what);
            from.RemoveAt(index);
            if (index <= currentIndex)
            {
                currentIndex--;
            }
        }
        
        private void InvokeActions(List<Action> actionsList)
        {
            for (currentIndex = 0; currentIndex < actionsList.Count; currentIndex++)
            {
                var updatableObj = actionsList[currentIndex];
                updatableObj.Invoke();
            }
        }
    }
}