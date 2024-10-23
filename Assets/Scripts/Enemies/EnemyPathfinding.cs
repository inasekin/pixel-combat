using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, управляющий передвижением врага по карте с помощью физического движка.
public class EnemyPathfinding : MonoBehaviour
{
    // Скорость передвижения врага. Может быть установлена через Unity Editor.
    [SerializeField] private float moveSpeed = 2f;
    
    // Ссылка на компонент Rigidbody2D, который управляет физическим телом объекта.
    private Rigidbody2D rb2d;
    
    // Направление, в котором враг должен двигаться.
    private Vector2 moveDir;
    private Knockback knockback;
    private SpriteRenderer spriteRenderer;

    // Метод Awake вызывается при инициализации объекта. Здесь мы получаем ссылку на компонент Rigidbody2D.
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
        // Получение компонента Rigidbody2D, который отвечает за физическое перемещение объекта.
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Метод FixedUpdate вызывается на каждом фиксированном кадре физики. Здесь мы перемещаем врага на основе его направления движения.
    private void FixedUpdate()
    {
        if (knockback.GettingKnockedBack)
        {
            return;
        }
        // Перемещаем объект, добавляя к его текущей позиции направление движения, умноженное на скорость и время.
        rb2d.MovePosition(rb2d.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
        
        if (moveDir.x < 0) {
            spriteRenderer.flipX = true;
        } else {
            spriteRenderer.flipX = false;
        }
    }

    // Метод для задания цели передвижения (направления движения). Этот метод вызывается AI для установки новой позиции.
    public void MoveTo(Vector2 targetPosition)
    {
        // Устанавливаем новое направление движения.
        moveDir = targetPosition;
    }
}