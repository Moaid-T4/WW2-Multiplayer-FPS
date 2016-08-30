using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonController : ControlableBehaviour
{
    [SerializeField]
    bool canMove = true;
    [SerializeField]
    bool canLook = true;
	[SerializeField]
	bool canSlide = true;

    //movement
    float xMove;
    float yMove;

    [SerializeField]
    float moveDelta = 0.05f;

    Vector3 xDir;
    Vector3 yDir;

    Vector3 movement;

    [SerializeField]
    internal float speed;

    [SerializeField]
    float deAcceleration = 0.1f;

    //forces
    [SerializeField]
    float maxJumpPower = 5;
    float jumpPower;
    [SerializeField]
    float jumpPowerDelta = 0.25f;

    [SerializeField]
    float maxGravity = 12;
    float gravity;
    [SerializeField]
    float gravityDelta = 0.25f;

    [SerializeField]
    float maxSlide = 2.7f;
    float slide;
    [SerializeField]
    float slideDelta = 0.05f;
    [SerializeField]
    float slideStateChangeDelta = 0.5f;

    //looking
    float xMouse;
    float yMouse;

    [SerializeField]
    float lookDelta = 0.2f;

    internal bool hitLimmit;

    [SerializeField]
    float mouseSensitivity = 2.5f;

    [SerializeField]
    Vector2 lookLimmit = new Vector2(80, 280);

    Vector3 camPosition;
    Vector3 camRotation;

    Vector3 chRotation;


    //character state
    bool standing = true;
    internal bool sprinting;
    bool crouching;
    bool proning;
    bool sliding;
    bool jumping;
    bool falling;
    bool onGround;

    internal bool changingState;

    [SerializeField]
    float changeStateDelta = 0.035f;
    [SerializeField]
    float speedChangeDelta = 0.5f;

    [SerializeField]
    internal CharacterState normal = new CharacterState(3,1.8f,0.35f);
    [SerializeField]
    internal CharacterState sprint = new CharacterState(8, 1.8f, 0.35f);
    [SerializeField] 
    CharacterState crouch = new CharacterState(1, 1.2f, 0.35f);
    [SerializeField]
    CharacterState prone = new CharacterState(0.25f, 0.4f, 0.2f);
    [SerializeField]
    CharacterState slideState = new CharacterState(4, 1.2f, 0.35f);

    CharacterState oldState;
    CharacterState desiredState;
    float desiredStateChangeSpeed;

    //leaning
    [SerializeField]
    float maxLean = 30;
    float lean;
    [SerializeField]
    float leanDelta = 2;

    [SerializeField]
    bool AlignCamRotation = true;

    //buttons
    [HideInInspector]
    public KeyCode walkF = KeyCode.W;
    [HideInInspector]
    public KeyCode walkB = KeyCode.S;
    [HideInInspector]
    public KeyCode walkR = KeyCode.D;
    [HideInInspector]
    public KeyCode walkL = KeyCode.A;
    [HideInInspector]
    public KeyCode lookUp = KeyCode.UpArrow;
    [HideInInspector]
    public KeyCode lookDown = KeyCode.DownArrow;
    [HideInInspector]
    public KeyCode lookRight = KeyCode.RightArrow;
    [HideInInspector]
    public KeyCode lookLeft = KeyCode.LeftArrow;
    [HideInInspector]
    public KeyCode jumpKey = KeyCode.Space;
    [HideInInspector]
    public KeyCode sprintKey = KeyCode.LeftShift;
    [HideInInspector]
    public KeyCode crouchKey = KeyCode.C;
    [HideInInspector]
    public KeyCode proneKey = KeyCode.LeftControl;
    [HideInInspector]
    public KeyCode leanR = KeyCode.E;
    [HideInInspector]
    public KeyCode leanL = KeyCode.Q;

    //generic
    [SerializeField]
    float CheckDistance = 0.1f;

    [SerializeField]
    internal GameObject currentWeapon;

    [SerializeField]
    Slider sprintProgress;

    [SerializeField]
    float maxSprintTime = 15;

    [SerializeField]
    float minimumSprintTime;

    float sprintTime;

    [SerializeField]
    float sprintTimeDelta = 0.025f;

    bool canRun;

    private CharacterController ch;
    private Transform camPivot;
    private Transform cam;

    void Start()
    {
        ch = GetComponent<CharacterController>();
        cam = transform.Find("CamPivot/Camera");
        camPivot = transform.Find("CamPivot");
        speed = normal.Speed;
        oldState = normal;
        gravity = maxGravity;
        if (sprintProgress)
        {
            sprintProgress.maxValue = maxSprintTime;
            sprintProgress.minValue = 0;
            sprintProgress.value = maxSprintTime - sprintTime;
        }

        Controls.AddListener(this);
        RefreshBinds();
    }

    public override void RefreshBinds()
    {
        walkF = Controls.controls["On Foot.Walk Forward"];
        walkB = Controls.controls["On Foot.Walk Backwards"];
        walkR = Controls.controls["On Foot.Strafe Right"];
        walkL = Controls.controls["On Foot.Strafe Left"];

        lookUp = Controls.controls["On Foot.Look Up"];
        lookDown = Controls.controls["On Foot.Look Down"];
        lookRight = Controls.controls["On Foot.Look Right"];
        lookLeft = Controls.controls["On Foot.Look Down"];

        sprintKey = Controls.controls["On Foot.Sprint"];
        jumpKey = Controls.controls["On Foot.Jump"];
        crouchKey = Controls.controls["On Foot.Crouch"];
        proneKey = Controls.controls["On Foot.Prone"];

        leanR = Controls.controls["On Foot.Lean Right"];
        leanL = Controls.controls["On Foot.Lean Left"];
    }

    void Update()
    {
        if (Game.paused)
            return;

        if(sprinting)
        {
            sprintTime = Mathf.MoveTowards(sprintTime, maxSprintTime, Time.deltaTime);
            if(sprintTime == maxSprintTime)
            {
                sprinting = false;
                canRun = false;
                SwitchState(normal, changeStateDelta);
            }
        }

        else
        {
            sprintTime = Mathf.MoveTowards(sprintTime, 0, sprintTimeDelta);
            if(sprintTime <= minimumSprintTime)
            {
                canRun = true;
            }
        }

        if (sprintProgress)
            sprintProgress.value = maxSprintTime - sprintTime;

        CheckState();
        Lean();
        Move();
        Look();
    }

    void Move()
    {
        onGround = CheckOnGrounded() || ch.isGrounded;

        if(!canMove)
        {
            xMove = 0;
            yMove = 0;
        }

        if (onGround)
        {
            if(canMove)
            {
                if (!sliding)
                {
                    Tools.KeyCodeToAxis(walkF, walkB, ref yMove, moveDelta);
                    Tools.KeyCodeToAxis(walkR, walkL, ref xMove, moveDelta);
                }
                else
                {
                    yMove = slide;
                    xMove = 0;
                }
            }
        }

        else
        {
            if (!falling && !jumping)
            {
                StartCoroutine(Fall());
            }
        }

        if (xMove == 0 && yMove == 0)
        {
            if (sprinting)
            {
                sprinting = false;
            }
        }
        

        //calculate both axis directions
        xDir = transform.right * (xMove * speed);
        yDir = transform.forward * (yMove * speed);

        //calculate movement direction
        movement = xDir + yDir;

        //applying jumpPower
        if (jumping)
            movement.y = jumpPower;
        else
            movement.y = -gravity;

        //apply movement
        ch.Move(movement * Time.deltaTime);
    }

    void Look()
    {
        if (!canLook)
        {
            xMouse = 0;
            yMouse = 0;
            return;
        }

        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            xMouse = Input.GetAxis("Mouse X");
            yMouse = Input.GetAxis("Mouse Y");
        }

        else
        {
            Tools.KeyCodeToAxis(lookUp, lookDown, ref yMouse, lookDelta);
            Tools.KeyCodeToAxis(lookRight, lookLeft, ref xMouse, lookDelta);
        }

        camRotation = cam.localEulerAngles;
        chRotation = transform.eulerAngles;

        chRotation.y += xMouse * mouseSensitivity;
        camRotation.x += -yMouse * mouseSensitivity;

        if (camRotation.x > lookLimmit.x && camRotation.x < 180)
        {
            hitLimmit = true;
            camRotation.x = lookLimmit.x;
        }

        else if (camRotation.x < lookLimmit.y && camRotation.x > 180)
        {
            hitLimmit = true;
            camRotation.x = lookLimmit.y;
        }

        else
            hitLimmit = false;

        cam.localEulerAngles = camRotation;
        transform.eulerAngles = chRotation;
    }

    void CheckState()
    {
        if (!canMove)
            return;
        if(Input.GetKeyUp(sprintKey) && !sliding)
        {
            sprinting = false;

            if (!standing)
            {
                standing = true;
                SwitchState(normal, changeStateDelta);
            }
        }

        if(Input.GetKey(sprintKey) && !sliding && sprintTime < maxSprintTime && canRun)
        {
            //if we are trying to go forward
            if(yMove > 0)
            {
                if (!sprinting)
                {
                    SwitchState(sprint, changeStateDelta);
                }

                sprinting = true;
                standing = false;

                crouching = false;
                proning = false;
            }
            else
            {
                if(sprinting)
                {
                    SwitchState(normal, changeStateDelta);
                }
                sprinting = false;
            }
        }

        if(Input.GetKeyDown(crouchKey) && !sliding)
        {
            crouching = !crouching;
            proning = false;
            standing = !crouching;

            if (sprinting && canSlide)
            {
                sprinting = false;
                StartCoroutine(Slide());
                return;
            }

            sprinting = false;

            if (crouching)
            {
                SwitchState(crouch, changeStateDelta);
            }
            else
            {
                SwitchState(normal, changeStateDelta);
            }
        }

        else if(Input.GetKeyDown(proneKey) && !sliding)
        {
            proning = !proning;
            crouching = false;
            sprinting = false;
            standing = !proning;
            if (proning)
            {
                SwitchState(prone, changeStateDelta);
            }
            else
            {
                SwitchState(normal,changeStateDelta);
            }
        }

        if(Input.GetKeyDown(jumpKey) && !sliding)
        {
            if (crouching || proning)
            {
                crouching = false;
                proning = false;
                standing = true;
                SwitchState(normal, changeStateDelta);
            }
            else if (!jumping && onGround)
                StartCoroutine(Jump());
        }
    }

    IEnumerator Fall()
    {
        falling = true;
        gravity = 0;

        while (!onGround)
        {
            gravity = Mathf.MoveTowards(gravity, maxGravity, gravityDelta);
            yield return new WaitForFixedUpdate();
        }

        falling = false;
        gravity = maxGravity;
    }

    IEnumerator Jump()
    {
        jumping = true;
        jumpPower = maxJumpPower;

        while (jumpPower != 0)
        {
            if (ch.collisionFlags == CollisionFlags.CollidedAbove)
                break;
            jumpPower = Mathf.MoveTowards(jumpPower, 0, jumpPowerDelta);
            yield return new WaitForFixedUpdate();
        }

        jumping = false;
    }

    IEnumerator Slide()
    {
        sliding = true;
        slide = maxSlide;

        SwitchState(slideState, slideStateChangeDelta);

        while (slide != 0)
        {
            slide = Mathf.MoveTowards(slide, 0, slideDelta);

            yield return new WaitForFixedUpdate();
        }

        sliding = false;
        speed = crouch.Speed;
    }

    void Lean()
    {
        if(Input.GetKey(leanR) && canMove)
        {
            lean = Mathf.MoveTowards(lean, -maxLean, leanDelta);
        }
        else if(Input.GetKey(leanL) && canMove)
        {
            lean = Mathf.MoveTowards(lean, maxLean, leanDelta);
        }
        else
        {
            lean = Mathf.MoveTowards(lean, 0, leanDelta);
        }

        camPivot.localEulerAngles = new Vector3(0, 0, lean);

        if(AlignCamRotation) 
        {
            camRotation.z = -lean;
            cam.localEulerAngles = camRotation;
        }

    }

    void SwitchState(CharacterState newState, float customChangeSpeed)
    {
        if (changingState)
        {
            desiredState = newState;
            desiredStateChangeSpeed = customChangeSpeed;
        }
        else
            StartCoroutine(ChangeState(newState, customChangeSpeed));
    }

    IEnumerator ChangeState(CharacterState newState, float customChangeSpeed)
    {
        desiredState = newState;
        desiredStateChangeSpeed = customChangeSpeed;

        if (changingState)
            yield break;

        changingState = true;

        while (ch.height != desiredState.CharacterHeight || ch.radius != desiredState.CharacterRadius || speed != desiredState.Speed)
        {
            if (oldState.CharacterHeight < desiredState.CharacterHeight && HitHead())
            {
                desiredState = oldState;
                if (desiredState == crouch)
                    crouching = true;
                else if (desiredState == prone)
                    proning = true;
            }

            ch.height = Mathf.MoveTowards(ch.height, desiredState.CharacterHeight, desiredStateChangeSpeed);
            ch.radius = Mathf.MoveTowards(ch.radius, desiredState.CharacterRadius, desiredStateChangeSpeed / 5);

            ch.center = new Vector3(0, ch.height / 2);

            camPosition = cam.localPosition;
            camPosition.y = ch.center.y - 0.05f;

            cam.localPosition = camPosition;
            camPivot.localPosition = camPosition;

            speed = Mathf.MoveTowards(speed, desiredState.Speed, speedChangeDelta);

            yield return new WaitForFixedUpdate();
        }

        changingState = false;
        oldState = desiredState;
    }

    bool HitHead()
    {
        Vector3 up = transform.TransformDirection(Vector3.up);
        Vector3 startPos = transform.position;
        startPos.y += ch.height + CheckDistance;
        if (Physics.Raycast(startPos, up, CheckDistance))
        {
            return true;
        }

        return false;
    }

    bool CheckOnGrounded()
    {
        Vector3 dwn = transform.TransformDirection(Vector3.down);
        Vector3 startPos = transform.position;
        if (Physics.Raycast(startPos, dwn, CheckDistance))
        {
            return true;
        }

        return false;
    }
}

[System.Serializable]
public class CharacterState
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float characterHeight;
    [SerializeField]
    private float characterRadius;

    public float Speed
    {
        get { return speed; }
    }
    public float CharacterHeight
    {
        get { return characterHeight; }
    }
    public float CharacterRadius
    {
        get { return characterRadius; }
    }

    public CharacterState()
    {
        speed = 0;
        characterHeight = 0;
        characterRadius = 0;
    }

    public CharacterState(float newSpeed, float newCharacterHeight, float newCharacterRadius)
    {
        speed = newSpeed;
        characterHeight = newCharacterHeight;
        characterRadius = newCharacterRadius;
    }
}