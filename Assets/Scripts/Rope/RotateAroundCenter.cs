using UnityEngine;
using System.Collections;

public class RotateAroundCenter : MonoBehaviour {

    public float speed = 1f;
    public float radius = 2.5f;

    private Vector3 originalPos;

    void Start () { originalPos = transform.position; }
    
	void FixedUpdate () {
        transform.localPosition = new Vector3(originalPos.x + (Mathf.Cos(Time.time * speed) * radius), originalPos.y, originalPos.z + (Mathf.Sin(Time.time * speed) * radius));
        transform.LookAt(originalPos);
    }
}
