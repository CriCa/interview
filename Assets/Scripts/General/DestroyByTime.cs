using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

    public float time = 1f;
	
	void Start () { Invoke("Destroy", time); }
	
	void Destroy() { Destroy(gameObject); }
}
