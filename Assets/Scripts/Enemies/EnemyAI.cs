using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, управляющий поведением врага
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float roamChangeDirFloat = 2f;
    [SerializeField] private float attackRange = 0f;
    [SerializeField] private MonoBehaviour enemyType;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool stopMovingWhileAttacking = false;

    private bool canAttack = true;
    
    // Внутренний enum, представляющий состояния врага. Пока что только одно состояние — "Роуминг".
    private enum State
    {
        Roaming, // Роуминг - состояние, при котором враг бродит по карте
        Attacking
    }
    
    private Vector2 roamPosition;
    private float timeRoaming = 0f;
    
    // Переменная, хранящая текущее состояние врага.
    private State state;
    
    // Ссылка на компонент EnemyPathfinding, который отвечает за передвижение врага.
    private EnemyPathfinding enemyPathfinding;

    // Метод Awake вызывается при инициализации объекта. Здесь устанавливаются ссылки на необходимые компоненты и состояние врага.
    private void Awake()
    {
        // Получаем компонент EnemyPathfinding, который должен быть прикреплен к этому же объекту.
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        
        // Устанавливаем начальное состояние врага в "Roaming".
        state = State.Roaming;
    }

    // Метод Start вызывается один раз при старте скрипта. Запускается корутина для управления роумингом врага.
    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }
    
    private void Update() {
        MovementStateControl();
    }

    private void MovementStateControl() {
        switch (state)
        {
            default:
            case State.Roaming:
                Roaming();
                break;

            case State.Attacking:
                Attacking();
                break;
        }
    }
    
    private void Roaming() {
        timeRoaming += Time.deltaTime;

        enemyPathfinding.MoveTo(roamPosition);

        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange) {
            state = State.Attacking;
        }

        if (timeRoaming > roamChangeDirFloat) {
            roamPosition = GetRoamingPosition();
        }
    }

    private void Attacking() {
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > attackRange)
        {
            state = State.Roaming;
        }

        if (attackRange != 0 && canAttack) {
            canAttack = false;
            (enemyType as IEnemy)?.Attack();

            if (stopMovingWhileAttacking) {
                enemyPathfinding.StopMoving();
            } else {
                enemyPathfinding.MoveTo(roamPosition);
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Метод для получения случайной позиции рядом с текущей.
    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;
        // Генерация случайного направления для перемещения (в диапазоне от -1 до 1) и нормализация для создания единичного вектора.
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}