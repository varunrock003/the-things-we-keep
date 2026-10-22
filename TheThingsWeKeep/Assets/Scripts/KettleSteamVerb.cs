using UnityEngine;
using UnityEngine.Events;

// Steam verb for the kettle. Listens to the PossessableObject on the same
// GameObject: possess the kettle with E, press Space, it steams.
// The look of the steam itself lives on the SteamFX particle system.
public class KettleSteamVerb : MonoBehaviour
{
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    [Header("Steam")]
    public ParticleSystem steamParticles;
    [Tooltip("Seconds of steam per use. Using it again restarts the timer.")]
    public float steamDuration = 5f;
    [Tooltip("Seconds over which the steam tapers off before stopping.")]
    public float tailTime = 1.2f;
    [Tooltip("Extra puffs on every press, so each use reads clearly.")]
    public int burstCount = 10;

    [Header("Kettle Feedback")]
    public float rattleAngle = 3f;
    public float rattleSpeed = 24f;
    public Color glowTint = new Color(1f, 0.85f, 0.68f);
    [Range(0f, 1f)]
    public float glowStrength = 0.35f;
    public float feedbackFadeTime = 0.4f;

    [Header("Events")]
    public UnityEvent onSteamStarted;
    public UnityEvent onSteamStopped;

    public bool IsSteaming { get; private set; }

    private Renderer objectRenderer;
    private MaterialPropertyBlock propertyBlock;
    private Color baseColor;
    private Quaternion restRotation;
    private float baseEmissionRate;
    private float steamTimer;
    private float feedbackWeight;

    void Awake()
    {
        restRotation = transform.localRotation;
        objectRenderer = GetComponent<Renderer>();
        baseColor = objectRenderer.sharedMaterial.GetColor(BaseColorId);
        propertyBlock = new MaterialPropertyBlock();
        baseEmissionRate = steamParticles.emission.rateOverTimeMultiplier;

        PossessableObject possessable = GetComponent<PossessableObject>();
        if (possessable != null)
            possessable.onVerbUsed.AddListener(StartSteam);
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

    public void StartSteam()
    {
        steamTimer = steamDuration;
        SetEmission(1f);
        steamParticles.Play();
        steamParticles.Emit(burstCount);

        if (IsSteaming)
            return;

        IsSteaming = true;
        onSteamStarted.Invoke();
    }

    public void StopSteam()
    {
        if (!IsSteaming)
            return;

        IsSteaming = false;
        // StopEmitting lets the steam already in the air drift up and fade out.
        steamParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        onSteamStopped.Invoke();
    }

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

        propertyBlock.SetColor(BaseColorId,
            Color.Lerp(baseColor, glowTint, glowStrength * feedbackWeight));
        objectRenderer.SetPropertyBlock(propertyBlock);
    }
}
