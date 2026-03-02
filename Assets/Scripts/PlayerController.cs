using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    //Initialzies variablews
    public DialogueManager dialogueManager;
    public Camera playerCamera;
    public Camera interviewCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;
    private bool onOfficeCam = true;

    //this runs as soon as the game scene is laoded
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        interviewCamera.enabled = false;
    }
    //this is called EVERY frame
    void Update()
    {
        //this sets the direction which is used in the movement part
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        //checks if the player is sprinting
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        //Checks if the player can move, then checks if they are running if they are use run speed if not use walk speed
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        //This moves based on the last code for curent speed on the x and y axis
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        //if the jump button is pressed then moves the player up by the jump power
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        //if in the air makes sure gravity will bring the player down
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        //if the player left clicks
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray;
            //checks which camera is on, if the office camera shoot a raycast from the player camera
            if(onOfficeCam)
            {
                ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            }
            //if from the interview shoots a ray from that camera
            else
            {
                ray = interviewCamera.ScreenPointToRay(Input.mousePosition);
            }
            
            RaycastHit hit;
            float maxDistance = 100f;
            //this part registers what happens from the ray that was cast
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                //if the ray hits something with the interview tag it swithces the camera and allows the mouse to move but stops the player from moving
                if (hit.collider.CompareTag("Interview"))
                {
Debug.Log("Did Hit, interview");
                    canMove = false;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    playerCamera.enabled = false;
                    interviewCamera.enabled = true;
                    onOfficeCam = false;
                }
                //If hit something with the office tag does the opposite, hides mouse but allows player to move, also swithces camera
                else if (hit.collider.CompareTag("Office"))
                {
Debug.Log("Did Hit, office");
                    canMove = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    playerCamera.enabled = true;
                    interviewCamera.enabled = false;
                    onOfficeCam = true;
                }
                //these options will let the dialouge manager know there is a new option and that script will dictate what dialogue to choose from
                else if (hit.collider.CompareTag("Positive"))
                {
                    dialogueManager.nextLine(1.0f);
                }
                else if (hit.collider.CompareTag("Neutral"))
                {
                    dialogueManager.nextLine(1.0f);
                }
                else if (hit.collider.CompareTag("Negative"))
                {
                    dialogueManager.nextLine(1.0f);
                }

            }
        }
        //Crouches if the player presses left control
        if (Input.GetKey(KeyCode.LeftControl) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;

        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }
        //moves the player
        characterController.Move(moveDirection * Time.deltaTime);
        //moves camera within the player
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}

