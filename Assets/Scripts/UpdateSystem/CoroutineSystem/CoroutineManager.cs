using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace UpdateSystem.CoroutineSystem
{
    public class CoroutineManager : IUpdatable
    {
        private UpdateManager updateManager;
        private readonly Dictionary<CoroutineCaller, CoroutinesItem> coroutinesItemsLookup = new Dictionary<CoroutineCaller, CoroutinesItem>();
        private readonly List<CoroutinesItem> coroutinesItems = new List<CoroutinesItem>();
        private readonly List<StopCoroutineData> coroutinesToRemove = new List<StopCoroutineData>();

        public CoroutineCaller GenerateCoroutineCaller() => new CoroutineCaller(this);
        
        public void Initialize()
        {
            updateManager = AllManagers.Instance.UpdateManager;
            updateManager.Register(this);
        }

        public void Destroy()
        {
            updateManager.Unregister(this);
        }
        
        public void PerformUpdate()
        {
            foreach (CoroutinesItem data in coroutinesItems)
            {
                data.PerformAllCoroutines();
            }

            // Because obviously You can't modify collection while iterating on it is still performing.
            StopCoroutinesInternal();
        }

        private Guid StartCoroutine(CoroutineCaller caller, IEnumerator action, int actionPriority = 0)
        {
            Guid coroutineId;
            if (coroutinesItemsLookup.TryGetValue(caller, out CoroutinesItem item))
            {
                coroutineId = item.AddCoroutine(action, actionPriority);
            }
            else
            {
                coroutineId = CreateNewCoroutinesItem(caller, action, actionPriority);
            }

            return coroutineId;
        }

        private Guid CreateNewCoroutinesItem(CoroutineCaller caller, IEnumerator action, int actionPriority)
        {
            CoroutinesItem coroutinesItem = new CoroutinesItem(action, actionPriority, out Guid coroutineId);
            coroutinesItemsLookup.Add(caller, coroutinesItem);
            coroutinesItems.Add(coroutinesItem);
            coroutinesItems.Sort();
            return coroutineId;
        }

        private void StopCoroutine(CoroutineCaller caller, Guid coroutineId)
        {
            if (!coroutinesItemsLookup.TryGetValue(caller, out CoroutinesItem coroutinesItem))
            {
                Debug.LogError("This object has no coroutine started.");
                return;
            }
            
            if (!coroutinesItem.MarkCoroutineForDestruction(coroutineId)) return;
            
            coroutinesToRemove.Add(new StopCoroutineData
            {
                Caller = caller,
                Id = coroutineId
            });
        }

        private void SetCallerPriority(CoroutineCaller caller, int priority)
        {
            if (coroutinesItemsLookup.TryGetValue(caller, out CoroutinesItem item))
            {
                item.Priority = priority;
                coroutinesItems.Sort();
            }
            else
            {
                Debug.LogError("This object has no coroutine started.");
            }
        }

        private void StopCoroutinesInternal()
        {
            if (coroutinesToRemove.Count == 0) return;
            
            foreach (StopCoroutineData data in coroutinesToRemove)
            {
                StopCoroutineInternal(data);
            }

            coroutinesToRemove.Clear();
        }

        private void StopCoroutineInternal(StopCoroutineData data)
        {
            CoroutinesItem item = coroutinesItemsLookup[data.Caller];
            
            item.RemoveCoroutine(data.Id);
            if (!item.HasAnyCoroutine())
            {
                RemoveCoroutinesItem(data, item);
            }
        }

        private void RemoveCoroutinesItem(StopCoroutineData data, CoroutinesItem item)
        {
            coroutinesItems.Remove(item);
            coroutinesItems.Sort();
            coroutinesItemsLookup.Remove(data.Caller);
        }
        
        private class CoroutinesItem : IComparable<CoroutinesItem>
        {
            private readonly Dictionary<Guid, CoroutineData> coroutinesLookup = new Dictionary<Guid, CoroutineData>();
            private readonly List<CoroutineData> coroutines = new List<CoroutineData>();
            
            public int Priority { get; set; }
            
            public CoroutinesItem(IEnumerator action, int actionPriority, out Guid id)
            {
                id = AddCoroutine(action, actionPriority);
            }

            public Guid AddCoroutine(IEnumerator action, int actionPriority)
            {
                Guid currentId = Guid.NewGuid();
                CoroutineData newCoroutineData = new CoroutineData(action, actionPriority);
                coroutinesLookup.Add(currentId, newCoroutineData);
                coroutines.Add(newCoroutineData);
                coroutines.Sort();
                return currentId;
            }

            public void RemoveCoroutine(Guid id)
            {
                CoroutineData coroutineData = coroutinesLookup[id];
                
                coroutines.Remove(coroutineData);
                coroutines.Sort();
                coroutinesLookup.Remove(id);
            }

            public bool MarkCoroutineForDestruction(Guid id)
            {
                if (!coroutinesLookup.TryGetValue(id, out CoroutineData coroutineData))
                {
                    Debug.LogError("You are trying to stop the coroutine, which doesn't exist.");
                    return false;
                }

                coroutineData.MarkForDestruction();
                return true;
            }
            
            public bool HasAnyCoroutine()
            {
                return coroutines.Count > 0;
            }

            public void PerformAllCoroutines()
            {
                foreach (CoroutineData coroutine in coroutines)
                {
                    coroutine.Perform();
                }
            }
            
            public int CompareTo(CoroutinesItem other)
            {
                return Priority.CompareTo(other.Priority);
            }
        }
        
        private class CoroutineData : IComparable<CoroutineData>
        {
            private readonly int priority;
            private readonly IEnumerator action;
            private bool markedForDestruction;

            public CoroutineData(IEnumerator action, int priority)
            {
                this.priority = priority;
                this.action = action;
            }
            
            public void MarkForDestruction() => markedForDestruction = true;

            public void Perform()
            {
                if (markedForDestruction) return;
                if (!PerformInternal(action)) MarkForDestruction();
            }
            
            /// <summary>
            /// Performs an action as a coroutine.
            /// </summary>
            /// <param name="action">Action to perform.</param>
            /// <returns>True - action can be continued. False - action is exhausted (passed the last yield return statement and reach the end of the action).</returns>
            private bool PerformInternal(IEnumerator action)
            {
                object response = action.Current;

                switch (response)
                {
                    case IWait wait:
                        if (!wait.CanPerformNow()) return true;
                        return action.MoveNext();
                    
                    case IEnumerator subAction:
                        if (PerformInternal(subAction)) return true;
                        return action.MoveNext();

                    default:
                        return action.MoveNext();
                }
            }

            public int CompareTo(CoroutineData other) => priority.CompareTo(other.priority);
        }
        
        private struct StopCoroutineData
        {
            public CoroutineCaller Caller { get; set; }
            public Guid Id { get; set; }
        }
        
        public class CoroutineCaller
        {
            private readonly CoroutineManager coroutineManager;

            public CoroutineCaller(CoroutineManager coroutineManager)
            {
                this.coroutineManager = coroutineManager;
            }

            public Guid StartCoroutine(IEnumerator action, int priority = 0)
            {
                return coroutineManager.StartCoroutine(this, action, priority);
            }

            public void StopCoroutine(ref Guid coroutineId)
            {
                coroutineManager.StopCoroutine(this, coroutineId);
                coroutineId = Guid.Empty;
            }

            public void SetPriority(int priority)
            {
                coroutineManager.SetCallerPriority(this, priority);
            }
        }
    }
}