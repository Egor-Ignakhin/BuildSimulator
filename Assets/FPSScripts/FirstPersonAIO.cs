using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[RequireComponent(typeof(CapsuleCollider)),RequireComponent(typeof(Rigidbody)),AddComponentMenu("First Person AIO")]

public class FirstPersonAIO : MonoBehaviour {

    #region Variables

    public bool controllerPauseState = false;

    #region Look Settings
    public bool enableCameraMovement = true;
    public enum InvertMouseInput{None,X,Y,Both}
    public InvertMouseInput mouseInputInversion = InvertMouseInput.None;
    public enum CameraInputMethod{Traditional}
    public CameraInputMethod cameraInputMethod = CameraInputMethod.Traditional;

    public float verticalRotationRange = 170;
    public float mouseSensitivity = 10;
    public  float   fOVToMouseSensitivity = 1;
    public float cameraSmoothing = 5f;
    public bool lockAndHideCursor = false;
    public Camera playerCamera;
    public bool enableCameraShake=false;
    internal Vector3 cameraStartingPosition;
    float baseCamFOV;

    public Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Vector3 originalRotation;
    #endregion

    #region Movement Settings
    public bool IsPause;
    public bool playerCanMove = true;
    public bool Sprint = false;
    public float walkSpeed = 4f;
    public KeyCode SprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 8f;
    public float jumpPower = 5f;
    public bool canJump = true;
    bool jumpInput;
    bool didJump;
    public float speed;
    internal float walkSpeedInternal;
    internal float sprintSpeedInternal;
    internal float jumpPowerInternal;

    [System.Serializable]
    public class CrouchModifiers {
        public bool useCrouch = true;
        public bool toggleCrouch = false;
        public KeyCode crouchKey = KeyCode.LeftControl;
        public float crouchWalkSpeedMultiplier = 0.5f;
        public float crouchJumpPowerMultiplier = 0f;
        public bool crouchOverride;
        internal float colliderHeight;
        
    }
    public CrouchModifiers _crouchModifiers = new CrouchModifiers();

    [System.Serializable]
    public class AdvancedSettings {
        public float gravityMultiplier = 1.0f;
        public PhysicMaterial zeroFrictionMaterial;
        public PhysicMaterial highFrictionMaterial;
        public float maxSlopeAngle = 55;
        internal bool isTouchingWalkable;
        internal bool isTouchingUpright;
        internal bool isTouchingFlat;
        public float maxWallShear = 89;
        public float maxStepHeight = 0.2f;
        internal bool stairMiniHop = false;
        public RaycastHit surfaceAngleCheck;
        public Vector3 curntGroundNormal;
        public Vector2 moveDirRef;
        public float lastKnownSlopeAngle;
        public float FOVKickAmount = 2.5f;
        public float changeTime = 0.75f;
        public float fovRef;
        
    }
    public AdvancedSettings advanced = new AdvancedSettings();
    private CapsuleCollider capsule;
    public bool IsGrounded { get; private set; }
    Vector2 inputXY;
    public bool isCrouching;
    float yVelocity;
    bool isSprinting = true;

    public Rigidbody fps_Rigidbody;

    #endregion

    #endregion

    private void Awake(){
        #region Look Settings - Awake
        
        originalRotation = transform.localRotation.eulerAngles;

        #endregion 

        #region Movement Settings - Awake
        walkSpeedInternal = walkSpeed;
        sprintSpeedInternal = sprintSpeed;
        jumpPowerInternal = jumpPower;
        capsule = GetComponent<CapsuleCollider>();
        IsGrounded = true;
        isCrouching = false;
        fps_Rigidbody = GetComponent<Rigidbody>();
        fps_Rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        fps_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _crouchModifiers.colliderHeight = capsule.height;
        #endregion

    }

    private void Start()
    {
        #region Look Settings - Start

        cameraStartingPosition = playerCamera.transform.localPosition;
        if (lockAndHideCursor) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        baseCamFOV = playerCamera.fieldOfView;
        #endregion

        #region Movement Settings - Start  
        capsule.radius = capsule.height / 4;
        advanced.zeroFrictionMaterial = new PhysicMaterial("Zero_Friction");
        advanced.zeroFrictionMaterial.dynamicFriction = 0;
        advanced.zeroFrictionMaterial.staticFriction = 0;
        advanced.zeroFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        advanced.zeroFrictionMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        advanced.highFrictionMaterial = new PhysicMaterial("Max_Friction");
        advanced.highFrictionMaterial.dynamicFriction = 1;
        advanced.highFrictionMaterial.staticFriction = 1;
        advanced.highFrictionMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        advanced.highFrictionMaterial.bounceCombine = PhysicMaterialCombine.Average;
        #endregion
    }

    private void Update()
    {
        if (IsPause)
            return;
        #region Look Settings - Update

        if (enableCameraMovement && !controllerPauseState)
        {
            float mouseYInput = 0;
            float mouseXInput = 0;

            float camFOV = playerCamera.fieldOfView;
            mouseYInput = mouseInputInversion == InvertMouseInput.None || mouseInputInversion == InvertMouseInput.X ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Mouse Y");
            mouseXInput = mouseInputInversion == InvertMouseInput.None || mouseInputInversion == InvertMouseInput.Y ? Input.GetAxis("Mouse X") : -Input.GetAxis("Mouse X");

            if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; } else if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
            if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x -= 360; } else if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }

            targetAngles.y += mouseXInput * (mouseSensitivity - ((baseCamFOV - camFOV) * fOVToMouseSensitivity) / 6f);
            if (cameraInputMethod == CameraInputMethod.Traditional)
                targetAngles.x += mouseYInput * (mouseSensitivity - ((baseCamFOV - camFOV) * fOVToMouseSensitivity) / 6f);
            else
                targetAngles.x = 0f;
            targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * verticalRotationRange, 0.5f * verticalRotationRange);
            followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, (cameraSmoothing) / 100);

            playerCamera.transform.localRotation = Quaternion.Euler(-followAngles.x + originalRotation.x, 0, 0);
            transform.localRotation = Quaternion.Euler(0, followAngles.y + originalRotation.y, 0);
        }

        #endregion

        #region Input Settings - Update
        if (Input.GetButtonDown("Jump") && canJump)
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
        if (IsPause)
            return;

        Vector3 MoveDirection = Vector3.zero;
        speed = Sprint ? isCrouching ? walkSpeedInternal : (isSprinting ? sprintSpeedInternal : walkSpeedInternal) : (isSprinting ? walkSpeedInternal : sprintSpeedInternal);


        if (advanced.maxSlopeAngle > 0)
        {
            if (advanced.isTouchingUpright && advanced.isTouchingWalkable)
            {

                MoveDirection = (transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal);
                if (!didJump) { fps_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation; }
            }
            else if (advanced.isTouchingUpright && !advanced.isTouchingWalkable)
            {
                fps_Rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
            }

            else
            {

                fps_Rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
                MoveDirection = ((transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal) * (fps_Rigidbody.velocity.y > 0.01f ? SlopeCheck() : 0.8f));
            }
        }
        else
        {
            MoveDirection = (transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal);
        }

        #region step logic


        if (advanced.maxStepHeight > 0 && Physics.Raycast(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - 0.01f, 0), MoveDirection, out RaycastHit WT, capsule.radius + 0.15f, Physics.AllLayers, QueryTriggerInteraction.Ignore) && Vector3.Angle(WT.normal, Vector3.up) > 88)
        {
            RaycastHit ST;
            if (!Physics.Raycast(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - (advanced.maxStepHeight), 0), MoveDirection, out ST, capsule.radius + 0.25f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                advanced.stairMiniHop = true;
                transform.position += new Vector3(0, advanced.maxStepHeight * 1.2f, 0);
            }
        }
        Debug.DrawRay(transform.position, MoveDirection, Color.red, 0, false);
        #endregion

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        inputXY = new Vector2(horizontalInput, verticalInput);
        if (inputXY.magnitude > 1) { inputXY.Normalize(); }

        #region Jump
        yVelocity = fps_Rigidbody.velocity.y;

        if (IsGrounded && jumpInput && jumpPowerInternal > 0 && !didJump)
        {
            if (advanced.maxSlopeAngle > 0)
            {
                if (advanced.isTouchingFlat || advanced.isTouchingWalkable)
                {
                    didJump = true;
                    jumpInput = false;
                    yVelocity += fps_Rigidbody.velocity.y < 0.01f ? jumpPowerInternal : jumpPowerInternal / 3;
                    advanced.isTouchingWalkable = false;
                    advanced.isTouchingFlat = false;
                    advanced.isTouchingUpright = false;
                    fps_Rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
                }

            }
            else
            {
                didJump = true;
                jumpInput = false;
                yVelocity += jumpPowerInternal;
            }

        }

        if (advanced.maxSlopeAngle > 0)
        {


            if (!didJump && advanced.lastKnownSlopeAngle > 5 && advanced.isTouchingWalkable)
            {
                yVelocity *= SlopeCheck() / 4;
            }
            if (advanced.isTouchingUpright && !advanced.isTouchingWalkable && !didJump)
            {
                yVelocity += Physics.gravity.y;
            }
        }

        #endregion

        if (playerCanMove && !controllerPauseState)
        {
            fps_Rigidbody.velocity = MoveDirection + (Vector3.up * yVelocity);

        }
        else  fps_Rigidbody.velocity = Vector3.zero; 

        if (inputXY.magnitude > 0 || !IsGrounded)
            capsule.sharedMaterial = advanced.zeroFrictionMaterial;        
        else 
            capsule.sharedMaterial = advanced.highFrictionMaterial; 

        fps_Rigidbody.AddForce(Physics.gravity * (advanced.gravityMultiplier - 1));


        if (advanced.FOVKickAmount > 0)
        {
            if (isSprinting && !isCrouching && playerCamera.fieldOfView != (baseCamFOV + (advanced.FOVKickAmount * 2) - 0.01f))
            {
                if (Mathf.Abs(fps_Rigidbody.velocity.x) > 0.5f || Mathf.Abs(fps_Rigidbody.velocity.z) > 0.5f)
                {
                    playerCamera.fieldOfView = Mathf.SmoothDamp(playerCamera.fieldOfView, baseCamFOV + (advanced.FOVKickAmount * 2), ref advanced.fovRef, advanced.changeTime);
                }

            }
            else if (playerCamera.fieldOfView != baseCamFOV) { playerCamera.fieldOfView = Mathf.SmoothDamp(playerCamera.fieldOfView, baseCamFOV, ref advanced.fovRef, advanced.changeTime * 0.5f); }

        }

        if (_crouchModifiers.useCrouch)
        {

            if (isCrouching)
            {
                capsule.height = Mathf.MoveTowards(capsule.height, _crouchModifiers.colliderHeight / 1.5f, 5 * Time.deltaTime);
                walkSpeedInternal = walkSpeed * _crouchModifiers.crouchWalkSpeedMultiplier;
                jumpPowerInternal = jumpPower * _crouchModifiers.crouchJumpPowerMultiplier;

            }
            else
            {
                capsule.height = Mathf.MoveTowards(capsule.height, _crouchModifiers.colliderHeight, 5 * Time.deltaTime);
                walkSpeedInternal = walkSpeed;
                sprintSpeedInternal = sprintSpeed;
                jumpPowerInternal = jumpPower;
            }
        }
        #endregion
        #region  Reset Checks

        IsGrounded = false;

        if (advanced.maxSlopeAngle > 0)
        {
            if (advanced.isTouchingFlat || advanced.isTouchingWalkable || advanced.isTouchingUpright) { didJump = false; }
            advanced.isTouchingWalkable = false;
            advanced.isTouchingUpright = false;
            advanced.isTouchingFlat = false;
        }
        #endregion
    }

 

    public IEnumerator CameraShake(float Duration, float Magnitude){
        float elapsed =0;
        while(elapsed<Duration && enableCameraShake){
            playerCamera.transform.localPosition =Vector3.MoveTowards(playerCamera.transform.localPosition, new Vector3(cameraStartingPosition.x+ Random.Range(-1,1)*Magnitude,cameraStartingPosition.y+Random.Range(-1,1)*Magnitude,cameraStartingPosition.z), Magnitude*2);
            yield return new WaitForSecondsRealtime(0.001f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.transform.localPosition = cameraStartingPosition;
    }

    public void RotateCamera(Vector2 Rotation, bool Snap){
        enableCameraMovement = !enableCameraMovement;
        if(Snap){followAngles = Rotation;targetAngles = Rotation;}else{targetAngles = Rotation;}
        enableCameraMovement = !enableCameraMovement;
    }



    float SlopeCheck(){
        
            advanced.lastKnownSlopeAngle = Mathf.MoveTowards(advanced.lastKnownSlopeAngle, Vector3.Angle(advanced.curntGroundNormal, Vector3.up),5f);
            
            return new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(advanced.maxSlopeAngle+15, 0f),new Keyframe(advanced.maxWallShear, 0.0f),new Keyframe(advanced.maxWallShear+0.1f, 1.0f),new Keyframe(90, 1.0f)){preWrapMode = WrapMode.Clamp, postWrapMode = WrapMode.ClampForever}.Evaluate(advanced.lastKnownSlopeAngle);
          
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
                    advanced.stairMiniHop = false;
                    if (didJump && a <= 70) { didJump = false; }
                }

                if (advanced.maxSlopeAngle > 0)
                {
                    if (a < 5.1f) { advanced.isTouchingFlat = true; advanced.isTouchingWalkable = true; }
                    else if (a < advanced.maxSlopeAngle + 0.1f) { advanced.isTouchingWalkable = true; /* IsGrounded = true; */}
                    else if (a < 90) { advanced.isTouchingUpright = true; }

                    advanced.curntGroundNormal = CollisionData.GetContact(i).normal;
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
                    advanced.stairMiniHop = false;
                }

                if (advanced.maxSlopeAngle > 0)
                {
                    if (a < 5.1f) { advanced.isTouchingFlat = true; advanced.isTouchingWalkable = true; }
                    else if (a < advanced.maxSlopeAngle + 0.1f) { advanced.isTouchingWalkable = true; /* IsGrounded = true; */}
                    else if (a < 90) { advanced.isTouchingUpright = true; }

                    advanced.curntGroundNormal = CollisionData.GetContact(i).normal;
                }
            }
        }
    }
    private void OnCollisionExit(Collision CollisionData)
    {
        IsGrounded = false;
        if (advanced.maxSlopeAngle > 0)
        {
            advanced.curntGroundNormal = Vector3.up;
            advanced.lastKnownSlopeAngle = 0;
            advanced.isTouchingWalkable = false;
            advanced.isTouchingUpright = false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonAIO)), InitializeOnLoadAttribute]
public class FPAIO_Editor : Editor
{

    FirstPersonAIO t;
    SerializedObject SerT;
    static bool showCrouchMods = false;

    void OnEnable()
    {

        t = (FirstPersonAIO)target;
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

        GUILayout.Label("First Person AIO", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        EditorGUILayout.Space();

        if (t.controllerPauseState) { GUILayout.Label("<b><color=#B40404>Controller Paused</color></b>", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, richText = true, fontSize = 16 }); }


        #region Movement Setup
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        t.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), t.playerCanMove);
        GUI.enabled = t.playerCanMove;
        t.Sprint = EditorGUILayout.ToggleLeft(new GUIContent("Sprint", "Determines if the default mode of movement is 'Walk' or 'Srpint'."), t.Sprint);
        t.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player walks."), t.walkSpeed, 0.1f, 10);
        t.SprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key needs to be pressed to enter a sprint"), t.SprintKey);
        t.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player sprints."), t.sprintSpeed, 0.1f, 20);
        t.canJump = EditorGUILayout.ToggleLeft(new GUIContent("Can Player Jump?", "Determines if the player is allowed to jump."), t.canJump);
        GUI.enabled = t.playerCanMove && t.canJump; EditorGUI.indentLevel++;
        t.jumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player can jump."), t.jumpPower, 0.1f, 15);
        EditorGUI.indentLevel--; GUI.enabled = t.playerCanMove;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        showCrouchMods = EditorGUILayout.BeginFoldoutHeaderGroup(showCrouchMods, new GUIContent("Crouch Modifiers", "Stat modifiers that will apply when player is crouching."));
        if (showCrouchMods)
        {
            t._crouchModifiers.useCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Coruch", "Determines if the player is allowed to crouch."), t._crouchModifiers.useCrouch);
            GUI.enabled = t.playerCanMove && t._crouchModifiers.useCrouch;
            t._crouchModifiers.crouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Crouch Key", "Determines what key needs to be pressed to crouch"), t._crouchModifiers.crouchKey);
            t._crouchModifiers.toggleCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Toggle Crouch?", "Determines if the crouching behaviour is on a toggle or momentary basis."), t._crouchModifiers.toggleCrouch);
            t._crouchModifiers.crouchWalkSpeedMultiplier = EditorGUILayout.Slider(new GUIContent("Crouch Movement Speed Multiplier", "Determines how fast the player can move while crouching."), t._crouchModifiers.crouchWalkSpeedMultiplier, 0.01f, 1.5f);
            t._crouchModifiers.crouchJumpPowerMultiplier = EditorGUILayout.Slider(new GUIContent("Crouching Jump Power Mult.", "Determines how much the player's jumping power is increased or reduced while crouching."), t._crouchModifiers.crouchJumpPowerMultiplier, 0, 1.5f);
            t._crouchModifiers.crouchOverride = EditorGUILayout.ToggleLeft(new GUIContent("Force Crouch Override", "A Toggle that will override the crouch key to force player to crouch."), t._crouchModifiers.crouchOverride);
        }
        GUI.enabled = t.playerCanMove;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
        GUI.enabled = t.playerCanMove;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();
        
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUI.enabled = true;
        EditorGUILayout.Space();
        #endregion
    }
    /*public static void DownloadImage(string url)
    {
        using (WebClient client = new WebClient())
        {
            byte[] data = client.DownloadData(url);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);

            adTex1 = tex;
        }
    }*/
}
#endif