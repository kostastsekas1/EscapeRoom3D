using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MovementAndLookController : MonoBehaviour, IDataPersistence
{
    public bool canMove { get; private set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(sprintKey);
    private bool shouldJump => Input.GetKeyDown(JumpKey) && characterController.isGrounded;
    private bool shouldCrouch => Input.GetKeyDown(crouchKey) &&!duringCrouch && characterController.isGrounded;


    [Header("Controller Functions")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool HeadBobEnabled = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool haveFootsteps = true;



    [Header("Movement variables")]
    [SerializeField] private float walkSpeed= 3.0f;
    [SerializeField] private float sprintSpeed = 4.5f;
    [SerializeField] private float crouchSpeed = 1.5f;


    [Header("Camera variables")]
    [SerializeField, Range(1,10)] private float lookSensitivityX= 2.0f;
    [SerializeField, Range(1,10)] private float lookSensitivityY= 2.0f;
    [SerializeField, Range(1,180)] private float UpperLookLimit= 80.0f;
    [SerializeField, Range(1,180)] private float LowerLookLimit= 80.0f;


    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode JumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode InteractKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode DropKey = KeyCode.Q;


    [Header("Jump variables")]
    [SerializeField] private float jumpHeight = 8.0f;
    [SerializeField] private float gravity = 30.0f;


    [Header("Crouch variables")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 centerCrouch = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 centerStanding = new Vector3(0,0,0);
    private bool isCrouching;
    private bool duringCrouch;


    [Header("HeadBob variables")]
    [SerializeField] private float walkHeadBobSpeed = 12f;
    [SerializeField] private float walkHeadBobValue = 0.05f;
    [SerializeField] private float sprintHeadBobSpeed = 15f;
    [SerializeField] private float sprintHeadBobValue = 0.09f;
    [SerializeField] private float crouchHeadBobSpeed = 8f;
    [SerializeField] private float crouchHeadBobValue = 0.025f;
    private float defaultYposition = 0;
    private float timer;

    [Header("Interact With Objects")]
    [SerializeField] private float interactionDistanceLimit = default;
    [SerializeField] private LayerMask interactionLayerMask = default;
    private InteractableObject currentInteractableObject;


    [Header("Footsteps Variables")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float sprintStepMultiplier = 1.5f;
    [SerializeField] private float CrouchStepMultiplier = 0.6f;
    [SerializeField] private AudioSource footstepAudio = default;
    [SerializeField] private AudioClip[] grassAudio = default;
    [SerializeField] private AudioClip[] WoodAudio = default;

    private float footsteptimer = 0;
    private float getCurrentOffset => isCrouching ? baseStepSpeed * CrouchStepMultiplier : isSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;


    [Header("Pick Up Variables")]
    [SerializeField] private Transform pickUpSlot;
    [SerializeField] private InteractableObject inHandItem;


    [Header("CrossHair Variables")]
    [SerializeField] private Image Standardcrosshair = default;

    



    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX=0;



    void Awake() {

        Standardcrosshair.color = Color.green;
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYposition = playerCamera.transform.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible= false;
        DataPersistence.instance.SaveGame();
    }

    void Update() {
        if (canMove){
            MovementInput();
            MouseLookInput();
            
            if (canJump)
            {
                JumpInput();
            }

            if (canCrouch)
            {
                CrouchInput();
            }

            if (HeadBobEnabled)
            {
                HeadBob();
            }
            if (canInteract) 
            {
                InteractionCheck();
                InteractionInput();

            }
            if (haveFootsteps)
            {
                footstepsHandler();
            }

            applyMovement();
        }
    }



    private void MovementInput()
    {
        currentInput = new Vector2((isCrouching?crouchSpeed : isSprinting?sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        float moveInDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y= moveInDirectionY; 
    }

    private void MouseLookInput()

    {
        if (PauseMenu.IsGamePaused) 
        { 
            return; 
        }
        
        if (Time.timeScale==0.0f)
        {
            return;
        }

        rotationX -= Input.GetAxis("Mouse Y") * lookSensitivityY;
        rotationX = Mathf.Clamp(rotationX, -UpperLookLimit, LowerLookLimit);
        playerCamera.transform.localRotation= Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *=Quaternion.Euler(0,Input.GetAxis("Mouse X")*lookSensitivityX, 0); 
    }
    
    private void applyMovement()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        characterController.Move(moveDirection*Time.deltaTime);    
        
    }


    private void JumpInput()
    {
        if (shouldJump)
        {
            moveDirection.y = jumpHeight;
        }
    }
    
    private void CrouchInput()
    {
        if (shouldCrouch)
        {
           StartCoroutine(CrouchStand());
        }

    }
    
    private IEnumerator CrouchStand()
    {

        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f)) 
        {
            yield break;
        }

        duringCrouch= true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? centerStanding : centerCrouch;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight,targetHeight,timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter,targetCenter,timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;

        }

        characterController.height = targetHeight;
        characterController.center = targetCenter; 
        
        isCrouching = !isCrouching;

        duringCrouch = false;
    }
    
    private void HeadBob()
    {
        if (!characterController.isGrounded) { return; }

        if(Mathf.Abs(moveDirection.x)>0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchHeadBobSpeed : isSprinting ? sprintHeadBobSpeed : walkHeadBobSpeed);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x,
                defaultYposition+Mathf.Sin(timer)* (isCrouching ? crouchHeadBobValue : isSprinting ? sprintHeadBobValue : walkHeadBobValue),
                playerCamera.transform.localPosition.z);
        }
    }

    private void InteractionCheck()
    {
        if(Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, interactionDistanceLimit))
        {

            if (hit.collider.gameObject.layer==6 && (currentInteractableObject == null || hit.collider.gameObject.GetInstanceID()!= currentInteractableObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractableObject);

                if (currentInteractableObject)
                {
                    currentInteractableObject.OnFocus();
                    Standardcrosshair.color= Color.red;
                }
            }
        }
        else if (currentInteractableObject)
        {
            currentInteractableObject.OnLoseFocus();
            currentInteractableObject = null;
            Standardcrosshair.color = Color.green;

        }
    }

    private void InteractionInput()
    {
        if (Input.GetKeyDown(InteractKey) && currentInteractableObject != null && Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition),out RaycastHit hit,interactionDistanceLimit,interactionLayerMask)) 
        {
            if (inHandItem != null)
            {
                return;
            }
            currentInteractableObject.OnInteract(pickUpSlot);
            Debug.Log( hit.collider.gameObject.name);
            if (hit.collider.tag == "Door" || hit.collider.tag =="uninteractableObjects/LockWheel")

            {
                Debug.Log("Lockwheel" + hit.collider.gameObject.name);
                return;
            }
            inHandItem = currentInteractableObject;
           
        }


        if (Input.GetKeyDown(DropKey))
        {
            if (inHandItem != null)
            {
               inHandItem.OnDrop();
               inHandItem = null;
            }

        }
    }


    private void footstepsHandler()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        if (currentInput == Vector2.zero)
        {
            return;
        }

        footsteptimer -= Time.deltaTime;
        if (footsteptimer <= 0)
        {
            if(Physics.Raycast(playerCamera.transform.position,Vector3.down,out RaycastHit hit, 2))
            {
                switch (hit.collider.tag)
                {
                    case "footsteps/GRASS":
                        footstepAudio.PlayOneShot(grassAudio[UnityEngine.Random.Range(0,grassAudio.Length-1)]);
                          
                        break;
                    case "footsteps/WOOD":

                        footstepAudio.PlayOneShot(WoodAudio[UnityEngine.Random.Range(0, grassAudio.Length - 1)]);
                        break;
                    default:
                        footstepAudio.PlayOneShot(WoodAudio[UnityEngine.Random.Range(0, grassAudio.Length - 1)]);

                        break;
                }
            }
            footsteptimer = getCurrentOffset;
        }

    }

    public void LoadData(PlayerData data)
    {
        Vector3 playerpositionvec = new Vector3(data.playerposition[0], data.playerposition[1], data.playerposition[2]);

        this.transform.position = playerpositionvec;
    }

    public void SaveData( PlayerData data)
    {
        
        data.playerposition[0] = this.transform.position.x;
        data.playerposition[1] = this.transform.position.y;
        data.playerposition[2] = this.transform.position.z;

        data.saveDate = DateTime.Now.ToShortDateString()+" "+ DateTime.Now.ToShortTimeString();
       

    }


    
}

