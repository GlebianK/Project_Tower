using UnityEngine;

public class RestartOutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            GameManager.Instance.RestartLevel();
    }
}
