using System;
using System.Collections;
using CustomInputSystem;
using Settings;
using UnityEngine;
using UnityEngine.Serialization;
using UpdateSystem.CoroutineSystem;
using Utilities;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cameraObject;
    
    [SerializeField] private float borderModeMovementSpeed = 20;
    [SerializeField] private float keyModeMovementSpeed = 0.05f;
    [SerializeField] private float screenBorderThickness = 10;
    [SerializeField] private float screenXMaxMargin;
    [SerializeField] private float screenXMinMargin;
    [SerializeField] private float screenZMaxMargin;
    [SerializeField] private float screenZMinMargin;
    [SerializeField] private float zoomTrailLength;
    [SerializeField, Range(0, 1)] private float zoomTargetNormalizedPosition = 0.5f;
    [SerializeField] private float zoomStep = 0.1f;
    [FormerlySerializedAs("zoomUpdateTime")] [SerializeField] private float zoomSpeed = 0.2f;
    [SerializeField] private Color trailColor = Color.magenta;

    private GameSettings settings;
    private CoroutineManager.CoroutineCaller coroutineCaller;

    private Tuple<float, float> screenXLimits;
    private Tuple<float, float> screenZLimits;
    private Vector2 keyModeMovementCursorLockPosition;
    private Enums.CameraMovementMode movementMode;
    private Guid movementCoroutineId;
    private Guid zoomingCoroutineId;
    private Vector3 minZoomLocalPosition;
    private Vector3 maxZoomLocalPosition;
    private float zoomNormalizedPosition;

    private void Awake()
    {
        coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        settings = AllManagers.Instance.GameManager.GameSettings;

        UpdateScreenLimits();
        UpdateTrail();
        SetInitPosition();
    }

    public void OnDestroy() => StopMovement();

    private void OnDrawGizmos()
    {
        Gizmos.color = trailColor;
        Gizmos.DrawLine(transform.TransformPoint(minZoomLocalPosition), transform.TransformPoint(maxZoomLocalPosition));
    }

    private void OnValidate()
    {
        UpdateTrail();
        UpdateZoom(true);
    }

    private void AddInput()
    {
        InputManager inputManager = AllManagers.Instance.InputManager;
        inputManager.GlobalMap.OnCameraMovementData.Performed += StartKeyModeMovement;
        inputManager.GlobalMap.OnCameraMovementData.Canceled += StopKeyModeMovement;
        inputManager.GlobalMap.OnCameraZoomData.Performed += UpdateZoomTarget;
    }

    private void RemoveInput()
    {
        InputManager inputManager = AllManagers.Instance.InputManager;
        inputManager.GlobalMap.OnCameraMovementData.Performed -= StartKeyModeMovement;
        inputManager.GlobalMap.OnCameraMovementData.Canceled -= StopKeyModeMovement;
        inputManager.GlobalMap.OnCameraZoomData.Performed -= UpdateZoomTarget;
    }
    
    public void SetInitPosition()
    {
        float initialX = (screenXLimits.Item1 + screenXLimits.Item2) / 2;
        float initialZ = (screenZLimits.Item1 + screenZLimits.Item2) / 2;
        transform.position = new Vector3(initialX, transform.position.y, initialZ);
        zoomNormalizedPosition = zoomTargetNormalizedPosition;
    }

    public void StartMovement()
    {
        AddInput();
        movementCoroutineId = coroutineCaller.StartCoroutine(Perform2DMovement());
    }

    public void StopMovement()
    {
        RemoveInput();
        coroutineCaller.StopCoroutine(ref movementCoroutineId);
        movementMode = default;
    }

    public void UpdateScreenLimits()
    {
        float boardRealWidth = settings.BoardWidth * settings.TileLength;
        float boardRealLength = settings.BoardLength * settings.TileLength;

        screenXLimits = new Tuple<float, float>(0 - screenXMinMargin, boardRealWidth + screenXMaxMargin);
        screenZLimits = new Tuple<float, float>(0 - screenZMinMargin, boardRealLength + screenZMaxMargin);
    }
    
    private void UpdateTrail()
    {
        float zoomTrailHalfLength = zoomTrailLength / 2;
        minZoomLocalPosition = new Vector3(0, 0, -zoomTrailHalfLength);
        maxZoomLocalPosition = new Vector3(0, 0, zoomTrailHalfLength);
    }

    private void StartKeyModeMovement()
    {
        keyModeMovementCursorLockPosition = AllManagers.Instance.InputManager.CursorPosition;
        movementMode = Enums.CameraMovementMode.Key;
    }

    private void StopKeyModeMovement()
    {
        keyModeMovementCursorLockPosition = Vector2.zero;
        movementMode = Enums.CameraMovementMode.Border;
    }
    
    private void UpdateZoomTarget(float zoomInput)
    {
        zoomTargetNormalizedPosition = Mathf.Clamp01(zoomTargetNormalizedPosition + zoomInput * zoomStep);
        UpdateZoom();
    }

    private IEnumerator Perform2DMovement()
    {
        while (true)
        {
            if (!Application.isFocused) yield return null;

            Vector3 movementVector = Vector3.zero;

            switch (movementMode)
            {
                case Enums.CameraMovementMode.Border:
                    movementVector = CalculateMovementVectorBorderMode();
                    break;

                case Enums.CameraMovementMode.Key:
                    movementVector = CalculateMovementVectorKeyMode();
                    break;
            }

            Vector3 position = transform.position;
            position += movementVector;
            position.x = Mathf.Clamp(position.x, screenXLimits.Item1, screenXLimits.Item2);
            position.z = Mathf.Clamp(position.z, screenZLimits.Item1, screenZLimits.Item2);
            transform.position = position;
            
            yield return null;
        }
    }
    
    private void UpdateZoom(bool immediately = false)
    {
        coroutineCaller?.StopCoroutine(ref zoomingCoroutineId);
        
        if (immediately)
        {
            zoomNormalizedPosition = zoomTargetNormalizedPosition;
            cameraObject.transform.localPosition = Vector3.Lerp(minZoomLocalPosition, maxZoomLocalPosition, zoomNormalizedPosition);
        }
        else
        {
            zoomingCoroutineId = coroutineCaller.StartCoroutine(ZoomingCoroutine(zoomNormalizedPosition, zoomTargetNormalizedPosition));
        }
    }

    private IEnumerator ZoomingCoroutine(float from, float to)
    {
        float deltaTime = 0;
        while (true)
        {
            deltaTime += Time.deltaTime * zoomSpeed;
            zoomNormalizedPosition = Utility.EaseOutCubic(from, to, deltaTime);
            
            if (Mathf.Approximately(zoomNormalizedPosition, zoomTargetNormalizedPosition))
            {
                zoomNormalizedPosition = zoomTargetNormalizedPosition;
                break;
            }
            
            cameraObject.transform.localPosition = Vector3.Lerp(minZoomLocalPosition, maxZoomLocalPosition, zoomNormalizedPosition);
            yield return null;
        }
    }
    
    private Vector3 CalculateMovementVectorBorderMode()
    {
        Vector3 cursorMovement = Vector3.zero;
        Vector2 cursorPosition = AllManagers.Instance.InputManager.CursorPosition;

        if (cursorPosition.y >= Screen.height - screenBorderThickness)
        {
            cursorMovement.z += 1;
        }
        else if (cursorPosition.y <= screenBorderThickness)
        {
            cursorMovement.z -= 1;
        }

        if (cursorPosition.x >= Screen.width - screenBorderThickness)
        {
            cursorMovement.x += 1;
        }
        else if (cursorPosition.x <= screenBorderThickness)
        {
            cursorMovement.x -= 1;
        }
            
        return borderModeMovementSpeed * Time.deltaTime * cursorMovement.normalized;
    }

    private Vector3 CalculateMovementVectorKeyMode()
    {
        Vector2 currentCursorPosition = AllManagers.Instance.InputManager.CursorPosition;
        Vector2 cameraDisplacement = keyModeMovementSpeed * Time.deltaTime * (currentCursorPosition - keyModeMovementCursorLockPosition);
            
        return new Vector3(cameraDisplacement.x, 0, cameraDisplacement.y);
    }
}