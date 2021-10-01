using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Transform references")]
    [SerializeField] Transform playerCamera = null;
    [SerializeField] Transform cameraHolder = null;
    [SerializeField] Transform cameraHolderV2 = null;
    CharacterController controller = null;
    public Camera cam;

    [Header("Mouse sensitivity")]
    [SerializeField, Range(1, 100)] float mouseSensitivity = 1f;

    [Header("Character movement speed")]
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float walkingBackwardsSpeed = 4f;
    [SerializeField] float CrouchingSpeed = 4f;
    private float movementSpeed = 0f;
    private Vector3 movement;
    private Vector3 velocity;

    [Header("Speed Build Up")]
    [SerializeField] float speedBuildUp = 10f;

    [Header("FOV")]
    [SerializeField] float runFOV;
    [SerializeField] float walkToRunFovTime;
    [SerializeField] float runToWalkFovTime;
    [SerializeField] float runToCrouchFovTime;
    [SerializeField] float runToBackwardsFovTime;
    [SerializeField] float runToStandstillFovTime;
    private float initFov;

    [Header("Crouching")]
    [SerializeField] private float crouchCamTransitionDuration = 1f;
    [SerializeField] private float crouchScaleTransitionDuration = 1f;
    [SerializeField] private AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] float crouchingCharacterHeight;
    private float startingCharacterHeight;
    private Vector3 startingCharacterCenter;
    private Vector3 crouchingCharacterCenter;
    private float startingCamPos;
    private float crouchingCamPos;
    private bool isCrouching = false;
    private bool crouchAnimationRunning = false;

    [Header("Jumping")]
    [SerializeField] AnimationCurve jumpFallOff;
    [SerializeField] float jumpMultiplier;
    [SerializeField] float landingAnimationDuration;
    [SerializeField] AnimationCurve landingAnimation;
    [SerializeField] float bigJumpAnimationLandingAmount;
    [SerializeField] float smallJumpAnimationLandingAmount;
    private bool isJumping = false;
    private float yBeforeJump;
    private float yAfterJump;
    private bool FallingOff = false;
    private bool jumpingSprint = true;
    private bool jumpingAnimationRunning = false;
    private float characterSlopeLimit;
    private float characterStepOffset;

    [Header("Zipline")]
    [SerializeField] float slidingSpeed = 10f;
    private Transform P1;
    private Transform P2;
    private GameObject tempZiplineObj;
    private GameObject tempZiplineParent;
    private bool readyToSlide = false;
    private bool isSliding = false;
    private float distance;
    private float time;

    [Header("Smoothing")]
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTimeInAir = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    private float moveSmooth;

    [Header("Gravity")]
    [SerializeField] float gravity = -13.0f;

    private float groundRayDistance = 1f;
    private RaycastHit slopeHit;
    private bool onSlopeSliding = false;

    [Header("Head bobbing")]
    [SerializeField] float bobWalkingFrequency;
    [SerializeField] float bobWalkingHorizontalAmp;
    [SerializeField] float bobWalkingVerticalAmp;
    [Space]
    [SerializeField] float bobRuningFrequency;
    [SerializeField] float bobRuningHorizontalAmp;
    [SerializeField] float bobRuningVerticalAmp;
    [Space]
    [SerializeField] float bobWalkingBackwardsFrequency;
    [SerializeField] float bobWalkingBackwardsHorizontalAmp;
    [SerializeField] float bobWalkingBackwardsVerticalAmp;
    [Space]
    [SerializeField] float bobCrouchingFrequency;
    [SerializeField] float bobCrouchingHorizontalAmp;
    [SerializeField] float bobCrouchingVerticalAmp;
    private float bobFrequency;
    private float bobHorizontalAmplitude;
    private float bobVerticalAmplitude;

    [Header("Head bob smoothing")]
    [Range(1f, 100f)] public float headBobSmoothing = 6f;
    [Range(1f, 100f)] public float returnTime = 10f;

    [SerializeField] bool lockCursor = true;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;

    public bool isWalking = false;
    private Vector3 targetCameraPosition;

    Vector2 currentDirection = Vector2.zero;
    Vector2 currentDirectionVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();

        startingCharacterHeight = controller.height;
        startingCharacterCenter = controller.center;

        startingCamPos = cameraHolderV2.localPosition.y;

        crouchingCharacterCenter = (crouchingCharacterHeight / 2f) * Vector3.up;
        crouchingCamPos = startingCamPos - (startingCharacterHeight - crouchingCharacterHeight);

        characterSlopeLimit = controller.slopeLimit;
        characterStepOffset = controller.stepOffset;

        initFov = cam.fieldOfView;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


    }

    void Update()
    {
        UpdateMouseLook();

        UpdateMovement();

        CalculateCamerasTargetPosAndMove();

        ZipLine();

        Crouching();

        FallOff();

        AboveColliderDetection();
    }

    private bool AboveColliderDetection()
    {
        if (Physics.CheckSphere(cameraHolderV2.transform.position + (Vector3.up * 0.4f), 0.4f))
        {
            return true;
        }
        return false;
    }

    private void FallOff()
    {
        if (!isJumping && !controller.isGrounded && !FallingOff)
        {
            StartCoroutine(FallOffCoroutine());

            FallingOff = true;
        }
    }

    private IEnumerator FallOffCoroutine()
    {
        float time = 0;

        do
        {
            time += Time.deltaTime;

            yield return null;

        } while (!controller.isGrounded);

        if (time > 0.2f)
        {

            if (time < 0.4f)
            {
                if (!jumpingAnimationRunning)
                {
                    StartCoroutine(LandingRoutine(smallJumpAnimationLandingAmount));
                }
            }
            else
            {
                if (!jumpingAnimationRunning)
                {
                    StartCoroutine(LandingRoutine(bigJumpAnimationLandingAmount));
                }
            }

        }

        FallingOff = false;
    }


    private void Crouching()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !crouchAnimationRunning && controller.isGrounded)
        {
            if (isCrouching && AboveColliderDetection())
            {
                return;
            }

            StartCoroutine(CrouchRoutine());
        }
    }

    protected virtual IEnumerator CrouchRoutine()
    {
        crouchAnimationRunning = true;

        float percent = 0f;
        float percent2 = 0f;
        float smoothPercent = 0f;
        float smoothPercent2 = 0f;
        float speed = 1f / crouchCamTransitionDuration;
        float speed2 = 1f / crouchScaleTransitionDuration;

        float _currentHeight = controller.height;
        Vector3 _currentCenter = controller.center;

        float _desiredHeight = isCrouching ? startingCharacterHeight : crouchingCharacterHeight;
        Vector3 _desiredCenter = isCrouching ? startingCharacterCenter : crouchingCharacterCenter;

        Vector3 _camPos = cameraHolderV2.localPosition;
        float _camCurrentHeight = _camPos.y;
        float _camDesiredHeight = isCrouching ? startingCamPos : crouchingCamPos;

        isCrouching = !isCrouching;


        while (percent < 1f)
        {
            percent += Time.deltaTime * speed;
            smoothPercent = crouchTransitionCurve.Evaluate(percent);

            percent2 += Time.deltaTime * speed2;
            smoothPercent2 = crouchTransitionCurve.Evaluate(percent2);

            controller.height = Mathf.Lerp(_currentHeight, _desiredHeight, smoothPercent2);
            controller.center = Vector3.Lerp(_currentCenter, _desiredCenter, smoothPercent2);

            _camPos.y = Mathf.Lerp(_camCurrentHeight, _camDesiredHeight, smoothPercent);
            cameraHolderV2.localPosition = _camPos;

            yield return null;
        }

        crouchAnimationRunning = false;
    }

    private Vector3 CalculateHeadBobOffset()
    {
        float horizontalOffset = 0;
        float verticalOffset = 0;

        Vector3 offset = Vector3.zero;

        if (isWalking && controller.isGrounded)
        {
            horizontalOffset = Mathf.Cos(Time.time * bobFrequency / 2) * bobHorizontalAmplitude;
            verticalOffset = Mathf.Sin(Time.time * bobFrequency) * bobVerticalAmplitude;

            offset = cameraHolder.right * horizontalOffset + cameraHolder.up * verticalOffset;
        }

        return offset;
    }

    void CalculateCamerasTargetPosAndMove()
    {
        targetCameraPosition = cameraHolder.position + CalculateHeadBobOffset();

        if (isWalking && controller.isGrounded)
        {
            playerCamera.position = Vector3.Lerp(playerCamera.position, targetCameraPosition, Time.deltaTime * headBobSmoothing);
        }
        
        else
        {
            if (playerCamera.position == cameraHolder.position)
            {
                playerCamera.position = cameraHolder.position;
                return;
            }
            else
            {
                playerCamera.position = Vector3.Lerp(playerCamera.position, cameraHolder.position, returnTime * Time.deltaTime);
            }
        }
        
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;

        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 75.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    private void SetMovementSpeed()
    {
        //Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.S) && !isCrouching && jumpingSprint && controller.velocity.magnitude > 1 && !isSliding)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, runFOV, walkToRunFovTime * Time.deltaTime);

            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * speedBuildUp);

            bobFrequency = bobRuningFrequency;
            bobHorizontalAmplitude = bobRuningHorizontalAmp;
            bobVerticalAmplitude = bobRuningVerticalAmp;
        }

        //Moving Backwards
        else if (Input.GetKey(KeyCode.S) && !isCrouching)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initFov, runToBackwardsFovTime * Time.deltaTime);

            movementSpeed = Mathf.Lerp(movementSpeed, walkingBackwardsSpeed, Time.deltaTime * speedBuildUp);

            bobFrequency = bobWalkingBackwardsFrequency;
            bobHorizontalAmplitude = bobWalkingBackwardsHorizontalAmp;
            bobVerticalAmplitude = bobWalkingBackwardsVerticalAmp;
        }

        //Crouching
        else if (isCrouching)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initFov, runToCrouchFovTime * Time.deltaTime);

            movementSpeed = Mathf.Lerp(movementSpeed, CrouchingSpeed, Time.deltaTime * speedBuildUp);

            bobFrequency = bobCrouchingFrequency;
            bobHorizontalAmplitude = bobCrouchingHorizontalAmp;
            bobVerticalAmplitude = bobCrouchingVerticalAmp;
        }

        //Walking
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initFov, runToWalkFovTime * Time.deltaTime);

            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * speedBuildUp);

            bobFrequency = bobWalkingFrequency;
            bobHorizontalAmplitude = bobWalkingHorizontalAmp;
            bobVerticalAmplitude = bobWalkingVerticalAmp;
        }

        //Idle
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initFov, runToStandstillFovTime * Time.deltaTime);
        }
    }

    void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && controller.isGrounded)
        {
            if (AboveColliderDetection() && isCrouching)
            {
                return;
            }

            isJumping = true;
            if (isCrouching) StartCoroutine(CrouchRoutine());
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        float timeInAir = 0.0f;
        float amount = 0f;

        controller.slopeLimit = 90.0f;
        controller.stepOffset = 0.0f;

        yBeforeJump = this.transform.position.y;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);

            controller.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);

            timeInAir += Time.deltaTime;

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                jumpingSprint = false;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                jumpingSprint = false;
            }

            yield return null;

        } while (!controller.isGrounded && controller.collisionFlags != CollisionFlags.Above);

        controller.slopeLimit = characterSlopeLimit;
        controller.stepOffset = characterStepOffset;

        yAfterJump = this.transform.position.y;

        if (yBeforeJump < yAfterJump)
        {
            amount = smallJumpAnimationLandingAmount;
        }
        else
        {
            amount = bigJumpAnimationLandingAmount;
        }

        if (!jumpingAnimationRunning)
        {
            StartCoroutine(LandingRoutine(amount));
        }

        jumpingSprint = true;

        isJumping = false;
    }

    void UpdateMovement()
    {
        SetMovementSpeed();

        JumpInput();

        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector2 idle = Vector2.zero;

        targetDirection.Normalize();

        if (!controller.isGrounded)
        {
            moveSmooth = moveSmoothTimeInAir;
        }
        else
        {
            moveSmooth = moveSmoothTime;
        }

        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirectionVelocity, moveSmooth);

        if (controller.isGrounded)
        {
            velocityY = -5.5f;
        }
        else if(isSliding)
        {
            velocityY = -10;
        }
        

        velocityY += gravity * Time.deltaTime;

        velocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x) * movementSpeed + Vector3.up * velocityY;

        movement = velocity;

        /*
        if (OnSteepSlope())
        {
            SteepSlopeMovement();
        }
        */

        controller.Move(movement * Time.deltaTime);

        if (targetDirection == idle || controller.velocity.magnitude < 1 || onSlopeSliding)
        {
            isWalking = false;
        }
        else
        {
            isWalking = true;
        }
    }

    protected virtual IEnumerator LandingRoutine(float amount)
    {
        jumpingAnimationRunning = true;

        float percent = 0f;
        float landAmount = 0f;

        float _speed = 1f / landingAnimationDuration;

        Vector3 localPos = cameraHolder.localPosition;

        float _initLandHeight = localPos.y;

        landAmount = amount;

        while (percent < 1f)
        {
            percent += Time.deltaTime * _speed;

            float _desiredY = landingAnimation.Evaluate(percent) * landAmount;

            localPos.y = _initLandHeight + _desiredY;

            cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, localPos.y, cameraHolder.localPosition.z);

            yield return null;
        }

        jumpingAnimationRunning = false;
    }

    private bool OnSteepSlope()
    {
        if (!controller.isGrounded) return false;

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, (controller.height / 2) + groundRayDistance))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);

            if (slopeAngle > controller.slopeLimit)
            {
                onSlopeSliding = true;

                return true;
            }
        }
        onSlopeSliding = false;

        return false;
    }

    private void SteepSlopeMovement()
    {
        Vector3 slopeDir = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);

        float slideSpeed = 8f + Time.deltaTime;

        movement = slopeDir * -slideSpeed;

        movement.y = movement.y - slopeHit.point.y;
    }


    //Ziplines Start
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Pole1Coll" || other.name == "Pole2Coll")
        {
            readyToSlide = true;
            tempZiplineParent = other.transform.parent.gameObject;
            tempZiplineObj = other.transform.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.name == "Pole1Coll" || other.name == "Pole2Coll") && readyToSlide)
        {
            readyToSlide = false;
        }
    }
    
    public void ZipLine()
    {
        if(readyToSlide && !isSliding)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if (tempZiplineObj.name == "Pole1Coll")
                {
                    P1 = tempZiplineParent.transform.Find("P1");
                    P2 = tempZiplineParent.transform.Find("P2");
                }
                else
                {
                    P1 = tempZiplineParent.transform.Find("P2");
                    P2 = tempZiplineParent.transform.Find("P1");
                }

                distance = Vector3.Distance(P1.transform.position, P2.transform.position);

                isSliding = true;

                StartCoroutine(Slide());

                readyToSlide = false;
            }
        }
    }

    IEnumerator Slide()
    {
        do
        {
            transform.position = Vector3.Lerp(P1.transform.position, P2.transform.position, time);
            time += Time.deltaTime / distance * slidingSpeed;
            yield return null;

        } while (time < 1 && !Input.GetKeyDown(KeyCode.Space));

        time = 0f;
        isSliding = false;
    }
    //Ziplines End

}
