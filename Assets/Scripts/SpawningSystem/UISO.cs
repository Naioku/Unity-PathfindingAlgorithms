using UnityEngine;

namespace SpawningSystem
{
    [CreateAssetMenu(fileName = "UI", menuName = "Spawnable objects/Create UI", order = 0)]
    public class UISO : Spawnable<Enums.SpawnedUI>
    {
        [SerializeField] private Enums.SpawnedUI uiName;
        [SerializeField] private GameObject prefab;

        public override Enums.SpawnedUI Name => uiName;
        public override GameObject GetPrefab(Enums.EmptyEnum type) => prefab;
    }
}