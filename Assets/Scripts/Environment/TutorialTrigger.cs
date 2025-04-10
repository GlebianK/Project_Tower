using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Tooltip("ѕо этому ID ищетс€ соответствующий туториал в UI")]
    [SerializeField] private int id;

    private bool isPassed;

    private void Awake()
    {
        isPassed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.name} entered Tutor Zone with id {id}!");
        if (other.CompareTag("Player") && !isPassed)
        {
            isPassed = true;
            TutorialCanvas.Instance.ShowTutorial(id);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"{other.name} exited Tutor Zone with id {id}!");
        if (other.CompareTag("Player") && isPassed)
        {
            TutorialCanvas.Instance.HideTutorial();
        }
    }
}
