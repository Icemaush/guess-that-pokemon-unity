using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// This script ensures the first scene defined in build settings (if any) is always loaded when entering play mode.
/// </summary>
[InitializeOnLoad]
public class AutoPlayModeSceneSetup
{
    static AutoPlayModeSceneSetup()
    {
        // Execute once on Unity editor startup.
        SetStartScene();

        // Also execute whenever build settings change.
        EditorBuildSettings.sceneListChanged += SetStartScene;
    }

    static void SetStartScene()
    {
        // At least one scene in build settings?
        if (EditorBuildSettings.scenes.Length > 0)
        {
            // Set start scene to first scene in build settings.
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
        }
        else
        {
            // Reset start scene.
            EditorSceneManager.playModeStartScene = null;
        }
    }
}