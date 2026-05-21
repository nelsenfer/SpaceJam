using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class ShadowFixer
{
    [MenuItem("Tools/Fix Shadow Re-Add Component")]
    public static void FixReAdd()
    {
        ShadowCaster2D[] casters = Object.FindObjectsByType<ShadowCaster2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (ShadowCaster2D caster in casters)
        {
            GameObject go = caster.gameObject;

            // Simpan nilai dulu
            bool selfShadows = caster.selfShadows;
            bool castsShadows = caster.castsShadows;

            // Remove
            Object.DestroyImmediate(caster);

            // Re-add
            ShadowCaster2D newCaster = go.AddComponent<ShadowCaster2D>();
            newCaster.selfShadows = selfShadows;
            newCaster.castsShadows = castsShadows;

            EditorUtility.SetDirty(go);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );

        Debug.Log("Done re-add Shadow Caster!");
    }
}