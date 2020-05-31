using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public string SceneToLoad;

    public void LoadScene() {
        SceneManager.LoadScene(SceneToLoad);
    }
}
