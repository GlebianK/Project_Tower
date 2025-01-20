using UnityEngine;

public class Died : MonoBehaviour
{
    public void OnDied()
    {
       Destroy(gameObject);
    }
}
