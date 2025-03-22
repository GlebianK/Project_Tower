using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void ExitGameButton()
    {
        Debug.Log("Exit the game");
        Application.Quit();
    }
}
