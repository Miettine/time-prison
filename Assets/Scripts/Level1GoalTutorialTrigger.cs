using UnityEngine;

public class Level1GoalTutorialTrigger : MonoBehaviour
{
    Tutorial tutorial;

    private void Awake()
    {
        tutorial = Tutorial.GetInstance();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            tutorial.OnPlayerEnteredLevel1GoalTutorialTrigger();
            Debug.Log("Player entered Level 1 Goal Tutorial Trigger");
            // Disable this trigger so it only happens once
            gameObject.SetActive(false);
        }
    }
}
