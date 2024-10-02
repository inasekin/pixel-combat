using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Компонент для управления игроком. Позволяет передвигать персонажа с помощью клавиатуры,
/// мыши (клики) и сенсорных экранов (тапы) на мобильных устройствах.
/// Также управляет анимациями и направлением взгляда персонажа.
/// </summary>
public class PlayerController : Singleton<PlayerController>
{
    // Создает геттер/сеттер для удобного изменения facingLeft
    public bool FacingLeft
    {
        get { return facingLeft; }
    }
    
    // Скорость перемещения игрока. Можно настроить в инспекторе Unity.
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    
    // Ссылка на сгенерированный класс управления вводом.
    private PlayerControls playerControls;

    // Вектор текущего движения, полученный из ввода с клавиатуры.
    private Vector2 movement;

    // Ссылка на компонент Rigidbody2D для физического перемещения персонажа.
    private Rigidbody2D rb;

    // Ссылка на компонент Animator для управления анимациями.
    private Animator myAnimator;

    // Ссылка на компонент SpriteRenderer для управления направлением персонажа.
    private SpriteRenderer mySpriteRenderer;

    // Целевая позиция, к которой должен двигаться игрок при клике или тапе.
    // Используем Nullable Vector2, чтобы определить, установлена ли цель.
    private Vector2? targetPosition = null;

    // Ссылка на основную камеру для преобразования экранных координат в мировые.
    private Camera mainCamera;
    
    // Повернут ли наш персонаж в другую сторону
    private bool facingLeft = false;
    private bool isDashing = false;
    private float startingMoveSpeed;
    

    /// <summary>
    /// Метод вызывается при инициализации объекта.
    /// Здесь происходит инициализация управления вводом, получение компонентов Rigidbody2D, Animator и SpriteRenderer,
    /// а также основной камеры.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // Создаем экземпляр класса управления вводом.
        playerControls = new PlayerControls();

        // Получаем компонент Rigidbody2D, прикрепленный к этому объекту.
        rb = GetComponent<Rigidbody2D>();

        // Получаем компонент Animator, прикрепленный к этому объекту.
        myAnimator = GetComponent<Animator>();

        // Получаем компонент SpriteRenderer, прикрепленный к этому объекту.
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        // Получаем основную камеру сцены.
        mainCamera = Camera.main;
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
        startingMoveSpeed = moveSpeed;
    }

    /// <summary>
    /// Метод вызывается при включении компонента.
    /// Включаем управление вводом.
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }

    /// <summary>
    /// Метод вызывается при отключении компонента.
    /// Отключаем управление вводом.
    /// </summary>
    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }

    /// <summary>
    /// Метод вызывается каждый кадр.
    /// Обрабатывает ввод с клавиатуры, мыши и сенсорного экрана.
    /// </summary>
    private void Update()
    {
        // Обрабатываем ввод с клавиатуры.
        PlayerInput();

        // Обрабатываем ввод с мыши и сенсорного экрана.
        HandlePointerInput();
    }

    /// <summary>
    /// Метод вызывается на фиксированных промежутках времени.
    /// Выполняет физическое перемещение персонажа и корректирует направление взгляда.
    /// </summary>
    private void FixedUpdate()
    {
        // Корректируем направление взгляда персонажа.
        AdjustPlayerFacingDirection();

        // Выполняем перемещение персонажа.
        Move();
    }

    /// <summary>
    /// Получает значение ввода с клавиатуры для движения.
    /// </summary>
    private void PlayerInput()
    {
        // Читаем значение вектора движения из системы ввода.
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        // В данном методе больше не обновляем параметры анимации.
        // Это будет делаться в методе Move().
    }

    /// <summary>
    /// Обрабатывает ввод с указателя (мышь) и сенсорного экрана.
    /// Устанавливает целевую позицию при клике или тапе и запускает анимацию атаки при левом клике.
    /// </summary>
    private void HandlePointerInput()
    {
        // Обработка правого клика мышью для перемещения.
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // Получаем позицию курсора мыши на экране.
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // Преобразуем экранные координаты в мировые координаты.
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePosition);

            // Устанавливаем целевую позицию (без учета оси Z).
            targetPosition = new Vector2(worldPos.x, worldPos.y);
        }
    }

    /// <summary>
    /// Перемещает персонажа либо в направлении ввода с клавиатуры,
    /// либо к целевой позиции, установленной кликом или тапом.
    /// Также обновляет параметры анимации и направление взгляда.
    /// </summary>
    private void Move()
    {
        Vector2 currentPosition = rb.position;
        Vector2 newMovement = Vector2.zero; // Временный вектор движения для анимации

        if (targetPosition.HasValue)
        {
            // Вычисляем направление к целевой позиции.
            Vector2 direction = (targetPosition.Value - currentPosition).normalized;

            // Перемещаем персонажа в направлении к цели с учетом скорости и времени.
            rb.MovePosition(currentPosition + direction * (moveSpeed * Time.fixedDeltaTime));

            // Обновляем параметры анимации на основе направления движения к цели.
            myAnimator.SetFloat("moveX", direction.x);
            myAnimator.SetFloat("moveY", direction.y);

            // Проверяем, достиг ли персонаж целевой позиции с небольшим порогом.
            if (Vector2.Distance(currentPosition, targetPosition.Value) < 0.1f)
            {
                // Если достиг, сбрасываем целевую позицию.
                targetPosition = null;
            }
        }
        else
        {
            // Перемещаемся по вводу с клавиатуры.
            rb.MovePosition(currentPosition + movement * (moveSpeed * Time.fixedDeltaTime));

            // Вычисляем скорость для анимации на основе ввода с клавиатуры.
            float speed = movement.sqrMagnitude;

            // Обновляем параметры анимации на основе ввода с клавиатуры.
            myAnimator.SetFloat("moveX", movement.x);
            myAnimator.SetFloat("moveY", movement.y);
        }
    }

    /// <summary>
    /// Корректирует направление взгляда персонажа в зависимости от позиции указателя мыши.
    /// Переворачивает спрайт персонажа по оси X, если указатель находится слева от персонажа.
    /// </summary>
    private void AdjustPlayerFacingDirection()
    {
        // Проверяем, что камера не null
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Получаем текущую позицию мыши на экране.
        Vector3 mousePos = Input.mousePosition;

        // Преобразуем позицию персонажа из мировых координат в экранные координаты.
        Vector3 playerScreenPoint = mainCamera.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            // Переворачиваем спрайт персонажа по оси X, если курсор слева.
            mySpriteRenderer.flipX = true;
            facingLeft = true;
        }
        else
        {
            // Сбрасываем переворот спрайта, если курсор справа.
            mySpriteRenderer.flipX = false;
            facingLeft = false;
        }
    }


    private void Dash()
    {
        if (!isDashing)
        {
            isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine()); 
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashDuration = .2f;
        float dashCD = .25f;
        yield return new WaitForSeconds(dashDuration);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }
}
