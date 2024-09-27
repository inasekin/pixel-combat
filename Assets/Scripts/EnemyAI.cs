using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс, управляющий поведением врага
public class EnemyAI : MonoBehaviour
{
    // Внутренний enum, представляющий состояния врага. Пока что только одно состояние — "Роуминг".
    private enum State
    {
        Roaming // Роуминг - состояние, при котором враг бродит по карте
    }
    
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
        // Запуск корутины, которая будет управлять поведением роуминга.
        StartCoroutine(RoamingRoutine());
    }

    // Корутина, которая отвечает за поведение врага в состоянии "Roaming".
    private IEnumerator RoamingRoutine()
    {
        // Пока враг находится в состоянии роуминга, он будет искать новую позицию для перемещения каждые 2 секунды.
        while (state == State.Roaming)
        {
            // Генерация новой случайной позиции для перемещения.
            Vector2 roamPosition = GetRoamingPosition();
            
            // Перемещение врага к сгенерированной позиции.
            enemyPathfinding.MoveTo(roamPosition);
            
            // Ждем 2 секунды перед следующим перемещением.
            yield return new WaitForSeconds(2f);
        }
    }

    // Метод для получения случайной позиции рядом с текущей.
    private Vector3 GetRoamingPosition()
    {
        // Генерация случайного направления для перемещения (в диапазоне от -1 до 1) и нормализация для создания единичного вектора.
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}