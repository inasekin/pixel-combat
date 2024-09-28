using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // Префаб анимации удара мечом
    [SerializeField] private GameObject slashAnimPrefab;
    
    // Точка, где будет создана анимация удара
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private Transform weaponCollider;
    [SerializeField] private float swordAttackCD = .5f;
    
    // Ссылка на управление игроком
    private PlayerControls playerControls;
    
    // Ссылка на аниматор для управления анимациями меча
    private Animator myAnimator;
    
    // Ссылка на контроллер игрока для получения информации о его положении и состоянии
    private PlayerController playerController;
    
    // Ссылка на активное оружие игрока для управления его направлением
    private ActiveWeapon activeWeapon;
    
    // Ссылка на созданный объект анимации удара
    private GameObject slashAnim;
    private bool attackButtonDown, isAttacking = false;

    // Метод Awake вызывается при инициализации объекта
    private void Awake()
    {
        // Получаем ссылку на компонент PlayerController, который находится на родительском объекте
        playerController = GetComponentInParent<PlayerController>();
        
        // Получаем ссылку на компонент ActiveWeapon, который находится на родительском объекте
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        
        // Получаем компонент Animator, который отвечает за воспроизведение анимаций
        myAnimator = GetComponent<Animator>();
        
        // Инициализируем управление игроком
        playerControls = new PlayerControls();
    }

    // Включаем управление игроком при активации меча
    private void OnEnable()
    {
        playerControls.Enable();
    }

    // Метод Start вызывается один раз при старте объекта
    void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();
    }

    // Метод Update вызывается каждый кадр
    private void Update()
    {
        // Обновляем положение меча в зависимости от положения мыши
        MouseFollowWithOffset();
        Attack();
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    // Метод, вызывающий анимацию атаки мечом
    public void Attack()
    {
        if (attackButtonDown && !isAttacking)
        {
            isAttacking = true;
            // Запускаем триггер анимации атаки
            myAnimator.SetTrigger("Attack");
            weaponCollider.gameObject.SetActive(true);
        
            // Создаем анимацию удара по заданной позиции и устанавливаем её родителем объект игрока
            slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
            slashAnim.transform.parent = this.transform.parent;
            StartCoroutine(AttackCDRoutine());
        }
    }

    private IEnumerator AttackCDRoutine()
    {
        yield return new WaitForSeconds(swordAttackCD);
        isAttacking = false;
    }

    public void DoneAttackingAnimEvent()
    {
        weaponCollider.gameObject.SetActive(false);
    }

    // Метод для корректировки анимации удара вверх (если персонаж смотрит влево)
    public void SwingUpFlipAnimEvent()
    {
        // Разворачиваем анимацию удара вверх
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        // Если персонаж смотрит влево, переворачиваем анимацию по оси X
        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    
    // Метод для корректировки анимации удара вниз (если персонаж смотрит влево)
    public void SwingDownFlipAnimEvent()
    {
        // Разворачиваем анимацию удара вниз
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Если персонаж смотрит влево, переворачиваем анимацию по оси X
        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    // Метод для корректировки положения меча в зависимости от положения мыши
    private void MouseFollowWithOffset()
    {
        // Получаем текущую позицию мыши
        Vector3 mousePosition = Input.mousePosition;
        
        // Преобразуем позицию игрока в экранные координаты
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);
        
        // Вычисляем угол поворота в зависимости от положения мыши
        float angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;

        // Если курсор мыши находится слева от персонажа, переворачиваем оружие по оси Y
        if (mousePosition.x < playerScreenPoint.x)
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
