using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO; // for Path.GetFileNameWithoutExtension

public class LevelLoader : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] string nextSceneName = "Transition_S2";
    [SerializeField] string playerTag = "Player";
    [Tooltip("Identifier telling the NEXT scene where we came from (must match a SpawnPoint.entryId in the next scene).")]
    [SerializeField] string exitIdForNextScene = "FromScene1";

    [Header("Fade (Animator on your CrossFade canvas)")]
    [SerializeField] Animator crossFadeAnimator;      // assign your CrossFade canvas Animator
    [SerializeField] AnimationClip fadeOutClip;       // to black (e.g., CrossFade_End)
    [SerializeField] AnimationClip fadeInClip;        // from black (e.g., CrossFade_Start)

    bool isLoading;

    void Awake()
    {
        if (!crossFadeAnimator)
            crossFadeAnimator = FindObjectOfType<Animator>(); // safe fallback if only one Animator in scene

        // Play fade-in on scene start
        if (crossFadeAnimator && fadeInClip)
        {
            Debug.Log($"[LevelLoader] Scene start fade-in: {fadeInClip.name}");
            crossFadeAnimator.Play(fadeInClip.name, 0, 0f);
        }

        // Sanity check: is target scene in Build Settings?
        int idx = GetBuildIndexByName(nextSceneName);
        if (idx < 0)
            Debug.LogWarning($"[LevelLoader] Target scene '{nextSceneName}' is not in Build Settings.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (isLoading) return;
        if (!other.CompareTag(playerTag)) return;

        // Remember which gate we used so the next scene can spawn us at the right entrance
        TransitionMemory.LastExitId = exitIdForNextScene;

        Debug.Log($"[LevelLoader] Triggered by {other.name} → loading '{nextSceneName}'");
        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        isLoading = true;

        // Fade to black before switching scenes
        if (crossFadeAnimator && fadeOutClip)
        {
            Debug.Log($"[LevelLoader] Playing fade-out: {fadeOutClip.name}");
            crossFadeAnimator.Play(fadeOutClip.name, 0, 0f);
            yield return new WaitForSecondsRealtime(fadeOutClip.length);
        }

        int buildIndex = GetBuildIndexByName(nextSceneName);
        if (buildIndex < 0)
        {
            Debug.LogError($"[LevelLoader] Cannot load '{nextSceneName}' — not found in Build Settings.");
            yield break;
        }

        Debug.Log($"[LevelLoader] Loading next scene: {nextSceneName} (index {buildIndex})");
        AsyncOperation op = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
        while (!op.isDone) yield return null;

        // In the next scene, your CrossFade Animator’s default state should be the fade-in clip
        Debug.Log($"[LevelLoader] Scene load completed: {nextSceneName}");
    }

    // Helper: get build index by scene file name (no .unity)
    int GetBuildIndexByName(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return i;
        }
        return -1;
    }
}
