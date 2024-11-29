using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    private List<GameObject> enemiesInRange = new List<GameObject>();

    public IEnumerable<GameObject> GetClosestEnemies() => enemiesInRange = new List<GameObject>(enemiesInRange.OrderBy(x => Vector3.SqrMagnitude(x.transform.position)));

    public bool HasTargets => enemiesInRange.Count > 0;
    public GameObject GetClosestEnemy()
    {
        GameObject closestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            float distanceSqr = (transform.position - enemy.transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public List<GameObject> GetEnemiesInRange()
    {
        return enemiesInRange.Where(enemy => enemy != null).ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

}

