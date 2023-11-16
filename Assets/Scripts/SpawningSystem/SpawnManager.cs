using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpawningSystem
{
    /// <summary>
    /// Manages objects spawning. Use with <see cref="Spawnable{TName,TType}"/> class.
    /// </summary>
    /// <typeparam name="TKey">Enum which contains all names of specific object's group You are creating SpawnManager for, e.g Units, Obstacles.</typeparam>
    /// <typeparam name="TType">If You want to add multiple prefabs within one object (e.g. Unit's tiers) You can specify another enum here. Otherwise use: <see cref="SpawnManager{TName}"/>.</typeparam>
    [Serializable]
    public class SpawnManager<TKey, TType> where TKey : Enum where TType : Enum
    {
        [SerializeField] private List<Spawnable<TKey, TType>> spawnableItems;

        private readonly Dictionary<TKey, Spawnable<TKey, TType>> entityItemsLookup = new Dictionary<TKey, Spawnable<TKey, TType>>();
        
        /// <summary>
        /// Initializes the manager. Should be used in MonoBehaviour's Awake() methods.
        /// </summary>
        public void Initialize()
        {
            foreach (Spawnable<TKey, TType> spawnableItem in spawnableItems)
            {
                entityItemsLookup.Add(spawnableItem.Name, spawnableItem);
            }
        }

        /// <summary>
        /// Instantiates an object locally.
        /// </summary>
        /// <param name="name">Object's name.</param>
        /// <param name="position">Initial position.</param>
        /// <param name="type">Prefab's name.</param>
        /// <returns>Instantiated GameObject.</returns>
        public GameObject CreateObject(TKey name, Vector3 position = default, TType type = default)
        {
            return Object.Instantiate(entityItemsLookup[name].GetPrefab(type), position, Quaternion.identity);
        }
        
        /// <inheritdoc cref="CreateObject(TKey, Vector3, TType)"/>
        /// <typeparam name="TReturn">Specifies returned component instead of GameObject.</typeparam>
        /// <returns>Specified component</returns>
        public TReturn CreateObject<TReturn>(TKey name, Vector3 position = default, TType type = default) where TReturn : Component
        {
            return CreateObject(name, position, type).GetComponent<TReturn>();
        }
        
        /// <inheritdoc cref="CreateObject(TKey, Vector3, TType)"/>
        /// <param name="parent">Parent which instantiated object will be parented to.</param>
        public GameObject CreateObject(TKey name, Transform parent, Vector3 localPosition = default, TType type = default)
        {
            return Object.Instantiate(entityItemsLookup[name].GetPrefab(type), parent.position + localPosition, Quaternion.identity, parent);
        }
        
        /// <inheritdoc cref="CreateObject(TKey, Transform, Vector3, TType)"/>
        /// <typeparam name="TReturn">Specifies returned component instead of GameObject.</typeparam>
        /// <returns>Specified component</returns>
        public TReturn CreateObject<TReturn>(TKey name, Transform parent, Vector3 localPosition = default, TType type = default) where TReturn : Component
        {
            return CreateObject(name, parent, localPosition, type).GetComponent<TReturn>();
        }
        
        /// <inheritdoc cref="CreateObject(TKey, Vector3, TType)"/>
        /// <param name="scriptableObject">Scriptable object related to the name.</param>
        /// <typeparam name="TReturn">Type which inherits from <see cref="Spawnable{TName,TType}"/></typeparam>
        public GameObject CreateObject<TReturn>(TKey name, Transform parent, Vector3 localPosition, out TReturn scriptableObject, TType type) where TReturn : Spawnable<TKey, TType>
        {
            scriptableObject = (TReturn)entityItemsLookup[name];
            return Object.Instantiate(entityItemsLookup[name].GetPrefab(type), parent.position + localPosition, Quaternion.identity, parent);
        }

        /// <summary>
        /// Destroys an object locally.
        /// </summary>
        /// <param name="instance">Instance of an object to destroy.</param>
        public void DestroyObject(GameObject instance)
        {
            Object.Destroy(instance);
        }
    }

    /// <summary>
    /// An one-parameter extension of class: <see cref="SpawnManager{TKey,TType}"/>. Use with <see cref="Spawnable{TName}"/> class.
    /// </summary>
    /// <typeparam name="TKey"><inheritdoc cref="SpawnManager{TName,TType}"/></typeparam>
    [Serializable]
    public class SpawnManager<TKey> : SpawnManager<TKey, Enums.EmptyEnum> where TKey : Enum {}
}