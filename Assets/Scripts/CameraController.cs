using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UpdateSystem.CoroutineSystem;

[Serializable]
public class CameraController
{
    [SerializeField] private float speed = 20;
    [SerializeField] private float keyModeMovementSpeed = 0.05f;
    [SerializeField] private float screenBorderThickness = 10;
    [SerializeField] private Vector2 screenXLimits; // Todo: Set limits depending on the game board's size.
    [SerializeField] private Vector2 screenZLimits;

    private Camera mainCamera;
    private Vector2 keyModeMovementCursorLockPosition;
    private CoroutineManager.CoroutineCaller coroutineCaller;
    private Enums.CameraMovementMode movementMode;
    private Guid movementCoroutineId;

    public void Initialize(Camera camera)
    {
        coroutineCaller = AllManagers.Instance.CoroutineManager.GenerateCoroutineCaller();
        mainCamera = camera;
            
        AllManagers.Instance.InputManager.SetOnPerformed
        (
            Enums.ActionMap.Global,
            Enums.InputAction.CameraMovement,
            StartKeyModeMovement
        );
            
        AllManagers.Instance.InputManager.SetOnCanceled
        (
            Enums.ActionMap.Global,
            Enums.InputAction.CameraMovement,
            StopKeyModeMovement
        );
            
        StartMovement();
    }

    public void Destroy()
    {
        AllManagers.Instance.InputManager.RemoveOnStarted
        (
            Enums.ActionMap.Global,
            Enums.InputAction.CameraMovement,
            StartKeyModeMovement
        );
            
        AllManagers.Instance.InputManager.RemoveOnCanceled
        (
            Enums.ActionMap.Global,
            Enums.InputAction.CameraMovement,
            StopKeyModeMovement
        );
            
        StopMovement();
    }

    private void StartKeyModeMovement(InputAction.CallbackContext context)
    {
        keyModeMovementCursorLockPosition = AllManagers.Instance.InputManager.CursorPosition;
        movementMode = Enums.CameraMovementMode.Key;
    }

    private void StopKeyModeMovement(InputAction.CallbackContext context)
    {
        keyModeMovementCursorLockPosition = Vector2.zero;
        movementMode = Enums.CameraMovementMode.Border;
    }

    private void StartMovement()
    {
        movementCoroutineId = coroutineCaller.StartCoroutine(PerformMovement());
    }

    private void StopMovement()
    {
        coroutineCaller.StopCoroutine(ref movementCoroutineId);
    }

    private IEnumerator<IWait> PerformMovement()
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
                
            Vector3 position = mainCamera.transform.position;
            position += movementVector;
            position.x = Mathf.Clamp(position.x, screenXLimits.x, screenXLimits.y);
            position.z = Mathf.Clamp(position.z, screenZLimits.x, screenZLimits.y);
            mainCamera.transform.position = position;
            
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
            
        return speed * Time.deltaTime * cursorMovement.normalized;
    }

    private Vector3 CalculateMovementVectorKeyMode()
    {
        Vector2 currentCursorPosition = AllManagers.Instance.InputManager.CursorPosition;
        Vector2 cameraDisplacement = keyModeMovementSpeed * Time.deltaTime * (currentCursorPosition - keyModeMovementCursorLockPosition);
            
        return new Vector3(cameraDisplacement.x, 0, cameraDisplacement.y);
    }
}