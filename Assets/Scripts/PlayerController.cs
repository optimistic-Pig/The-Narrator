using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Player movement and interaction controller.
///
/// CHANGES FROM ORIGINAL:
///   1. NPC click now checks GameStateManager.IsNPCAvailable() before
///      allowing the interview to start. Locked NPCs are silently ignored
///      (add visual feedback here if desired).
///   2. Added "Desk" tag handling — calls dialogueManager.ShowDesk().
///      Add a collider tagged "Desk" to your writing-desk prop in the scene.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // =====================================================================
    // PUBLIC FIELDS
    // =====================================================================

    public DialogueManager dialogueManager;
    public Camera playerCamera;
    public Camera interviewCamera;
    public float walkSpeed  = 6f;
    public float runSpeed   = 12f;
    public float jumpPower  = 7f;
    public float gravity    = 10f;
    public float lookSpeed  = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight  = 1f;
    public float crouchSpeed   = 3f;

    public GameObject dictionaryButton;
    public GameObject dictionaryScreen;
    public GameObject crosshair;

    // ─── Player Thoughts ─────────────────────────────────────────────────
    [Header("Player Thoughts")]
    [Tooltip("Assign the PlayerThoughts component here.")]
    public PlayerThoughts playerThoughts;

    // =====================================================================
    // PRIVATE STATE
    // =====================================================================

    private Vector3 moveDirection = Vector3.zero;
    private float   rotationX     = 0;
    private CharacterController characterController;

    private bool canMove    = true;
    private bool onOfficeCam = true;

    // ─── Hover-thought tracking ───────────────────────────────────────────
    private string _lastHoverTag  = "";
    private string _lastHoverName = "";
    private float  _hoverTimer    = 0f;
    private bool   _hoverThoughtShown = false;

    // =====================================================================
    // PUBLIC METHODS
    // =====================================================================

    /// <summary>
    /// Call from the "Return to Office" button.
    /// </summary>
    public void ReturnToOffice()
    {
        canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        playerCamera.enabled    = true;
        interviewCamera.enabled = false;
        onOfficeCam = true;
        crosshair.gameObject.SetActive(true);
        dictionaryButton.SetActive(true);
        dictionaryScreen.SetActive(false);
        // NOTE: do NOT call dialogueManager.ReturnToOffice() here.
        // DialogueManager is the caller — calling back would cause infinite recursion.

        if (playerThoughts != null) playerThoughts.SetOfficeMode(true);
    }

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        interviewCamera.enabled = false;

        // Subscribe to game-state changes to fire contextual thoughts
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    // ─── State-change thoughts ────────────────────────────────────────────
    private void OnGameStateChanged()
    {
        if (playerThoughts == null) return;
        var gsm = GameStateManager.Instance;
        if (gsm == null) return;

        if (gsm.WaitingForDesk)
        {
            playerThoughts.ShowThought(
                "That went well. I should head to my desk and write up the article.", 6f);
        }
        else if (gsm.InterviewPaused)
        {
            playerThoughts.ShowThought(
                "I should go back and finish that interview...", 5f);
        }
        else if (!gsm.InterviewInProgress && !gsm.WaitingForDesk && !gsm.InterviewPaused)
        {
            // Article just published — day may have advanced
            int day = gsm.CurrentDay;
            string thought = BuildDayStartThought(gsm, day);
            if (thought != null)
                playerThoughts.ShowThought(thought, 7f);
        }
    }

    /// <summary>
    /// Builds a contextual thought for the start of each day based on which
    /// NPCs are still available, so the player knows who to look for.
    /// </summary>
    private string BuildDayStartThought(GameStateManager gsm, int day)
    {
        if (day == 1)
            return "Day 1. Izul and Kortnara are both here \u2014 I can only interview one today.";

        if (day == 2)
        {
            bool izulLeft     = !gsm.IsAlreadyInterviewed(gsm.izulNPC);
            bool kortnaraLeft = !gsm.IsAlreadyInterviewed(gsm.kortnaraNPC);

            if (izulLeft && kortnaraLeft)
                // Shouldn't happen, but handle gracefully
                return "Day 2. I should pick someone to interview \u2014 and Gorp is here too.";
            if (izulLeft)
                return "Day 2. I still need to talk to Izul \u2014 and I hear Gorp is around as well.";
            if (kortnaraLeft)
                return "Day 2. Kortnara is still available \u2014 plus there\u2019s someone named Gorp here.";
            return "Day 2. Gorp is available today \u2014 let\u2019s see what they have to say.";
        }

        if (day == 3)
            return "Day 3. Andrew is here \u2014 this is the last interview before I file my final story.";

        return null;
    }

    void Update()
    {
        // ── Movement ─────────────────────────────────────────────────────
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right   = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical")   : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            moveDirection.y = jumpPower;
        else
            moveDirection.y = movementDirectionY;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        // ── Mouse click / raycast ─────────────────────────────────────────
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = onOfficeCam
                ? playerCamera.ScreenPointToRay(Input.mousePosition)
                : interviewCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                string tag     = hit.collider.tag;
                string hitName = hit.collider.gameObject.name;

                // ── Interview NPC ─────────────────────────────────────────
                if (tag == "Interview")
                {
                    Debug.Log("Did Hit, interview");
                    var interview = hit.collider.GetComponentInParent<InterviewBase>();

                    // ── CHANGE 1: check lock state ────────────────────────
                    if (GameStateManager.Instance != null &&
                        !GameStateManager.Instance.IsNPCAvailable(interview))
                    {
                        Debug.Log($"[PlayerController] NPC is not available today — ignoring click.");
                        // TODO: optional — show a "not available" message or visual cue
                        return;
                    }

                    canMove = false;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible   = true;
                    crosshair.gameObject.SetActive(false);
                    playerCamera.enabled    = false;
                    interviewCamera.enabled = true;
                    onOfficeCam = false;
                    if (playerThoughts != null) playerThoughts.SetOfficeMode(false);
                    dialogueManager.ShowBriefing(interview);
                }

                // ── Office / return ───────────────────────────────────────
                else if (tag == "Office")
                {
                    Debug.Log("Did Hit, office");
                    ReturnToOffice();
                }

                // ── Writing Desk ──────────────────────────────────────────
                // ── CHANGE 2: new Desk tag ────────────────────────────────
                else if (tag == "Desk")
                {
                    Debug.Log("Did Hit, desk");

                    if (GameStateManager.Instance != null &&
                        GameStateManager.Instance.WaitingForDesk)
                    {
                        // Switch to interview camera / UI for article writing
                        canMove = false;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible   = true;
                        crosshair.gameObject.SetActive(false);
                        playerCamera.enabled    = false;
                        interviewCamera.enabled = true;
                        onOfficeCam = false;
                        if (playerThoughts != null) playerThoughts.SetOfficeMode(false);
                        dialogueManager.ShowDesk();
                    }
                    else
                    {
                        Debug.Log("[PlayerController] Desk clicked but no article to write yet.");
                    }
                }

                // ── Dictionary ────────────────────────────────────────────
                else if (tag == "DictionaryOpen")
                {
                    dictionaryButton.SetActive(false);
                    dictionaryScreen.SetActive(true);
                    Debug.Log("Dictionary open");
                }
                else if (tag == "DictionaryClose")
                {
                    dictionaryButton.SetActive(true);
                    dictionaryScreen.SetActive(false);
                    Debug.Log("Dictionary close");
                }
                else if (tag == "Dictionary1") { dialogueManager.DictionaryLookup(1); }
                else if (tag == "Dictionary2") { dialogueManager.DictionaryLookup(2); }
                else if (tag == "Dictionary3") { dialogueManager.DictionaryLookup(3); }
                else if (tag == "Dictionary4") { dialogueManager.DictionaryLookup(4); }
                else if (tag == "Dictionary5") { dialogueManager.DictionaryLookup(5); }

                // ── Dialogue options ──────────────────────────────────────
                // Uses BOTH tag and name detection to work around Unity
                // tag serialisation issues.
                if (tag == "Option1" || hitName.Contains("Option 1") || hitName.Contains("Option  1"))
                {
                    dialogueManager.OptionClicked(1.0f);
                    Debug.Log("option 1");
                }
                else if (tag == "Option2" || hitName.Contains("Option 2") || hitName.Contains("Option  2"))
                {
                    dialogueManager.OptionClicked(2.0f);
                    Debug.Log("option 2");
                }
                else if (tag == "Option3" || hitName.Contains("Option 3") || hitName.Contains("Option  3"))
                {
                    dialogueManager.OptionClicked(3.0f);
                    Debug.Log("option 3");
                }
                else if (tag == "Option4" || hitName.Contains("Option 4") || hitName.Contains("Option  4"))
                {
                    dialogueManager.OptionClicked(4.0f);
                    Debug.Log("option 4");
                }
            }
        }

        // ── Hover thoughts (crosshair-based) ─────────────────────────────
        // When in the 3D office, detect what the player is looking at and
        // show a brief contextual thought after they look at it for ~0.6 s.
        if (onOfficeCam && playerThoughts != null)
        {
            Ray hoverRay = playerCamera.ScreenPointToRay(
                new UnityEngine.Vector3(Screen.width * 0.5f, Screen.height * 0.5f));

            RaycastHit hoverHit;
            string newTag  = "";
            string newName = "";
            Collider newCol = null;

            if (Physics.Raycast(hoverRay, out hoverHit, 15f))
            {
                newTag  = hoverHit.collider.tag;
                newName = hoverHit.collider.gameObject.name;
                newCol  = hoverHit.collider;
            }

            // Reset timer whenever the look-target changes
            if (newTag != _lastHoverTag || newName != _lastHoverName)
            {
                _lastHoverTag       = newTag;
                _lastHoverName      = newName;
                _hoverTimer         = 0f;
                _hoverThoughtShown  = false;
            }
            else if (!_hoverThoughtShown && newTag != "")
            {
                _hoverTimer += Time.deltaTime;
                if (_hoverTimer > 0.6f)
                {
                    _hoverThoughtShown = true;
                    ShowHoverThought(newTag, newCol);
                }
            }
        }

        // ── Crouch ───────────────────────────────────────────────────────
        if (Input.GetKey(KeyCode.LeftControl) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed  = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed  = 12f;
        }

        // ── Apply movement ────────────────────────────────────────────────
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX  = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    // =====================================================================
    // HOVER THOUGHT HELPER
    // =====================================================================

    private void ShowHoverThought(string tag, Collider col)
    {
        if (playerThoughts == null) return;
        var gsm = GameStateManager.Instance;

        if (tag == "Interview" && col != null)
        {
            var npc = col.GetComponentInParent<InterviewBase>();
            if (npc == null) return;

            // Pull just the character's first name (before the colon)
            string name = npc.CharacterName.Contains(":")
                ? npc.CharacterName.Split(':')[0].Trim()
                : npc.CharacterName;

            if (gsm != null)
            {
                if (gsm.IsInterviewPaused(npc))
                    playerThoughts.ShowThought($"I should finish my interview with {name}...");
                else if (gsm.IsAlreadyInterviewed(npc))
                    playerThoughts.ShowThought($"Already talked to {name}. Nothing more to ask.");
                else if (!gsm.IsNPCAvailable(npc))
                    playerThoughts.ShowThought($"{name} doesn\u2019t seem available right now.");
                else
                    playerThoughts.ShowThought($"I should talk to {name} \u2014 click to approach.");
            }
        }
        else if (tag == "Desk")
        {
            if (gsm != null && gsm.WaitingForDesk)
                playerThoughts.ShowThought("My article won\u2019t write itself\u2026 I should sit down.");
            else if (gsm != null && gsm.InterviewPaused)
                playerThoughts.ShowThought("I need to finish my interview before I can write.");
            else
                playerThoughts.ShowThought("Nothing to write yet.");
        }
    }
}
