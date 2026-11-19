using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Reveal verb for the lamp. Listens to the <see cref="PossessableObject"/> on
/// the same GameObject: possess the lamp with E, press Space, and the lamp
/// fades on and uncovers a hidden clue.
/// </summary>
/// <remarks>
/// The lamp's Light component is switched by this script, so leave the
/// LampLight GameObject itself active in the Hierarchy. The script turns the
/// light off on Start. Do not also wire <see cref="ActivateReveal"/> into the
/// PossessableObject's onVerbUsed event in the Inspector, because the script
/// subscribes itself and the reveal would run twice.
/// </remarks>
[RequireComponent(typeof(PossessableObject))]
public class LampRevealVerb : MonoBehaviour
{
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    [Header("Lamp References")]
    [Tooltip("Light switched on by Reveal. Keep its GameObject active.")]
    [SerializeField] private Light lampLight;
    [Tooltip("Clue object shown by Reveal. Starts hidden.")]
    [SerializeField] private GameObject hiddenMessage;
    [Tooltip("Optional lamp shade renderer that glows while the lamp is on.")]
    [SerializeField] private Renderer shadeRenderer;

    [Header("Settings")]
    [Tooltip("When on, Reveal works once until ResetReveal is called.")]
    [SerializeField] private bool revealOnce = true;
    [Tooltip("Light intensity once fully on.")]
    [Min(0f)]
    [SerializeField] private float onIntensity = 3f;
    [Tooltip("Seconds for the light and the clue to fade in.")]
    [Min(0.01f)]
    [SerializeField] private float fadeTime = 0.6f;
    [Tooltip("Seconds of flicker when Reveal is pressed again after revealing.")]
    [Min(0f)]
    [SerializeField] private float repeatFlickerTime = 0.3f;

    [Header("Audio")]
    [Tooltip("One-shot source for the switch click. Leave Play On Awake off.")]
    [SerializeField] private AudioSource switchSource;
    [Tooltip("Click played when the lamp switches on or is pressed again.")]
    [SerializeField] private AudioClip switchOnClip;
    [Tooltip("Click played when the lamp is reset (switched off).")]
    [SerializeField] private AudioClip switchOffClip;
    [Tooltip("Looping source for the electrical hum while the lamp is on.")]
    [SerializeField] private AudioSource buzzSource;
    [Tooltip("Hum volume once the lamp is fully on; fades with the light.")]
    [Range(0f, 1f)]
    [SerializeField] private float buzzVolume = 0.35f;

    [Header("Testing")]
    [Tooltip("Lets the R key call ResetReveal in Play Mode. Turn off for the final build.")]
    [SerializeField] private bool debugResetKey = true;

    [Header("Shade Glow")]
    [SerializeField] private Color shadeGlowTint = new Color(1f, 0.9f, 0.6f);
    [Range(0f, 1f)]
    [SerializeField] private float shadeGlowStrength = 0.6f;
    [Tooltip("Shade brightness while the lamp is off (1 = material colour).")]
    [Range(0f, 1f)]
    [SerializeField] private float shadeOffBrightness = 0.55f;

    [Header("Events")]
    [Tooltip("Invoked once per successful reveal (sound is handled by the Audio fields).")]
    public UnityEvent onRevealed;
    [Tooltip("Invoked when the lamp is reset to its hidden state.")]
    public UnityEvent onReset;

    /// <summary>True after a reveal, until <see cref="ResetReveal"/> runs.</summary>
    public bool HasRevealed { get; private set; }

    private PossessableObject possessable;
    private TMP_Text messageText;
    private MaterialPropertyBlock propertyBlock;
    private Color shadeBaseColor;
    private float revealWeight;
    private float flickerTimer;

    private void Awake()
    {
        possessable = GetComponent<PossessableObject>();
        // The possession system invokes this event when Space is pressed.
        possessable.onVerbUsed.AddListener(ActivateReveal);

        if (hiddenMessage != null)
            messageText = hiddenMessage.GetComponentInChildren<TMP_Text>(true);

        if (shadeRenderer != null)
        {
            propertyBlock = new MaterialPropertyBlock();
            shadeBaseColor = shadeRenderer.sharedMaterial.HasProperty(BaseColorId)
                ? shadeRenderer.sharedMaterial.GetColor(BaseColorId)
                : Color.white;
        }

        if (lampLight == null)
            Debug.LogWarning("LampRevealVerb on " + name + " has no Lamp Light assigned.", this);
        if (hiddenMessage == null)
            Debug.LogWarning("LampRevealVerb on " + name + " has no Hidden Message assigned.", this);
    }

    private void Start()
    {
        ResetReveal();
    }

    private void Update()
    {
        if (debugResetKey && Input.GetKeyDown(KeyCode.R))
            ResetReveal();

        float target = HasRevealed ? 1f : 0f;
        if (revealWeight != target)
            revealWeight = Mathf.MoveTowards(revealWeight, target, Time.deltaTime / fadeTime);

        float flicker = 1f;
        if (flickerTimer > 0f)
        {
            flickerTimer -= Time.deltaTime;
            // Quick dip-and-return so a repeated press visibly registers.
            flicker = 0.35f + 0.65f * Mathf.Abs(Mathf.Cos(flickerTimer * 40f));
        }

        ApplyVisuals(revealWeight * flicker, revealWeight);
    }

    /// <summary>
    /// Switches on the lamp and reveals the clue. Called through the
    /// PossessableObject's onVerbUsed event. With Reveal Once enabled, later
    /// calls do not reveal again; they only flicker the lamp as feedback.
    /// </summary>
    public void ActivateReveal()
    {
        if (revealOnce && HasRevealed)
        {
            flickerTimer = repeatFlickerTime;
            PlaySwitch(switchOnClip);
            Debug.Log("Lamp Reveal already active.");
            return;
        }

        HasRevealed = true;

        if (lampLight != null)
            lampLight.enabled = true;
        if (hiddenMessage != null)
            hiddenMessage.SetActive(true);

        PlaySwitch(switchOnClip);
        if (buzzSource != null && !buzzSource.isPlaying)
        {
            buzzSource.loop = true;
            buzzSource.volume = 0f;
            buzzSource.Play();
        }

        Debug.Log("Lamp Reveal activated.");
        onRevealed?.Invoke();
    }

    /// <summary>
    /// Restores the lamp to its initial state: light off, clue hidden, and
    /// Reveal available again. Also available from the component's context
    /// menu in the Inspector, including during Play Mode.
    /// </summary>
    [ContextMenu("Reset Reveal")]
    public void ResetReveal()
    {
        HasRevealed = false;
        revealWeight = 0f;
        flickerTimer = 0f;

        ApplyVisuals(0f, 0f);
        if (lampLight != null)
            lampLight.enabled = false;
        if (hiddenMessage != null)
            hiddenMessage.SetActive(false);

        if (buzzSource != null)
            buzzSource.Stop();

        // Skip the click and log for the silent reset in Start.
        if (Application.isPlaying && Time.frameCount > 1)
        {
            PlaySwitch(switchOffClip);
            Debug.Log("Lamp Reveal reset.");
        }
        onReset?.Invoke();
    }

    private void PlaySwitch(AudioClip clip)
    {
        if (switchSource != null && clip != null)
            switchSource.PlayOneShot(clip);
    }

    // Applies light intensity, clue alpha and shade glow for the current fade.
    private void ApplyVisuals(float lightWeight, float messageWeight)
    {
        if (lampLight != null)
            lampLight.intensity = onIntensity * lightWeight;

        if (messageText != null)
            messageText.alpha = messageWeight;

        if (buzzSource != null)
            buzzSource.volume = buzzVolume * lightWeight;

        if (shadeRenderer != null)
        {
            // Dim shade while off, warm bright shade while on.
            Color offColor = shadeBaseColor * shadeOffBrightness;
            offColor.a = shadeBaseColor.a;
            Color onColor = Color.Lerp(shadeBaseColor, shadeGlowTint, shadeGlowStrength);
            // Lazy so a script reload during Play Mode can't leave it null.
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor(BaseColorId, Color.Lerp(offColor, onColor, lightWeight));
            shadeRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void OnDestroy()
    {
        if (possessable != null)
            possessable.onVerbUsed.RemoveListener(ActivateReveal);
    }
}
