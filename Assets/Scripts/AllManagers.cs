using CustomInputSystem;
using SpawningSystem;
using UnityEngine;
using UpdateSystem;
using UpdateSystem.CoroutineSystem;

namespace DefaultNamespace
{
    public class AllManagers : MonoBehaviour
    {
        [field: SerializeField] public GameManager GameManager { get; private set; }
        [field: SerializeField] public SpawnManager<Enums.Utils> UtilsSpawner { get; private set; }
        [field: SerializeField] public UpdateManager UpdateManager { get; private set; }
        public CoroutineManager CoroutineManager { get; private set; }
        public InputManager InputManager { get; private set; }
        
        public static AllManagers Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            CoroutineManager = new CoroutineManager();
            InputManager = new InputManager();

            CoroutineManager.Initialize();
            UtilsSpawner.Initialize();
            InputManager.Initialize();
            GameManager.Initialize();
        }

        private void Start()
        {
            GameManager.StartGame();
        }

        private void OnDestroy()
        {
            InputManager.Destroy();
            CoroutineManager.Destroy();
            GameManager.Destroy();
        }
    }
}