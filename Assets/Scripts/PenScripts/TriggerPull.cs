using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPull : MonoBehaviour
{
    public GameObject Trigger;
    public float AnimationDuration = 0.5f;
    public float totalRotation = 20;
    private float currentRotation = 0;
    private bool isPullingTrigger = false;
    private float degreePerSecond;
    private Transform RotationPoint;

    void Awake()
    {
        degreePerSecond = totalRotation / AnimationDuration;
        if (Trigger == null)
        {
            Debug.LogWarning("Trigger object is empty, defaulting to child named 'Trigger'.");
            Trigger = transform.Find("Trigger").gameObject;
        }
        RotationPoint = Trigger.transform.GetChild(0);
    }

    void Update()
    {
        if (isPullingTrigger && currentRotation < totalRotation)
        {
            currentRotation += degreePerSecond * Time.deltaTime;
            PlayTriggerPullAnimation(degreePerSecond * Time.deltaTime, true);
        }
        else if (!isPullingTrigger && currentRotation > 0)
        {
            currentRotation -= degreePerSecond * Time.deltaTime;
            PlayTriggerPullAnimation(degreePerSecond * Time.deltaTime, false);
        }
    }

    public void PullTrigger()
    {
        isPullingTrigger = true;
    }
    
    public void ReleaseTrigger()
    {
        isPullingTrigger = false;
    }

    private void PlayTriggerPullAnimation(float rotationAmount, bool forward)
    {
        Vector3 point = RotationPoint.position;
        Vector3 axis = RotationPoint.right;
        transform.RotateAround(point, axis, rotationAmount * (forward ? 1 : -1));
    }
}
