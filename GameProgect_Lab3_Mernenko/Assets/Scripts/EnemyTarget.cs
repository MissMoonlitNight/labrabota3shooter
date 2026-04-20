using UnityEngine;

/// <summary>
/// Простой враг-мишень, получающий урон и уничтожающийся при смерти.
/// </summary>
public class EnemyTarget : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    [SerializeField] private GameObject deathEffect; // Опционально: эффект при смерти

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}