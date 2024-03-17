using UnityEngine;

namespace SpawningSystem
{
    [CreateAssetMenu(fileName = "Utility", menuName = "Spawnable objects/Create utility", order = 0)]
    public class UtilsSO : Spawnable<Enums.SpawnedUtils>
    {
        [SerializeField] private Enums.SpawnedUtils utilityName;
        [SerializeField] private GameObject prefab;

        public override Enums.SpawnedUtils Name => utilityName;
        public override GameObject GetPrefab(Enums.EmptyEnum type) => prefab;
    }
}