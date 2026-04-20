using UnityEngine;

/// <summary>
/// Подбираемый объект, пополняющий боезапас активного оружия.
/// </summary>
public class AmmoPack : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Находим активное оружие (false = ищем только в активных дочерних объектах)
            Gun activeGun = other.GetComponentInChildren<Gun>(false);
            if (activeGun != null)
            {
                activeGun.AddAmmo(ammoAmount);

                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}