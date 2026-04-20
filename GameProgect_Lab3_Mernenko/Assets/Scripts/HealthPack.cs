using UnityEngine;

/// <summary>
/// Подбираемый объект, восстанавливающий здоровье игрока.
/// </summary>
public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 25;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);

                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}