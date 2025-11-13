using UnityEngine;

public class TimePortal : MonoBehaviour
{
    private TimeTravel timeTravel;
    private Collider portalCollider;
    private Tutorial tutorial;

    private void Awake()
    {
        timeTravel = TimeTravel.GetInstance();
        portalCollider = GetComponent<Collider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tutorial = Tutorial.GetOrCreateInstance();
        Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeTravel.TimeTravelling == false)
        {
            Enable();
        }   
    }

    private void OnTriggerEnter(Collider other)
    {
        // Use GameObject tags to detect the player. Check the collider's GameObject and its root in case the collider
        // belongs to a child object of the player.
        var go = other.gameObject;
        var root = other.transform.root.gameObject;

        if (go.CompareTag("Player") || root.CompareTag("Player"))
        {
            tutorial.OnTimePortalEntered();
            Disable(); // Disable the collider to prevent multiple triggers
            timeTravel.StartTimeTravelToBeginning();
        }
    }

    // Enables the portal collider so it can be triggered.
    private void Enable()
    {
        portalCollider.enabled = true;
    }

    // Disables the portal collider to prevent re-triggering.
    private void Disable()
    {
        portalCollider.enabled = false;
    }
}
