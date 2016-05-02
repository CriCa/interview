using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Transform[] cameraSockets;
    public KeyCode[] cameraTriggers;

    [HideInInspector]
    public float angle;
    [HideInInspector]
    public int selected;

    private OrbitController orbit;

    void Start()
    {
        angle = 0; selected = 0;
        orbit = GetComponent<OrbitController>();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(cameraTriggers[3]))
            {
                transform.parent = cameraSockets[3];
                selected = 3;
                orbit.enabled = true;
                orbit.CalcOrbit();
            }
            else
            {
                for (int i = 0; i < cameraTriggers.Length - 1; i++)
                    if (Input.GetKeyDown(cameraTriggers[i]))
                    {
                        transform.parent = cameraSockets[i];
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = Quaternion.identity;
                        selected = i;
                        orbit.enabled = false;
                    }
            }
        }
    }

    void LateUpdate()
    {
        Vector3 localFwd = transform.forward.normalized; localFwd.y = 0;
        angle = Vector3.Angle(localFwd, Vector3.forward);
        if (transform.forward.x > 0) angle *= -1;
    }
}
