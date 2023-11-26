using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace UpdateSystem
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

        private Guid StartCoroutine(CoroutineCaller caller, Action action, int actionPriority = 0)
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

        private Guid CreateNewCoroutinesItem(CoroutineCaller caller, Action action, int actionPriority)
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
            
            if (!coroutinesItem.DeactivateCoroutine(coroutineId)) return;
            
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

        public void PerformUpdate()
        {
            foreach (CoroutinesItem data in coroutinesItems)
            {
                data.PerformAllCoroutines();
            }

            // Because obviously You can't modify collection while iterating on it is still performing.
            StopCoroutinesInternal();
        }

        private void StopCoroutinesInternal()
        {
            if (coroutinesToRemove.Count > 0) return;
            
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
            
            public CoroutinesItem(Action action, int actionPriority, out Guid id)
            {
                id = AddCoroutine(action, actionPriority);
            }

            public Guid AddCoroutine(Action action, int actionPriority)
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

            public bool DeactivateCoroutine(Guid id)
            {
                if (!coroutinesLookup.TryGetValue(id, out CoroutineData coroutineData))
                {
                    Debug.LogError("You are trying to stop the coroutine, which doesn't exist.");
                    return false;
                }

                coroutineData.IsActive = false;
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
                    if (!coroutine.IsActive) continue;
                    coroutine.Action.Invoke();
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
            public bool IsActive { get; set; }
            public Action Action { get; }

            public CoroutineData(Action action, int priority)
            {
                this.priority = priority;
                Action = action;
                IsActive = true;
            }

            public int CompareTo(CoroutineData other)
            {
                return priority.CompareTo(other.priority);
            }
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

            public Guid StartCoroutine(Action action, int priority = 0)
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