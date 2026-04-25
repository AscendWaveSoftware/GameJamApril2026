using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Player.PlayerGold))]
[RequireComponent(typeof(Player.PlayerEquipmentHandler))]

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float m_acceleration = 2.5f;
    [SerializeField] private float m_moveSpeed = 2.5f;

    Rigidbody m_rb;
    [SerializeField] private bool m_characterSide;
    private SpriteRenderer m_spriteRenderer;
    private Vector2 m_Movement;

    public bool IsMoving => m_Movement.sqrMagnitude > 0.01f;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_rb.useGravity = false;
        m_rb.constraints = RigidbodyConstraints.FreezeRotationX |
                            RigidbodyConstraints.FreezeRotationY |
                            RigidbodyConstraints.FreezeRotationZ |
                            RigidbodyConstraints.FreezePositionY;
    }

    private void Update()
    {
        m_spriteRenderer.flipX = m_characterSide;
    }

    private void FixedUpdate() => Movement();
    public void OnMove(InputAction.CallbackContext _context)
    {
        m_Movement = _context.ReadValue<Vector2>();

        if (m_Movement.x > 0)
            m_characterSide = false;
        else if (m_Movement.x < 0)
            m_characterSide = true;
    }


    private void Movement()
    {
        Vector3 localInput = new Vector3(m_Movement.x, 0, m_Movement.y).normalized;
        Vector3 targetSpeed = transform.TransformDirection(localInput) * m_moveSpeed;
        Vector3 velocityXZ = new Vector3(m_rb.linearVelocity.x, 0, m_rb.linearVelocity.z);
        Vector3 speedDif = targetSpeed - velocityXZ;

        float accelRate = (targetSpeed.magnitude > 0.01f) ? 5 : m_acceleration;

        Vector3 movement = new Vector3(
            Mathf.Pow(Mathf.Abs(speedDif.x) * accelRate, 1) * Mathf.Sign(speedDif.x),
            0,
            Mathf.Pow(Mathf.Abs(speedDif.z) * accelRate, 1) * Mathf.Sign(speedDif.z)
        );
        m_rb.AddForce(movement, ForceMode.Force);
    }
}
