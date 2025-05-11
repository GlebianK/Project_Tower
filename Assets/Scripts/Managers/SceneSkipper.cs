using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSkipper : MonoBehaviour
{
    [SerializeField] private int nextSceneBuildIndex = 2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SceneManager.LoadScene(nextSceneBuildIndex);
    }
}
