using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Vector2? targetPosition = null;
    private Camera mainCamera;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
        HandlePointerInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
    }

    private void HandlePointerInput()
    {
        // Обработка клика мышью
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePosition);
            targetPosition = worldPos;
        }

        // Обработка касания на мобильных устройствах
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(touchPosition);
            targetPosition = worldPos;
        }
    }

    private void Move()
    {
        if (targetPosition.HasValue)
        {
            Vector2 direction = (targetPosition.Value - rb.position).normalized;
            rb.MovePosition(rb.position + direction * (moveSpeed * Time.fixedDeltaTime));

            // Проверка, достиг ли персонаж целевой позиции
            if (Vector2.Distance(rb.position, targetPosition.Value) < 0.1f)
            {
                targetPosition = null;
            }
        }
        else
        {
            rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
        }
    }
}
