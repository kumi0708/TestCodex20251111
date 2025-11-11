using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCameraController : MonoBehaviour
{
    public float smoothTime = 0.25f;
    public float minDistance = 15f;
    public float extraMargin = 3f;
    public float heightFactor = 0.6f; // how high above the bounds

    Camera cam;
    Vector3 velocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        var runners = FindObjectsOfType<Runner>();
        if (runners == null || runners.Length == 0) return;

        // Compute bounds of all runners
        Bounds b = new Bounds(runners[0].transform.position, Vector3.zero);
        foreach (var r in runners) b.Encapsulate(r.transform.position);
        b.Expand(extraMargin * 2f);

        // Determine last-place runner (smallest progress)
        var last = runners.OrderBy(r => r.Progress).First();
        Vector3 targetPos = last.transform.position;

        // Distance so that all fit in vertical FOV
        float radius = Mathf.Max(b.extents.x, b.extents.z);
        float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
        float dist = Mathf.Max(minDistance, radius / Mathf.Tan(fovRad * 0.5f) + extraMargin);

        // Position the camera behind last place along -Z, and above
        Vector3 desired = new Vector3(b.center.x, b.center.y + Mathf.Max(5f, radius * heightFactor), targetPos.z - dist);

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        transform.LookAt(targetPos);
    }
}
