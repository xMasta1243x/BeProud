using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpInput { get; set; }

    private InputActionMap gameplayActionMap;

    [Header("Sensitivity Options")]
    public float userMouseSensitivity = 3.0f;
    public float userPlayerSensitivity= 3.0f;

    private const float mouseSensitivityMultiplier = 30f;
    private const float playerSensitivityMultiplier = 120f;

    public float mouseSensitivity => userMouseSensitivity * mouseSensitivityMultiplier;
    public float playerSensitivity => userPlayerSensitivity * playerSensitivityMultiplier;
    private void Awake()
    {
        var playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        gameplayActionMap = playerInput.actions.FindActionMap("Gameplay", true);

        if (gameplayActionMap == null)
        {
            Debug.LogError("ActionMap 'Gameplay' no encontrado.");
            return;
        }

        gameplayActionMap.Enable();
    }


    private void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        LookInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        JumpInput = value.isPressed && gameplayActionMap["Jump"].triggered;
    }

    private void LateUpdate()
    {
        JumpInput = false;
    }
}