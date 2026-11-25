using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Editor tool that builds Hassan's Milestone 3 test scene. It copies
/// KitchenIntegrated to FeedbackTest_Hassan (KitchenIntegrated itself is not
/// modified), adds a possessable lamp with the Reveal verb and a hidden clue,
/// registers the lamp with the Spirit's PossessionController, and saves the
/// lamp as the self-contained prefab PF_LampReveal.
/// </summary>
public static class FeedbackTestSceneBuilder
{
    const string SourceScene = "Assets/Scenes/KitchenIntegrated.unity";
    const string TestScene = "Assets/Scenes/FeedbackTest_Hassan.unity";
    const string PrefabPath = "Assets/Prefabs/PF_LampReveal.prefab";

    const string ClueText = "Some memories shine when we share them.";

    [MenuItem("Tools/Hassan/Build Feedback Test Scene")]
    public static void Build()
    {
        if (File.Exists(TestScene))
        {
            if (!EditorUtility.DisplayDialog("Feedback Test Scene",
                    TestScene + " already exists. Rebuild it from KitchenIntegrated? " +
                    "Changes made in the test scene will be lost.",
                    "Rebuild", "Cancel"))
                return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.OpenScene(SourceScene, OpenSceneMode.Single);
        // Re-path the open scene as the test scene so KitchenIntegrated stays untouched.
        if (!EditorSceneManager.SaveScene(scene, TestScene))
        {
            Debug.LogError("Could not save " + TestScene);
            return;
        }
        scene = EditorSceneManager.OpenScene(TestScene, OpenSceneMode.Single);

        GameObject lamp = BuildLamp();
        RegisterWithSpirit(lamp.GetComponent<PossessableObject>());

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        PrefabUtility.SaveAsPrefabAssetAndConnect(lamp, PrefabPath, InteractionMode.AutomatedAction);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Selection.activeGameObject = lamp;
        SceneView.FrameLastActiveSceneView();
        Debug.Log("Built " + TestScene + " with LampTest and " + PrefabPath + ".");
    }

    const string SwitchOnClip = "Assets/Audio/lamp_switch_on.wav";
    const string SwitchOffClip = "Assets/Audio/lamp_switch_off.wav";
    const string BuzzClip = "Assets/Audio/lamp_buzz_loop.wav";

    /// <summary>
    /// Adds the switch click and looping buzz to PF_LampReveal. Edits the
    /// prefab asset, so every lamp instance (including the test scene's) gets it.
    /// </summary>
    [MenuItem("Tools/Hassan/Wire Lamp Audio")]
    public static void WireLampAudio()
    {
        AudioClip on = AssetDatabase.LoadAssetAtPath<AudioClip>(SwitchOnClip);
        AudioClip off = AssetDatabase.LoadAssetAtPath<AudioClip>(SwitchOffClip);
        AudioClip buzz = AssetDatabase.LoadAssetAtPath<AudioClip>(BuzzClip);
        if (on == null || off == null || buzz == null)
        {
            Debug.LogError("Lamp audio clips not found in Assets/Audio.");
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
        try
        {
            AudioSource[] sources = root.GetComponents<AudioSource>();
            AudioSource switchSource = sources.Length > 0 ? sources[0] : root.AddComponent<AudioSource>();
            AudioSource buzzSource = sources.Length > 1 ? sources[1] : root.AddComponent<AudioSource>();

            switchSource.playOnAwake = false;
            switchSource.loop = false;
            switchSource.spatialBlend = 0f;
            switchSource.clip = on;

            buzzSource.playOnAwake = false;
            buzzSource.loop = true;
            buzzSource.spatialBlend = 0f;
            buzzSource.volume = 0f;
            buzzSource.clip = buzz;

            var so = new SerializedObject(root.GetComponent<LampRevealVerb>());
            so.FindProperty("switchSource").objectReferenceValue = switchSource;
            so.FindProperty("switchOnClip").objectReferenceValue = on;
            so.FindProperty("switchOffClip").objectReferenceValue = off;
            so.FindProperty("buzzSource").objectReferenceValue = buzzSource;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
        Debug.Log("Wired lamp audio into " + PrefabPath + ".");
    }

    static GameObject BuildLamp()
    {
        Material wood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Wood_Dark.mat");
        Material shadeMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Light_Warm.mat");

        // Root stays unscaled so child text and light aren't distorted.
        var lamp = new GameObject("LampTest");
        lamp.transform.position = new Vector3(-2f, 0.6f, -1f);

        GameObject body = Primitive(PrimitiveType.Cylinder, "LampBody", lamp.transform,
            Vector3.zero, new Vector3(0.3f, 0.6f, 0.3f), wood);
        GameObject shade = Primitive(PrimitiveType.Sphere, "LampShade", lamp.transform,
            new Vector3(0f, 0.6f, 0f), new Vector3(0.45f, 0.24f, 0.45f), shadeMat);

        var lightGo = new GameObject("LampLight");
        lightGo.transform.SetParent(lamp.transform, false);
        lightGo.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        Light light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.78f, 0.45f);
        light.range = 5f;
        light.intensity = 3f;
        light.shadows = LightShadows.None;
        // GameObject stays active; LampRevealVerb switches the component off on Start.

        var message = new GameObject("HiddenMessage");
        message.transform.SetParent(lamp.transform, false);
        message.transform.localPosition = new Vector3(0f, 1.7f, 0f);
        // Tilted to match the fixed camera's ~25 degree downward angle.
        message.transform.localRotation = Quaternion.Euler(25f, 0f, 0f);
        TextMeshPro text = message.AddComponent<TextMeshPro>();
        text.text = ClueText;
        text.fontSize = 3f;
        text.color = new Color(1f, 0.96f, 0.86f);
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.rectTransform.sizeDelta = new Vector2(4f, 1.2f);
        message.SetActive(false);

        PossessableObject possessable = lamp.AddComponent<PossessableObject>();
        possessable.objectName = "Lamp";
        possessable.verbName = "Reveal";

        LampRevealVerb reveal = lamp.AddComponent<LampRevealVerb>();
        var so = new SerializedObject(reveal);
        so.FindProperty("lampLight").objectReferenceValue = light;
        so.FindProperty("hiddenMessage").objectReferenceValue = message;
        so.FindProperty("shadeRenderer").objectReferenceValue = shade.GetComponent<Renderer>();
        so.ApplyModifiedPropertiesWithoutUndo();

        AudioSource audio = lamp.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
        audio.spatialBlend = 0f;

        return lamp;
    }

    static GameObject Primitive(PrimitiveType type, string name, Transform parent,
        Vector3 localPos, Vector3 localScale, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = localScale;
        Object.DestroyImmediate(go.GetComponent<Collider>());
        if (mat != null)
            go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    static void RegisterWithSpirit(PossessableObject lamp)
    {
        PossessionController controller = Object.FindFirstObjectByType<PossessionController>();
        if (controller == null)
        {
            Debug.LogError("No PossessionController found; add LampTest to Objects In Room manually.");
            return;
        }

        var so = new SerializedObject(controller);
        SerializedProperty list = so.FindProperty("objectsInRoom");
        int index = list.arraySize;
        list.InsertArrayElementAtIndex(index);
        list.GetArrayElementAtIndex(index).objectReferenceValue = lamp;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
