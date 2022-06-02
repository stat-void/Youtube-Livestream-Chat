using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ViewSystem
{
    private static List<string> _openScenes = new();
    private static Dictionary<string, IReusableView> _disabledScenes = new();

    /// <summary> Open a scene and add it to the list of open scenes. </summary>
    public static void Open(string scenePath, bool throwError = true, bool onMainThread = false)
    {
        // If scene is already open, return early.
        if (_openScenes.Contains(scenePath))
            return;

        // Restore a scene if it was opened before and disabled at runtime.
        if (_disabledScenes.ContainsKey(scenePath))
        {
            _disabledScenes[scenePath].Reactivate();
            _disabledScenes.Remove(scenePath);
            _openScenes.Add(scenePath);
            return;
        }

        int index = SceneUtility.GetBuildIndexByScenePath($"Scenes/{scenePath}");

        // Error case if scene path was not found
        if (index == -1)
        {
            if (throwError)
                throw new System.Exception($"Scene not found - {scenePath}");

            Debug.LogWarning($"Scene not found - {scenePath}");
            return;
        }

        // Load in scene
        if (onMainThread)
            SceneManager.LoadScene(index, LoadSceneMode.Additive);
        else
            SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        _openScenes.Add(scenePath);
    }

    /// <summary> Attempts to close the scene by hiding it. If that fails, the scene is closed normally. </summary>
    public static void Deactivate(string scenePath)
    {
        if (!_openScenes.Contains(scenePath))
            return;

        IReusableView presenter = GetPresenter(scenePath);

        // The scene does not have a presenter that supports deactivation
        if (presenter == null)
            Close(scenePath);

        _openScenes.Remove(scenePath);
        _disabledScenes[scenePath] = presenter;
        presenter.Deactivate();
    }

    /// <summary> Asynchronously unload scene if it exists. </summary>
    public static void Close(string scenePath)
    {
        if (!_openScenes.Contains(scenePath))
            return;

        _openScenes.Remove(scenePath);
        SceneManager.UnloadSceneAsync(scenePath);
    }

    /// <summary> Closes the scene by completly unloading it. </summary>
    /// <returns> Task that completes when unloading is finished. </returns>
    public static async Task CloseAsync(string view)
    {
        if (!_openScenes.Contains(view))
            return;

        _openScenes.Remove(view);
        var unloading = SceneManager.UnloadSceneAsync(view);
        while (!unloading.isDone)
        {
            await Task.Delay(100);
        }
    }

    private static IReusableView GetPresenter(string view)
    {
        // Scene can be hidden only if there is a presenter that supports it
        IReusableView presenter = null;

        var scene = SceneManager.GetSceneByName(view);
        var roots = scene.GetRootGameObjects();

        foreach (var root in roots)
        {
            presenter = root.GetComponent<IReusableView>();
            if (presenter != null)
                break;
        }

        return presenter;
    }

    private interface IReusableView
    {
        void Reactivate();
        void Deactivate();
    }

}
