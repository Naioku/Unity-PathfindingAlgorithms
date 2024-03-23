using UnityEngine;

namespace SpawningSystem
{
    [CreateAssetMenu(fileName = "UI", menuName = "Spawnable objects/Create UI", order = 0)]
    public class UISO : Spawnable<Enums.UISpawned>
    {
        [SerializeField] private Enums.UISpawned uiName;
        [SerializeField] private GameObject prefab;

        public override Enums.UISpawned Name => uiName;
        public override GameObject GetPrefab(Enums.EmptyEnum type) => prefab;
    }
}