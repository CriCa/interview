using UnityEngine;

public class BackAndForward : MonoBehaviour {

    public Vector3 axis = Vector3.right;
    public float speed = 1.0f;
    public float displacement = 2.0f;

    [HideInInspector]
    public Vector3 originalPosition;

	void Start () { originalPosition = transform.position; }
	
	void FixedUpdate () {
        float disp = (Mathf.Sin(Time.time * speed) * displacement);
        transform.position = new Vector3(originalPosition.x + disp * axis.normalized.x, originalPosition.y + disp * axis.normalized.y, originalPosition.z + disp * axis.normalized.z);
    }
}