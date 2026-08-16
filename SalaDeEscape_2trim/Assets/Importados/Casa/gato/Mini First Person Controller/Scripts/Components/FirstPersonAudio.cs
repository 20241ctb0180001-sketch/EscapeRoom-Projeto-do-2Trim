using System.Linq;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FirstPersonAudio : MonoBehaviour
{
    public FirstPersonMovement character;
    public GroundCheck groundCheck;

    [Header("Step (FMOD events)")]
    public string stepEvent = "event:/footstep/step";
    public string runningEvent = "event:/footstep/run";
    [Tooltip("Minimum velocity for moving audio to play")]
    public float velocityThreshold = .01f;
    Vector2 lastCharacterPosition;
    Vector2 CurrentCharacterPosition => new Vector2(character.transform.position.x, character.transform.position.z);

    [Header("Landing")]
    public string landingEvent = "event:/player/land";

    [Header("Jump")]
    public Jump jump;
    public string jumpEvent = "event:/player/jump";

    [Header("Crouch")]
    public Crouch crouch;
    public string crouchStartEvent = "event:/player/crouch_start";
    public string crouchedEvent = "event:/player/crouch_loop";
    public string crouchEndEvent = "event:/player/crouch_end";

    // FMOD event instances for looping moving sounds
    EventInstance movingInstance;
    string currentMovingEventPath;


    void Reset()
    {
        // Setup references only; FMOD events are set by path strings.
        character = GetComponentInParent<FirstPersonMovement>();
        groundCheck = (transform.parent ?? transform).GetComponentInChildren<GroundCheck>();
        jump = GetComponentInParent<Jump>();
        crouch = GetComponentInParent<Crouch>();
    }

    void OnEnable() => SubscribeToEvents();

    void OnDisable() => UnsubscribeToEvents();

    void FixedUpdate()
    {
        // Play moving audio if the character is moving and on the ground.
        float velocity = Vector3.Distance(CurrentCharacterPosition, lastCharacterPosition);
        if (velocity >= velocityThreshold && groundCheck && groundCheck.isGrounded)
        {
            if (crouch && crouch.IsCrouched)
            {
                SetMovingEvent(crouchedEvent);
            }
            else if (character && character.IsRunning)
            {
                SetMovingEvent(runningEvent);
            }
            else
            {
                SetMovingEvent(stepEvent);
            }
        }
        else
        {
            StopMovingEvent();
        }

        // Remember lastCharacterPosition.
        lastCharacterPosition = CurrentCharacterPosition;
    }


    /// <summary>
    /// Pause all MovingAudios and enforce play on audioToPlay.
    /// </summary>
    /// <param name="audioToPlay">Audio that should be playing.</param>
    void SetMovingEvent(string eventPath)
    {
        if (string.IsNullOrEmpty(eventPath))
        {
            StopMovingEvent();
            return;
        }

        // Prevent the walking step loop from continuing while sprinting.
        if (character && character.IsRunning && eventPath == stepEvent)
        {
            StopMovingEvent();
            return;
        }

        if (currentMovingEventPath == eventPath && movingInstance.isValid())
            return; // already playing

        StopMovingEvent();
        movingInstance = RuntimeManager.CreateInstance(eventPath);
        movingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        movingInstance.start();
        currentMovingEventPath = eventPath;
    }

    void StopMovingEvent()
    {
        if (movingInstance.isValid())
        {
            movingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            movingInstance.release();
        }
        movingInstance = default;
        currentMovingEventPath = null;
    }

    #region Play instant-related audios.
    void PlayLandingAudio() => RuntimeManager.PlayOneShot(landingEvent, transform.position);
    void PlayJumpAudio() => RuntimeManager.PlayOneShot(jumpEvent, transform.position);
    void PlayCrouchStartAudio() => RuntimeManager.PlayOneShot(crouchStartEvent, transform.position);
    void PlayCrouchEndAudio() => RuntimeManager.PlayOneShot(crouchEndEvent, transform.position);
    #endregion

    #region Subscribe/unsubscribe to events.
    void SubscribeToEvents()
    {
        // PlayLandingAudio when Grounded.
        groundCheck.Grounded += PlayLandingAudio;

        // PlayJumpAudio when Jumped.
        if (jump)
        {
            jump.Jumped += PlayJumpAudio;
        }

        // Play crouch audio on crouch start/end.
        if (crouch)
        {
            crouch.CrouchStart += PlayCrouchStartAudio;
            crouch.CrouchEnd += PlayCrouchEndAudio;
        }
    }

    void UnsubscribeToEvents()
    {
        // Undo PlayLandingAudio when Grounded.
        groundCheck.Grounded -= PlayLandingAudio;

        // Undo PlayJumpAudio when Jumped.
        if (jump)
        {
            jump.Jumped -= PlayJumpAudio;
        }

        // Undo play crouch audio on crouch start/end.
        if (crouch)
        {
            crouch.CrouchStart -= PlayCrouchStartAudio;
            crouch.CrouchEnd -= PlayCrouchEndAudio;
        }
    }
    #endregion

    #region Utility.
    /// <summary>
    /// Get an existing AudioSource from a name or create one if it was not found.
    /// </summary>
    /// <param name="name">Name of the AudioSource to search for.</param>
    /// <returns>The created AudioSource.</returns>
    void OnDestroy()
    {
        StopMovingEvent();
    }
    #endregion 
}
