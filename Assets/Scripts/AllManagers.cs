using CustomInputSystem;
using SpawningSystem;
using UI;
using UnityEngine;
using UpdateSystem;
using UpdateSystem.CoroutineSystem;

public class AllManagers : MonoBehaviour
{
    [field: SerializeField] public GameManager GameManager { get; private set; }
    [field: SerializeField] public SpawnManager<Enums.SpawnedUtils> UtilsSpawner { get; private set; }
    [field: SerializeField] public SpawnManager<Enums.UISpawned> UISpawner { get; private set; }
    [field: SerializeField] public SpawnManager<Enums.UIPopupType> UIPopupSpawner { get; private set; }
    [field: SerializeField] public UIManager UIManager { get; private set; }
    public UpdateManager UpdateManager { get; private set; }
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

        UpdateManager = new UpdateManager();
        CoroutineManager = new CoroutineManager();
        InputManager = new InputManager();

        CoroutineManager.Awake();
        UtilsSpawner.Awake();
        UISpawner.Awake();
        UIPopupSpawner.Awake();
        InputManager.Awake();
        UIManager.Awake();
        GameManager.Awake();
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

    private void Update() => UpdateManager.Update();
    private void FixedUpdate() => UpdateManager.FixedUpdate();
    private void LateUpdate() => UpdateManager.LateUpdate();
}