using UnityEngine;

namespace SpawningSystem
{
    [CreateAssetMenu(fileName = "Utility", menuName = "Spawnable objects/Create utility", order = 0)]
    public class UtilsSO : Spawnable<Enums.Utils>
    {
        [SerializeField] private Enums.Utils utilityName;
        [SerializeField] private GameObject prefab;

        public override Enums.Utils Name => utilityName;
        public override GameObject GetPrefab(Enums.EmptyEnum type) => prefab;
    }
}