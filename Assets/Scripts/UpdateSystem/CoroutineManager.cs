using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace UpdateSystem
{
    public class CoroutineManager : IUpdatable
    {
        private UpdateManager updateManager;
        private readonly Dictionary<object, CoroutinesItem> coroutinesItemsLookup = new Dictionary<object, CoroutinesItem>();
        private readonly List<CoroutinesItem> coroutinesItems = new List<CoroutinesItem>();

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

        private Guid StartCoroutine(object caller, Action action, int actionPriority = 0)
        {
            Guid coroutineId;
            if (coroutinesItemsLookup.TryGetValue(caller, out CoroutinesItem item))
            {
                coroutineId = item.AddCoroutine(action, actionPriority);
            }
            else
            {
                CoroutinesItem coroutinesItem = new CoroutinesItem(action, actionPriority, out coroutineId);
                coroutinesItemsLookup.Add(caller, coroutinesItem);
                coroutinesItems.Add(coroutinesItem);
                coroutinesItems.Sort();
            }

            return coroutineId;
        }

        private void StopCoroutine(CoroutineCaller caller, Guid coroutineId)
        {
            if (!coroutinesItemsLookup.TryGetValue(caller, out CoroutinesItem item))
            {
                Debug.LogError("This object has no coroutine started.");
                return;
            }
            
            item.RemoveCoroutine(coroutineId);
            if (!item.HasAnyCoroutine())
            {
                coroutinesItems.Remove(item);
                coroutinesItems.Sort();
                coroutinesItemsLookup.Remove(caller);
            }
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
                if (!coroutinesLookup.TryGetValue(id, out CoroutineData coroutineData))
                {
                    Debug.LogError("You are trying to stop the coroutine, which doesn't exist.");
                    return;
                }
                
                coroutines.Remove(coroutineData);
                coroutines.Sort();
                coroutinesLookup.Remove(id);
            }
            
            public bool HasAnyCoroutine()
            {
                return coroutines.Count > 0;
            }

            public void PerformAllCoroutines()
            {
                foreach (CoroutineData coroutine in coroutines)
                {
                    coroutine.Action.Invoke();
                }
            }
            
            public int CompareTo(CoroutinesItem other)
            {
                return Priority.CompareTo(other.Priority);
            }
        }
        
        private readonly struct CoroutineData : IComparable<CoroutineData>
        {
            private readonly int priority;
            public Action Action { get; }

            public CoroutineData(Action action, int priority)
            {
                this.priority = priority;
                Action = action;
            }

            public int CompareTo(CoroutineData other)
            {
                return priority.CompareTo(other.priority);
            }
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