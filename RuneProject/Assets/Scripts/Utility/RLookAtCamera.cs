using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RLookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform vcam = null;
    [SerializeField] private float offset = 0.0f;

    private Vector3 origin;

    private void Start()
    {
        origin = transform.localPosition;
    }

    void Update()
    {
        if (vcam == null) {
            vcam = Camera.main.GetComponentInParent<RuneProject.CameraSystem.RPlayerCameraComponent>().transform.Find("VCamParent/VirtualCam").transform;
        }
        Vector3 target = vcam.transform.forward;
        transform.forward = target;
        transform.localPosition = origin;
        transform.position += offset * vcam.transform.up.normalized;
    }
}
