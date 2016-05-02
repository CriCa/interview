using UnityEngine;
using System.Collections;

public class RotateAroundItself : MonoBehaviour {

    public Vector3 axis = Vector3.up;
    public float speed = 100f;

	void FixedUpdate () { transform.Rotate(axis.normalized * speed * Time.deltaTime); }
}
