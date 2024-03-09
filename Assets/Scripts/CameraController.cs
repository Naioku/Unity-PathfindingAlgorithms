﻿using System;
using System.Collections;
using CustomInputSystem;
using UnityEngine;
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
        InputManager inputManager = AllManagers.Instance.InputManager;
        mainCamera = camera;

        inputManager.GlobalMap.OnCameraMovementData.Performed += StartKeyModeMovement;
        inputManager.GlobalMap.OnCameraMovementData.Canceled += StopKeyModeMovement;
        StartMovement();
    }

    public void Destroy()
    {
        InputManager inputManager = AllManagers.Instance.InputManager;
        inputManager.GlobalMap.OnCameraMovementData.Performed -= StartKeyModeMovement;
        inputManager.GlobalMap.OnCameraMovementData.Canceled -= StopKeyModeMovement;
        StopMovement();
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

    private void StartMovement()
    {
        movementCoroutineId = coroutineCaller.StartCoroutine(PerformMovement());
    }

    private void StopMovement()
    {
        coroutineCaller.StopCoroutine(ref movementCoroutineId);
    }

    private IEnumerator PerformMovement()
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