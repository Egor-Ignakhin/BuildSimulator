using UnityEngine;
using Assets;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(Rigidbody)), AddComponentMenu("First Person Controller")]

public sealed class FirstPersonController : Singleton<FirstPersonController> ,ILooking
{
    #region Variables

    #region Look Settings
    private bool EnableCameraMovement = true;//возможность вращать камеру

    public float VerticalRotationRange { get; set; } = 0f;
    public float HeadMaxY { get; set; }
    public float HeadMinY { get; set; }
    public int Sensitivity { get; set; } = 10;
    public float FOVToMouseSensitivity { get; private set; } = 1f;
    public float CameraSmoothing { get; private set; } = 5f;

    public Camera PlayerCamera { get; set; }

    private float baseCamFOV;

    private Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    internal Vector3 originalRotation;
    #endregion

    #region Movement Settings
    internal bool PlayerCanMove { get; set; } = true;
    internal bool Sprint { get; set; } = false;
    public float WalkSpeed { get; set; } = 4f;

    public KeyCode SprintKey = KeyCode.LeftShift;
    public float SprintSpeed { get; set; } = 8f;
    public float JumpPower { get; set; } = 5f;
    public bool CanJump { get; set; } = true;
    private bool jumpInput;
    private bool didJump;

    private float speed;
    internal float WalkSpeedInternal { get; set; }
    internal float SprintSpeedInternal { get; set; }
    internal float JumpPowerInternal { get; set; }

    [System.Serializable]
    public sealed class CrouchModifiers
    {
        public bool useCrouch = true;
        public bool toggleCrouch = false;
        public KeyCode crouchKey = KeyCode.LeftControl;
        public float crouchWalkSpeedMultiplier = 0.5f;
        public float crouchJumpPowerMultiplier = 0f;
        public bool crouchOverride;
        internal float colliderHeight;

    }
    public CrouchModifiers _crouchModifiers { get; set; } = new CrouchModifiers();

    [System.Serializable]
    public sealed class AdvancedSettings
    {
        public float GravityMultiplier { get; set; } = 1f;
        public PhysicMaterial ZeroFrictionMaterial { get; set; }
        public PhysicMaterial HighFrictionMaterial { get; set; }
        public float MaxSlopeAngle { get; set; } = 55f;
        internal bool IsTouchingWalkable { get; set; }
        internal bool IsTouchingUpright { get; set; }
        internal bool IsTouchingFlat { get; set; }
        public float MaxWallShear { get; set; } = 89f;
        public float MaxStepHeight { get; set; } = 0.2f;
        internal bool stairMiniHop = false;
        public Vector3 CurntGroundNormal { get; set; }
        public float LastKnownSlopeAngle { get; set; }
        public float FOVKickAmount { get; set; } = 2.5f;
        public float ChangeTime { get; set; } = 0.75f;
        public float fovRef;

    }
    public AdvancedSettings Advanced { get; set; } = new AdvancedSettings();
    private CapsuleCollider capsule;
    public bool IsGrounded { get; private set; }
    private Vector2 inputXY;
    private bool isCrouching;
    private float yVelocity;
    private bool isSprinting = true;

    private Rigidbody _fpsRigidbody;

    #endregion

    #endregion

    private void Awake()
    {
        #region Movement Settings - Awake
       
        JumpPowerInternal = JumpPower;
        capsule = GetComponent<CapsuleCollider>();
        IsGrounded = true;
        isCrouching = false;
        _fpsRigidbody = GetComponent<Rigidbody>();
        _fpsRigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        _fpsRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _crouchModifiers.colliderHeight = capsule.height;
        #endregion
    }

    private void OnEnable()
    {
        float value = Assets.AdvancedSettings.MovingSpeed;
        if (value == 0)
            this.WalkSpeed = 2;
        else
            this.WalkSpeed = value == 1 ? 4 : 8;
        SprintSpeed = WalkSpeed * 1.5f;
        WalkSpeedInternal = WalkSpeed;
        SprintSpeedInternal = SprintSpeed;
    }

    private void Start()
    {
        #region Look Settings - Start
        VerticalRotationRange = 2 * HeadMaxY + Mathf.Clamp(0, HeadMinY, 0);
        baseCamFOV = PlayerCamera.fieldOfView;
        #endregion

        #region Movement Settings - Start  
        capsule.radius = capsule.height / 4;
        Advanced.ZeroFrictionMaterial = new PhysicMaterial("Zero_Friction")
        {
            dynamicFriction = 0,
            staticFriction = 0,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Minimum
        };

        Advanced.HighFrictionMaterial = new PhysicMaterial("Max_Friction")
        {
            dynamicFriction = 1,
            staticFriction = 1,
            frictionCombine = PhysicMaterialCombine.Maximum,
            bounceCombine = PhysicMaterialCombine.Average
        };
        #endregion
    }

    private void Update()
    {
        if (GameMenu.ActiveGameMenu)
            return;
        #region Look Settings - Update

        if (EnableCameraMovement)
        {
            float camFOV = PlayerCamera.fieldOfView;
            float mouseYInput =  Input.GetAxis("Mouse Y");
            float mouseXInput =  Input.GetAxis("Mouse X");

            if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; } else if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
            if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x -= 360; } else if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }

            targetAngles.y += mouseXInput * (Sensitivity - ((baseCamFOV - camFOV) * FOVToMouseSensitivity) / 6f);//rotate camera

            targetAngles.x += mouseYInput * (Sensitivity - ((baseCamFOV - camFOV) * FOVToMouseSensitivity) / 6f);

            targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * VerticalRotationRange, 0.5f * VerticalRotationRange);
            followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, (CameraSmoothing) / 100);

            PlayerCamera.transform.localRotation = Quaternion.Euler(-followAngles.x + originalRotation.x, 0, 0);
            transform.localRotation = Quaternion.Euler(0, followAngles.y + originalRotation.y, 0);
        }

        #endregion

        #region Input Settings - Update
        if (Input.GetButtonDown("Jump") && CanJump)
            jumpInput = true;
        else if (Input.GetButtonUp("Jump"))
            jumpInput = false;


        if (_crouchModifiers.useCrouch)
        {
            if (!_crouchModifiers.toggleCrouch)
                isCrouching = _crouchModifiers.crouchOverride || Input.GetKey(_crouchModifiers.crouchKey);
            else if (Input.GetKeyDown(_crouchModifiers.crouchKey))
                isCrouching = !isCrouching || _crouchModifiers.crouchOverride;
        }

        if (Input.GetKey(SprintKey))
            Sprint = true;
        else
            Sprint = false;
        #endregion
    }

    private void FixedUpdate()
    {
        #region Movement Settings - FixedUpdate
        if (GameMenu.ActiveGameMenu)
            return;

        Vector3 MoveDirection = Vector3.zero;
        speed = Sprint ? isCrouching ? WalkSpeedInternal : (isSprinting ? SprintSpeedInternal : WalkSpeedInternal) : (isSprinting ? WalkSpeedInternal : SprintSpeedInternal);

        if (Advanced.MaxSlopeAngle > 0)
        {
            if (Advanced.IsTouchingUpright && Advanced.IsTouchingWalkable)
            {
                MoveDirection = transform.forward * inputXY.y * speed + transform.right * inputXY.x * WalkSpeedInternal;
                if (!didJump) 
                    _fpsRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation; 
            }
            else if (Advanced.IsTouchingUpright && !Advanced.IsTouchingWalkable)
            {
                _fpsRigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
            }

            else
            {

                _fpsRigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
                MoveDirection = (transform.forward * inputXY.y * speed + transform.right * inputXY.x * WalkSpeedInternal) * (_fpsRigidbody.velocity.y > 0.01f ? SlopeCheck() : 0.8f);
            }
        }
        else
        {
            MoveDirection = transform.forward * inputXY.y * speed + transform.right * inputXY.x * WalkSpeedInternal;
        }

        #region step logic
        // здесь немного получше стало
        if (Advanced.MaxStepHeight > 0 && Physics.Raycast(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - 0.01f, 0), MoveDirection, out RaycastHit WT, capsule.radius - 3, Physics.AllLayers, QueryTriggerInteraction.Ignore) && Vector3.Angle(WT.normal, Vector3.up) > 88)
        {
          //  Debug.Log("drawRay");
            if (!Physics.Raycast(transform.position - new Vector3(0, (capsule.height / 2 * transform.localScale.y) - Advanced.MaxStepHeight, 0), MoveDirection, out RaycastHit ST, capsule.radius + 0.25f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                Advanced.stairMiniHop = true;
                transform.position += new Vector3(0, Advanced.MaxStepHeight * 1.2f, 0);
            }
        }
     //   Debug.DrawRay(transform.position, MoveDirection, Color.red, 0, false);
        #endregion

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        inputXY = new Vector2(horizontalInput, verticalInput);
        if (inputXY.magnitude > 1) 
            inputXY.Normalize(); 

        #region Jump
        yVelocity = _fpsRigidbody.velocity.y;

        if (IsGrounded && jumpInput && JumpPowerInternal > 0 && !didJump)
        {
            if (Advanced.MaxSlopeAngle > 0)
            {
                if (Advanced.IsTouchingFlat || Advanced.IsTouchingWalkable)
                {
                    didJump = true;
                    jumpInput = false;
                    yVelocity += _fpsRigidbody.velocity.y < 0.01f ? JumpPowerInternal : JumpPowerInternal / 3;
                    Advanced.IsTouchingWalkable = false;
                    Advanced.IsTouchingFlat = false;
                    Advanced.IsTouchingUpright = false;
                    _fpsRigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
                }

            }
            else
            {
                didJump = true;
                jumpInput = false;
                yVelocity += JumpPowerInternal;
            }

        }

        if (Advanced.MaxSlopeAngle > 0)
        {
            if (!didJump && Advanced.LastKnownSlopeAngle > 5 && Advanced.IsTouchingWalkable)
            {
                yVelocity *= SlopeCheck() / 4;
            }
          /*  if (Advanced.IsTouchingUpright && !Advanced.IsTouchingWalkable && !didJump)
            {
                // yVelocity += Physics.gravity.y;  //  этот баг не давал запрыгивать на блоки и платформы стоящие близко от точки прыжка
            }*/
        }

        #endregion

        if (PlayerCanMove)
        {
            _fpsRigidbody.velocity = MoveDirection + (Vector3.up * yVelocity);

        }
        else _fpsRigidbody.velocity = Vector3.zero;

        if (inputXY.magnitude > 0 || !IsGrounded)
            capsule.sharedMaterial = Advanced.ZeroFrictionMaterial;
        else
            capsule.sharedMaterial = Advanced.HighFrictionMaterial;

        _fpsRigidbody.AddForce(Physics.gravity * (Advanced.GravityMultiplier - 1));


        if (Advanced.FOVKickAmount > 0)
        {
            if (isSprinting && !isCrouching && PlayerCamera.fieldOfView != (baseCamFOV + (Advanced.FOVKickAmount * 2) - 0.01f))
            {
                if (Mathf.Abs(_fpsRigidbody.velocity.x) > 0.5f || Mathf.Abs(_fpsRigidbody.velocity.z) > 0.5f)
                {
                    PlayerCamera.fieldOfView = Mathf.SmoothDamp(PlayerCamera.fieldOfView, baseCamFOV + (Advanced.FOVKickAmount * 2), ref Advanced.fovRef, Advanced.ChangeTime);
                }

            }
            else if (PlayerCamera.fieldOfView != baseCamFOV)
                PlayerCamera.fieldOfView = Mathf.SmoothDamp(PlayerCamera.fieldOfView, baseCamFOV, ref Advanced.fovRef, Advanced.ChangeTime * 0.5f);
        }

        if (_crouchModifiers.useCrouch)
        {
            if (isCrouching)
            {
                capsule.height = Mathf.MoveTowards(capsule.height, _crouchModifiers.colliderHeight / 1.5f, 5 * Time.deltaTime);
                WalkSpeedInternal = WalkSpeed * _crouchModifiers.crouchWalkSpeedMultiplier;
                JumpPowerInternal = JumpPower * _crouchModifiers.crouchJumpPowerMultiplier;

            }
            else
            {
                capsule.height = Mathf.MoveTowards(capsule.height, _crouchModifiers.colliderHeight, 5 * Time.deltaTime);
                WalkSpeedInternal = WalkSpeed;
                SprintSpeedInternal = SprintSpeed;
                JumpPowerInternal = JumpPower;
            }
        }
        #endregion
        #region  Reset Checks

        IsGrounded = false;

        if (Advanced.MaxSlopeAngle > 0)
        {
            if (Advanced.IsTouchingFlat || Advanced.IsTouchingWalkable || Advanced.IsTouchingUpright)
                didJump = false;
            Advanced.IsTouchingWalkable = false;
            Advanced.IsTouchingUpright = false;
            Advanced.IsTouchingFlat = false;
        }
        #endregion
    }


    float SlopeCheck()
    {
        Advanced.LastKnownSlopeAngle = Mathf.MoveTowards(Advanced.LastKnownSlopeAngle, Vector3.Angle(Advanced.CurntGroundNormal, Vector3.up), 5f);

        return new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(Advanced.MaxSlopeAngle + 15, 0f), new Keyframe(Advanced.MaxWallShear, 0.0f), new Keyframe(Advanced.MaxWallShear + 0.1f, 1.0f), new Keyframe(90, 1.0f)) { preWrapMode = WrapMode.Clamp, postWrapMode = WrapMode.ClampForever }.Evaluate(Advanced.LastKnownSlopeAngle);
    }

    private void OnCollisionEnter(Collision CollisionData)
    {
        for (int i = 0; i < CollisionData.contactCount; i++)
        {
            float a = Vector3.Angle(CollisionData.GetContact(i).normal, Vector3.up);
            if (CollisionData.GetContact(i).point.y < transform.position.y - ((capsule.height / 2) - capsule.radius * 0.95f))
            {

                if (!IsGrounded)
                {
                    IsGrounded = true;
                    Advanced.stairMiniHop = false;
                    if (didJump && a <= 70) { didJump = false; }
                }

                if (Advanced.MaxSlopeAngle > 0)
                {
                    if (a < 5.1f)
                    {
                        Advanced.IsTouchingFlat = true;
                        Advanced.IsTouchingWalkable = true;
                    }
                    else if (a < Advanced.MaxSlopeAngle + 0.1f)
                        Advanced.IsTouchingWalkable = true;
                    else if (a < 90)
                        Advanced.IsTouchingUpright = true;

                    Advanced.CurntGroundNormal = CollisionData.GetContact(i).normal;
                }
            }
        }
    }
    private void OnCollisionStay(Collision CollisionData)
    {
        for (int i = 0; i < CollisionData.contactCount; i++)
        {
            float a = Vector3.Angle(CollisionData.GetContact(i).normal, Vector3.up);
            if (CollisionData.GetContact(i).point.y < transform.position.y - ((capsule.height / 2) - capsule.radius * 0.95f))
            {
                if (!IsGrounded)
                {
                    IsGrounded = true;
                    Advanced.stairMiniHop = false;
                }

                if (Advanced.MaxSlopeAngle > 0)
                {
                    if (a < 5.1f)
                    {
                        Advanced.IsTouchingFlat = true;
                        Advanced.IsTouchingWalkable = true;
                    }
                    else if (a < Advanced.MaxSlopeAngle + 0.1f)
                        Advanced.IsTouchingWalkable = true;
                    else if (a < 90)
                        Advanced.IsTouchingUpright = true;

                    Advanced.CurntGroundNormal = CollisionData.GetContact(i).normal;
                }
            }
        }
    }
    private void OnCollisionExit(Collision CollisionData)
    {
        IsGrounded = false;
        if (Advanced.MaxSlopeAngle > 0)
        {
            Advanced.CurntGroundNormal = Vector3.up;
            Advanced.LastKnownSlopeAngle = 0;
            Advanced.IsTouchingWalkable = false;
            Advanced.IsTouchingUpright = false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonController)), InitializeOnLoad]
public sealed class FirstPersonController_Editor : Editor
{
    FirstPersonController t;
    SerializedObject SerT;
    private static bool showCrouchMods = false;

    private void OnEnable()
    {
        t = (FirstPersonController)target;
        SerT = new SerializedObject(t);
    }
    public override void OnInspectorGUI()
    {
        if (t.transform.localScale != Vector3.one)
        {
            t.transform.localScale = Vector3.one;
            Debug.LogWarning("Scale needs to be (1,1,1)! \n Please scale controller via Capsule collider height/raduis.");
        }
        SerT.Update();
        EditorGUILayout.Space();

        GUILayout.Label("First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        EditorGUILayout.Space();


        #region Movement Setup
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        t.PlayerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), t.PlayerCanMove);
        GUI.enabled = t.PlayerCanMove;
        t.Sprint = EditorGUILayout.ToggleLeft(new GUIContent("Sprint", "Determines if the default mode of movement is 'Walk' or 'Srpint'."), t.Sprint);
        t.SprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key needs to be pressed to enter a sprint"), t.SprintKey);
        t.CanJump = EditorGUILayout.ToggleLeft(new GUIContent("Can Player Jump?", "Determines if the player is allowed to jump."), t.CanJump);
        GUI.enabled = t.PlayerCanMove && t.CanJump; EditorGUI.indentLevel++;
        t.JumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player can jump."), t.JumpPower, 0.1f, 15);
        EditorGUI.indentLevel--; GUI.enabled = t.PlayerCanMove;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        showCrouchMods = EditorGUILayout.BeginFoldoutHeaderGroup(showCrouchMods, new GUIContent("Crouch Modifiers", "Stat modifiers that will apply when player is crouching."));
        if (showCrouchMods)
        {
            t._crouchModifiers.useCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Coruch", "Determines if the player is allowed to crouch."), t._crouchModifiers.useCrouch);
            GUI.enabled = t.PlayerCanMove && t._crouchModifiers.useCrouch;
            t._crouchModifiers.crouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Crouch Key", "Determines what key needs to be pressed to crouch"), t._crouchModifiers.crouchKey);
            t._crouchModifiers.toggleCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Toggle Crouch?", "Determines if the crouching behaviour is on a toggle or momentary basis."), t._crouchModifiers.toggleCrouch);
            t._crouchModifiers.crouchWalkSpeedMultiplier = EditorGUILayout.Slider(new GUIContent("Crouch Movement Speed Multiplier", "Determines how fast the player can move while crouching."), t._crouchModifiers.crouchWalkSpeedMultiplier, 0.01f, 1.5f);
            t._crouchModifiers.crouchJumpPowerMultiplier = EditorGUILayout.Slider(new GUIContent("Crouching Jump Power Mult.", "Determines how much the player's jumping power is increased or reduced while crouching."), t._crouchModifiers.crouchJumpPowerMultiplier, 0, 1.5f);
            t._crouchModifiers.crouchOverride = EditorGUILayout.ToggleLeft(new GUIContent("Force Crouch Override", "A Toggle that will override the crouch key to force player to crouch."), t._crouchModifiers.crouchOverride);
        }
        GUI.enabled = t.PlayerCanMove;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
        GUI.enabled = t.PlayerCanMove;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
        
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUI.enabled = true;
        EditorGUILayout.Space();
        #endregion
    }
}
#endif