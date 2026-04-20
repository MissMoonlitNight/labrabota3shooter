using UnityEngine;

/// <summary>
/// Базовое управление FPS-персонажем: перемещение, вращение камеры от мыши и корректная отдача.
/// Отдача теперь применяется как временное смещение и не накапливается в основном повороте.
/// </summary>
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Recoil Settings")]
    [Tooltip("Множитель силы отдачи. Подбирается экспериментально в Inspector.")]
    [SerializeField] private float recoilMultiplier = 0.12f;

    [Header("References")]
    [SerializeField] private Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float baseVerticalLook = 0f; // Хранит поворот ТОЛЬКО от мыши
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    /// <summary>
    /// Обработка вращения камеры и применение отдачи без накопления.
    /// </summary>
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 1. Поворот игрока по горизонтали
        transform.Rotate(Vector3.up * mouseX);

        // 2. Базовый поворот камеры по вертикали ТОЛЬКО от мыши
        baseVerticalLook -= mouseY;
        baseVerticalLook = Mathf.Clamp(baseVerticalLook, -maxLookAngle, maxLookAngle);

        // 3. Получаем текущее значение отдачи из активного оружия
        float recoilOffset = 0f;
        Gun activeGun = playerCamera.GetComponentInChildren<Gun>(false);
        if (activeGun != null)
        {
            recoilOffset = activeGun.GetCurrentRecoil() * recoilMultiplier;
        }

        // 4. Итоговый угол = мышь + отдача
        float finalVerticalLook = baseVerticalLook - recoilOffset;
        finalVerticalLook = Mathf.Clamp(finalVerticalLook, -maxLookAngle, maxLookAngle);

        // Применяем к камере
        playerCamera.localRotation = Quaternion.Euler(finalVerticalLook, 0f, 0f);
    }

    /// <summary>
    /// Перемещение персонажа (WASD + прыжок + гравитация).
    /// </summary>
    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Прижим к земле
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}