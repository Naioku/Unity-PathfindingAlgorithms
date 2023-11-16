using System.Collections.Generic;
using UnityEngine;

namespace UpdateSystem
{
    public class UpdateManager : MonoBehaviour
    {
        private readonly List<IUpdatable> objectsToUpdate = new List<IUpdatable>();

        public void Register(IUpdatable updatableObj)
        {
            objectsToUpdate.Add(updatableObj);
        }

        public void Unregister(IUpdatable updatableObj)
        {
            objectsToUpdate.Remove(updatableObj);
        }
        
        private void Update()
        {
            foreach (IUpdatable updatableObj in objectsToUpdate)
            {
                updatableObj.PerformUpdate();
            }
        }
    }
}