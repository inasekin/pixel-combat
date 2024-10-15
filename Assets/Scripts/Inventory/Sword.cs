using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    // Префаб анимации удара мечом
    [SerializeField] private GameObject slashAnimPrefab;
    
    // Точка, где будет создана анимация удара
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private float swordAttackCD = .5f;
    [SerializeField] private WeaponInfo weaponInfo;
    
    // Ссылка на аниматор для управления анимациями меча
    private Animator myAnimator;
    private Transform weaponCollider;
    // Ссылка на созданный объект анимации удара
    private GameObject slashAnim;

    // Метод Awake вызывается при инициализации объекта
    private void Awake()
    {
        // Получаем компонент Animator, который отвечает за воспроизведение анимаций
        myAnimator = GetComponent<Animator>();
    }
    
    private void Start() {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = GameObject.Find("SlashAnimSpawnPoint").transform;
    }

    // Метод Update вызывается каждый кадр
    private void Update()
    {
        // Обновляем положение меча в зависимости от положения мыши
        MouseFollowWithOffset();
    }
    
    public WeaponInfo GetWeaponInfo() {
        return weaponInfo;
    }

    // Метод, вызывающий анимацию атаки мечом
    public void Attack()
    {
        // Запускаем триггер анимации атаки
        myAnimator.SetTrigger("Attack");
        weaponCollider.gameObject.SetActive(true);
    
        // Создаем анимацию удара по заданной позиции и устанавливаем её родителем объект игрока
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;
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
        if (PlayerController.Instance.FacingLeft)
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
        if (PlayerController.Instance.FacingLeft)
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
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);
        
        // Вычисляем угол поворота в зависимости от положения мыши
        float angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;

        // Если курсор мыши находится слева от персонажа, переворачиваем оружие по оси Y
        if (mousePosition.x < playerScreenPoint.x)
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
