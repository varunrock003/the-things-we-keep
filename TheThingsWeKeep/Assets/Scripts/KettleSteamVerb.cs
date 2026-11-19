using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Steam verb for the kettle. Listens to the <see cref="PossessableObject"/> on
/// the same GameObject: possess the kettle with E, press Space, and it steams.
/// </summary>
/// <remarks>
/// The look of the steam itself lives on the SteamFX particle system. This
/// script controls when it emits, tapers the emission off at the end, and adds
/// a lid rattle and warm tint on the kettle so the verb reads from the fixed
/// camera. Pressing Space while steaming restarts the timer.
/// </remarks>
public class KettleSteamVerb : MonoBehaviour
{
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    [Header("Steam")]
    [Tooltip("SteamFX particle system. Leave Play On Awake off on it.")]
    public ParticleSystem steamParticles;
    [Tooltip("Seconds of steam per use. Using it again restarts the timer.")]
    [Min(0.1f)]
    public float steamDuration = 5f;
    [Tooltip("Seconds over which the steam tapers off before stopping.")]
    [Min(0.01f)]
    public float tailTime = 1.2f;
    [Tooltip("Extra puffs on every press, so each use reads clearly.")]
    [Min(0)]
    public int burstCount = 10;
    [Tooltip("Minimum seconds between bursts, so mashing Space can't flood the particle system.")]
    [Min(0f)]
    public float burstCooldown = 0.5f;

    [Header("Kettle Feedback")]
    public float rattleAngle = 3f;
    public float rattleSpeed = 24f;
    public Color glowTint = new Color(1f, 0.85f, 0.68f);
    [Range(0f, 1f)]
    public float glowStrength = 0.35f;
    [Min(0.01f)]
    public float feedbackFadeTime = 0.4f;

    [Header("Events")]
    [Tooltip("Invoked when steam starts from idle (not on a restart).")]
    public UnityEvent onSteamStarted;
    [Tooltip("Invoked when the steam timer runs out.")]
    public UnityEvent onSteamStopped;

    /// <summary>True while the steam timer is running.</summary>
    public bool IsSteaming { get; private set; }

    private Renderer objectRenderer;
    private MaterialPropertyBlock propertyBlock;
    private Color baseColor;
    private Quaternion restRotation;
    private float baseEmissionRate;
    private float steamTimer;
    private float feedbackWeight;
    private float lastBurstTime = float.NegativeInfinity;

    void Awake()
    {
        restRotation = transform.localRotation;
        propertyBlock = new MaterialPropertyBlock();

        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null && objectRenderer.sharedMaterial.HasProperty(BaseColorId))
            baseColor = objectRenderer.sharedMaterial.GetColor(BaseColorId);

        if (steamParticles != null)
            baseEmissionRate = steamParticles.emission.rateOverTimeMultiplier;
        else
            Debug.LogWarning("KettleSteamVerb on " + name + " has no Steam Particles assigned.", this);

        PossessableObject possessable = GetComponent<PossessableObject>();
        if (possessable != null)
            possessable.onVerbUsed.AddListener(StartSteam);
    }

    void OnDestroy()
    {
        PossessableObject possessable = GetComponent<PossessableObject>();
        if (possessable != null)
            possessable.onVerbUsed.RemoveListener(StartSteam);
    }

    void OnDisable()
    {
        // Don't leave the kettle tilted or tinted if it's disabled mid-rattle.
        feedbackWeight = 0f;
        transform.localRotation = restRotation;
        if (objectRenderer != null)
            objectRenderer.SetPropertyBlock(null);
    }

    void Update()
    {
        if (IsSteaming)
        {
            steamTimer -= Time.deltaTime;
            if (steamTimer <= 0f)
                StopSteam();
            else
                SetEmission(Mathf.Clamp01(steamTimer / tailTime));
        }

        UpdateFeedback();
    }

    /// <summary>
    /// Starts the steam, or restarts the timer at full strength if it is
    /// already steaming. Each call also emits an extra burst of
    /// <see cref="burstCount"/> puffs, at most once per
    /// <see cref="burstCooldown"/> seconds. Fires <see cref="onSteamStarted"/>
    /// only when starting from idle.
    /// </summary>
    public void StartSteam()
    {
        if (steamParticles == null)
            return;

        steamTimer = steamDuration;
        SetEmission(1f);
        if (!steamParticles.isPlaying)
            steamParticles.Play();

        if (Time.time - lastBurstTime >= burstCooldown)
        {
            steamParticles.Emit(burstCount);
            lastBurstTime = Time.time;
        }

        if (IsSteaming)
            return;

        IsSteaming = true;
        onSteamStarted.Invoke();
    }

    /// <summary>
    /// Stops emitting new steam. Particles already in the air finish their
    /// lifetime and fade out. Fires <see cref="onSteamStopped"/>. Does nothing
    /// if the kettle isn't steaming.
    /// </summary>
    public void StopSteam()
    {
        if (!IsSteaming)
            return;

        IsSteaming = false;
        if (steamParticles != null)
            steamParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        onSteamStopped.Invoke();
    }

    // Scales the SteamFX emission rate; 1 = the rate set on the particle system.
    void SetEmission(float strength)
    {
        ParticleSystem.EmissionModule emission = steamParticles.emission;
        emission.rateOverTimeMultiplier = baseEmissionRate * strength;
    }

    // Lid rattle and a warm tint while boiling, so the verb reads from the fixed camera.
    void UpdateFeedback()
    {
        float target = IsSteaming ? 1f : 0f;
        if (feedbackWeight == 0f && target == 0f)
            return;

        feedbackWeight = Mathf.MoveTowards(
            feedbackWeight, target, Time.deltaTime / feedbackFadeTime);

        float wobble = Mathf.Sin(Time.time * rattleSpeed)
            + 0.5f * Mathf.Sin(Time.time * rattleSpeed * 1.7f);
        float rattle = wobble / 1.5f * rattleAngle * feedbackWeight;
        transform.localRotation =
            restRotation * Quaternion.Euler(rattle * 0.5f, 0f, rattle);

        if (objectRenderer == null)
            return;
        // Lazy so a script reload during Play Mode can't leave it null.
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor(BaseColorId,
            Color.Lerp(baseColor, glowTint, glowStrength * feedbackWeight));
        objectRenderer.SetPropertyBlock(propertyBlock);
    }
}
