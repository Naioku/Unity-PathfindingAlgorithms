using System;
using UnityEngine;

namespace SpawningSystem
{
    /// <summary>
    /// Base type for all spawnable objects. Interface for SpawnManager, which ensures that SpawnManager can take necessary data.
    /// </summary>
    /// <typeparam name="TName">Enum which contains all names of specific object's group You are creating SpawnManager for, e.g Units, Obstacles.</typeparam>
    /// <typeparam name="TType">If You want to add multiple prefabs within one object (e.g. Unit's tiers) You can specify another enum here. Otherwise use: <see cref="Spawnable{TName}"/>.</typeparam>
    public abstract class Spawnable<TName, TType> : ScriptableObject where TName : Enum where TType : Enum
    {
        public abstract TName Name { get; }
        public abstract GameObject GetPrefab(TType type);
    }

    /// <summary>
    /// An one-parameter extension of class: <see cref="Spawnable{TName, TTYpe}"/>
    /// </summary>
    /// <typeparam name="TName"><inheritdoc cref="Spawnable{TName,TType}"/></typeparam>
    public abstract class Spawnable<TName> : Spawnable<TName, Enums.EmptyEnum> where TName : Enum {}
}