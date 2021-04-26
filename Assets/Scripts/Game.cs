using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint
{
    public string Scene;
    public Vector3 Position;

    public SavePoint() { }
    public SavePoint(string scene, Vector3 position)
    {
        Scene = scene;
        Position = position;
    }
}

public class Game : SingleInstance<Game>
{

    public static bool IsTransitioning = false;
    private SceneTransition sceneTransitionInstance;
    public static int? spawnPoint = null;
    public static Vector3? spawnPosition = null;

    public static SavePoint SavePoint;
    public static int? PlayerHealth;
    public static int? PotionCount;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Load a scene with the provided name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public static async Task LoadAsync(string sceneName, SceneTransition sceneTransitionPrefab, int? spawnPoint = null, Vector3? spawnPosition = null)
    {
        if (IsTransitioning) return;

        if (spawnPoint != null) Game.spawnPoint = spawnPoint.Value;
        if (spawnPosition != null) Game.spawnPosition = spawnPosition.Value;

        IsTransitioning = true;

        // Transition out
        Instance.sceneTransitionInstance = Instantiate(sceneTransitionPrefab);
        Instance.sceneTransitionInstance.Out();

        while (!Instance.sceneTransitionInstance.DidReachHalfway)
        {
            await new WaitForUpdate();
        }

        await new WaitForSeconds(0.25f);

        SceneManager.LoadScene(sceneName);

        // Transition in
        Instance.sceneTransitionInstance.In();

        IsTransitioning = false;
    }

    public static bool ProceedText()
    {
        return Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Z)
            || Input.GetKeyDown(KeyCode.X)
            || Input.GetKeyDown(KeyCode.C);

    }
}