using UnityEngine;

/// <summary>
/// Обрабатывает переключение между несколькими единицами оружия.
/// Ожидается, что все оружия находятся в одном родительском объекте (WeaponHolder) 
/// и изначально активен только один.
/// </summary>
public class WeaponSwitcher : MonoBehaviour
{
    [Tooltip("Массив всех доступных оружий (дочерние объекты WeaponHolder).")]
    [SerializeField] private GameObject[] weapons;

    [Tooltip("Клавиши для переключения (по умолчанию 1, 2...). Можно также использовать колёсико мыши.")]
    [SerializeField] private KeyCode[] switchKeys;

    private int currentWeaponIndex = 0;

    private void Start()
    {
        // Убедимся, что активен только первый индекс, остальные выключены
        SelectWeapon(currentWeaponIndex);
    }

    private void Update()
    {
        // Переключение по цифровым клавишам
        for (int i = 0; i < switchKeys.Length; i++)
        {
            if (Input.GetKeyDown(switchKeys[i]) && i < weapons.Length)
            {
                SelectWeapon(i);
                break;
            }
        }

        // Дополнительно: переключение колёсиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
                SelectWeapon((currentWeaponIndex + 1) % weapons.Length);
            else
                SelectWeapon((currentWeaponIndex - 1 + weapons.Length) % weapons.Length);
        }
    }

    /// <summary>
    /// Активирует оружие по индексу и деактивирует все остальные.
    /// </summary>
    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        if (index == currentWeaponIndex) return;

        // Деактивируем текущее
        weapons[currentWeaponIndex].SetActive(false);
        // Активируем новое
        weapons[index].SetActive(true);

        currentWeaponIndex = index;

        // Здесь в будущем можно добавить логику сброса отдачи, 
        // анимации переключения и звуков смены оружия.
    }
}