using System.Collections;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Tooltip("По этому ID ищется соответствующий туториал в UI")]
    [SerializeField] private int id;

    [Tooltip("Время удержания туториала на экране (в секундах)")]
    [SerializeField] private float hideTimer = 2.5f;

    private bool isPassed;

    private void Start()
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
            StartCoroutine(HideTutorialPanelTimerCoroutine(id));
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"{other.name} exited Tutor Zone with id {id}!");
        if (other.CompareTag("Player") && isPassed)
        {
            TutorialCanvas.Instance.HideTutorial();
        }
    }
    */

    private IEnumerator HideTutorialPanelTimerCoroutine(int triggerID)
    {
        yield return new WaitForSeconds(hideTimer);
        if (isPassed)
        {
            TutorialCanvas.Instance.HideTutorial(triggerID);
        }
    }
}
