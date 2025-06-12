using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Sound 
    private AudioSource source;
    public AudioClip[] walkSounds;
    public AudioClip LandingSound;
    public AudioClip JumpSound;

    private bool ismoving = false;
    private bool iscrouching = false;
    private bool isrunning = false;
    private bool landed = true;
    private float counter = 0;
    [SerializeField] private float timebetweensounds = 1f;
    private float timer = 0;
    [SerializeField] private float ladingCooldown = 0.5f;
    private bool landingCooldownActive = false;

    // Player movement
    public float movespeed;
    public float sensitivity;
    public float jumpforse;
    public float gravity;
    private float mouseX;
    private float mouseY;
    private float vertical;
    private float horizontal;
    [SerializeField] private float walkingVolume = 0.2f;
    [SerializeField] private float runningVolume = 0.4f;
    [SerializeField] private float crouchingVolume = 0.1f;
    [SerializeField] private float jumpVolume = 0.1f;
    [SerializeField] private float landingVolume = 0.5f;

    public Vector2 clampangle;
    private Vector3 Velocity;
    private Vector2 angle;
    public Transform cameraTransform; 

    private CharacterController charactercontroller;
    private Animator animator;

    private bool isCrouching = false;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        charactercontroller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (landingCooldownActive)
        {
            timer += Time.deltaTime;
            if (timer >= ladingCooldown)
            {
                landingCooldownActive = false;
                timer = 0;
            }
        }
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        Vector3 playerMovementInput = new Vector3(horizontal, 0.0f, vertical);
        Vector3 moveVector = transform.TransformDirection(playerMovementInput);

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0 && !isCrouching)
            {
                movespeed = 6;
                animator.SetInteger("state", 2); // Running
            }
            else if (!isCrouching)
            {
                movespeed = 3f;
                animator.SetInteger("state", 1); // Walking
            }
        }
        else
        {
            movespeed = 0;
            animator.SetInteger("state", 0); // Idle
        }

        if (charactercontroller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                landed = false;
                PlayJumpSound();
                Velocity.y = jumpforse;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                movespeed = 2;
                transform.localScale = new Vector3(1, 0.5f, 1);
                isCrouching = true;
                animator.SetInteger("state", 0); // Crouching
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
                isCrouching = false;
            }

            if (!landed && Velocity.y < 0)
            {
                PlayLandingSound();
                landed = true;
            }
        }
        else
        {
            Velocity.y -= gravity * Time.deltaTime;
            //landed = false;
        }

        charactercontroller.Move(moveVector * movespeed * Time.deltaTime);
        charactercontroller.Move(Velocity * Time.deltaTime);

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        angle.x -= mouseY * sensitivity;
        angle.y += mouseX * sensitivity;

        angle.x = Mathf.Clamp(angle.x, -clampangle.x, clampangle.y);

        Quaternion rotation = Quaternion.Euler(angle.x, angle.y, 0.0f);
        Quaternion rotationTwo = Quaternion.Euler(0.0f, angle.y, 0.0f);
        transform.rotation = rotationTwo;
        cameraTransform.rotation = rotation;

        // Sound logic
        if (charactercontroller.isGrounded && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0 && !Input.GetKey(KeyCode.LeftControl))
            {
                ismoving = false;
                isrunning = true;
                iscrouching = false;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                iscrouching = true;
                ismoving = false;
                isrunning = false;
            }
            else
            {
                ismoving = true;
                isrunning = false;
                iscrouching = false;
            }
        }
        else
        {
            ismoving = false;
            isrunning = false;
            iscrouching = false;
        }

        if (ismoving && charactercontroller.isGrounded && !landingCooldownActive)
        {
            if (Time.time > counter)
            {
                source.volume = walkingVolume;
                source.PlayOneShot(walkSounds[Random.Range(0, walkSounds.Length)]);
                counter = Time.time + timebetweensounds;
            }
        }

        if (isrunning && charactercontroller.isGrounded)
        {
            if (Time.time > counter)
            {
                source.volume = runningVolume;
                source.PlayOneShot(walkSounds[Random.Range(0, walkSounds.Length)]);
                counter = Time.time + timebetweensounds / 2f;
            }
        }

        if (iscrouching && charactercontroller.isGrounded)
        {
            if (Time.time > counter)
            {
                source.volume = crouchingVolume;
                source.PlayOneShot(walkSounds[Random.Range(0, walkSounds.Length)]);
                counter = Time.time + timebetweensounds * 1.5f;
            }
        }
    }

    private void PlayJumpSound()
    {
        source.volume = jumpVolume;
        source.PlayOneShot(JumpSound);
    }

    private void PlayLandingSound()
    {
        source.volume = landingVolume;
        source.PlayOneShot(LandingSound);
    }
}