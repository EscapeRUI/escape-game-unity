using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 1f;

    bool ButtonDown;
    bool IsAiming;
    Vector3 WalkDirection;

    Gvr.Internal.BaseVRDevice device;

    // Use this for initialization
    void Start () {
        device = Gvr.Internal.BaseVRDevice.GetDevice();
    }

    private void FixedUpdate()
    {
        Raycast();
    }

    // Update is called once per frame
    void Update()
    {
        DispatchEvents();
        
        if (ButtonDown)
        {
            // Not aiming an item so can walk
            if (!IsAiming)
            {
                WalkDirection = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                transform.position += WalkDirection * speed * Time.deltaTime;
            }
        }
    }

    private void DispatchEvents()
    {
        ButtonDown = Input.GetMouseButton(0);
#if UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_EDITOR)
        ButtonDown |= GvrController.ClickButton;
#endif  // UNITY_HAS_GOOGLEVR && (UNITY_ANDROID || UNITY_EDITOR)
    }

    private void Raycast()
    {
        GvrPointerPhysicsRaycaster GvrPPR = Camera.main.GetComponent<GvrPointerPhysicsRaycaster>();
        Ray ray = GvrPPR.GetLastRay();

        float dist = GvrPPR.eventCamera.farClipPlane - GvrPPR.eventCamera.nearClipPlane;
        float radius = GvrPPR.PointerRadius;
        RaycastHit[] hits;

        if (radius > 0.0f)
        {
            hits = Physics.SphereCastAll(ray, radius, dist, GvrPPR.finalEventMask);
        }
        else
        {
            hits = Physics.RaycastAll(ray, dist, GvrPPR.finalEventMask);
        }

        IsAiming = !(hits.Length == 0);
    }
}
