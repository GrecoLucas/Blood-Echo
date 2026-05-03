using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioSource AudioFootsteps;
        public AudioSource LandingAudio;
        public AudioSource AudioFoley;
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDDodge;
        private int _animIDDodgeArmed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private float heavyAttackTimer = 0.0f;

        [Header("Stats")]
        [SerializeField] private Stamina _stamina;

        [Header("Dodge")]
        [SerializeField] private float DodgeDistance = 2.5f;
        [SerializeField] private float DodgeDuration = 0.35f;
        [SerializeField] private float DodgeStaminaCost = 20f;
        [SerializeField] private float DodgeCooldown = 0.6f;

        [Header("Attack")]
        [SerializeField] private float AttackFallbackDuration = 1.0f;

        [Header("Heavy Attack")]
        public float HeavyAttackCooldown;

        // Referência ao WeaponController
        private WeaponController _weaponController;

        private const float _threshold = 0.01f;
        private bool _hasAnimator;
        private bool _isDodging;
        private bool _isAttacking;
        private float _dodgeTimer;
        private float _attackFallbackTimer;
        private Vector3 _dodgeDirection;
        private float _nextDodgeTime;
        private Inventory _inventory;
        public bool IsInvincible => _isDodging;
        public float HeavyAttackTimer => heavyAttackTimer;
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Inventory>();
            if (_inventory == null)
            {
                Debug.LogError("ThirdPersonController: Inventory component not found on Player object.");
            }
            heavyAttackTimer = 0.0f;

        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _weaponController = GetComponent<WeaponController>();

#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            if (_stamina == null)
                _stamina = GetComponent<Stamina>();
            if (_stamina == null)
                _stamina = FindObjectOfType<Stamina>();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            HandleAttack();
            UpdateAttackState();
            HandleDodge();
            JumpAndGravity();
            GroundedCheck();
            Move();
            CooldownUpdate();
        }

        private void CooldownUpdate()
        {
            if (heavyAttackTimer > 0.0f)
            {
                heavyAttackTimer -= Time.fixedDeltaTime;
            }
        }
        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDDodge = Animator.StringToHash("Dodge");
            _animIDDodgeArmed = Animator.StringToHash("Dodge 0");
        }

        private void HandleDodge()
        {
            if (!_hasAnimator || _isDodging) return;
            if (Time.time < _nextDodgeTime) return;

            bool dodgePressed = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                dodgePressed = Keyboard.current.rKey.wasPressedThisFrame;
            }
#else
            dodgePressed = Input.GetKeyDown(KeyCode.R);
#endif

            if (dodgePressed)
            {
                if (_stamina != null && !_stamina.TryUseStamina(DodgeStaminaCost))
                {
                    return;
                }

                _dodgeDirection = GetDodgeDirection();
                int dodgeStateHash = (_weaponController != null && _weaponController.IsArmed)
                    ? _animIDDodgeArmed
                    : _animIDDodge;

                _animator.CrossFadeInFixedTime(dodgeStateHash, 0.05f, 0, 0.0f);
                _isDodging = true;
                _dodgeTimer = DodgeDuration;
                _nextDodgeTime = Time.time + DodgeCooldown;

                if (_dodgeDirection.sqrMagnitude > 0.0001f)
                {
                    transform.rotation = Quaternion.LookRotation(_dodgeDirection, Vector3.up);
                }
            }
        }

        private Vector3 GetDodgeDirection()
        {
            if (_input != null && _input.move.sqrMagnitude > 0.01f && _mainCamera != null)
            {
                Vector3 cameraForward = _mainCamera.transform.forward;
                Vector3 cameraRight = _mainCamera.transform.right;

                cameraForward.y = 0f;
                cameraRight.y = 0f;
                cameraForward.Normalize();
                cameraRight.Normalize();

                Vector3 moveDirection = cameraForward * _input.move.y + cameraRight * _input.move.x;
                if (moveDirection.sqrMagnitude > 0.0001f)
                {
                    return moveDirection.normalized;
                }
            }

            return transform.forward;
        }

        private void HandleAttack()
        {
            if (!_hasAnimator) return;

            // Só permite atacar se o WeaponController reportar que está armado
            if (_weaponController == null || !_weaponController.IsArmed) return;

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (heavyAttackTimer <= 0f && _inventory != null && _inventory.HasHeavyAttack && Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed)
                {
                    _weaponController.TriggerHeavyAttack();
                    heavyAttackTimer = HeavyAttackCooldown;
                } else {
                    _weaponController.TriggerAttack();
                }
                _isAttacking = true;
                _attackFallbackTimer = AttackFallbackDuration;
            }
#else
            if (Input.GetMouseButtonDown(0))
            {
                _weaponController.TriggerAttack();
                _isAttacking = true;
                _attackFallbackTimer = AttackFallbackDuration;
            }
#endif
        }

        private void UpdateAttackState()
        {
            if (!_isAttacking) return;

            _attackFallbackTimer -= Time.deltaTime;
            if (_attackFallbackTimer <= 0f)
            {
                _isAttacking = false;
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        public void SetCameraRotation(float yaw, float pitch)
        {
            _cinemachineTargetYaw = ClampAngle(yaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(pitch, BottomClamp, TopClamp);
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            if (_isDodging)
            {
                float dodgeSpeed = DodgeDistance / Mathf.Max(DodgeDuration, 0.0001f);
                Vector3 dodgeMotion = _dodgeDirection.normalized * (dodgeSpeed * Time.deltaTime) +
                                      new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;

                _controller.Move(dodgeMotion);

                _dodgeTimer -= Time.deltaTime;
                if (_dodgeTimer <= 0.0f)
                {
                    _isDodging = false;
                }

                if (_hasAnimator)
                {
                    _animator.SetFloat(_animIDSpeed, 0f);
                    _animator.SetFloat(_animIDMotionSpeed, 0f);
                }

                return;
            }

            // Trava movimento horizontal durante ataque para evitar deslizamento
            if (_isAttacking)
            {
                _controller.Move(new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
                return;
            }

            bool canSprint = _stamina == null || _stamina.HasStamina;
            float targetSpeed = (_input.sprint && canSprint) ? SprintSpeed : MoveSpeed;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (AudioFootsteps != null)
                    AudioFootsteps.Play();
                if (AudioFoley != null)
                    AudioFoley.Play();
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (LandingAudio != null)
                    LandingAudio.Play();
            }
        }
    
        public void AddSprintSpeedBonus(float bonus)
        {
            SprintSpeed += bonus;
        }

        // Chame este método via Animation Event no final de cada clip de ataque
        public void OnAttackEnd()
        {
            _isAttacking = false;
            _attackFallbackTimer = 0f;
            // Reseta sprint para evitar que fique preso
            if (_input != null)
                _input.sprint = false;
        }
    }


}