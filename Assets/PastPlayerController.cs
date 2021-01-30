using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastPlayerController : MonoBehaviour
{
    float fieldOfViewDegrees;
    float fieldOfViewRange;
    Transform playerTransform;
    TimeTracker timeTracker;

    private void Awake() {
        timeTracker = FindObjectOfType<TimeTracker>();
    }

	// Start is called before the first frame update
	void Start()
    {
        var light = GetComponentInChildren<Light>();
        fieldOfViewDegrees = light.spotAngle;
        fieldOfViewRange = light.range;

        playerTransform = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        var toPlayer = playerTransform.position - this.transform.position;

        var withinRange = toPlayer.magnitude < fieldOfViewRange;

        Vector3 lookDirection = transform.eulerAngles;

        var lookDirectionToPlayerAngle = Vector3.Angle(lookDirection, toPlayer);
        var withinFieldOfViewAngle = lookDirectionToPlayerAngle < fieldOfViewDegrees;

        Debug.Log(lookDirectionToPlayerAngle);

        if (withinFieldOfViewAngle && withinRange) {
            //timeTracker.TimeParadox();
        }
    }
}
