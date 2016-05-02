using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ROVModule : MonoBehaviour
{
    public GameObject sandParticles;
    public GameObject sparkParticles;

    private float sTimer = 0f;
    private float hTimer = 0f;

    private float sDuration;
    private float hDuration;

    void Start()
    {
        sDuration = sandParticles.GetComponent<ParticleSystem>().duration;
        hDuration = sparkParticles.GetComponent<ParticleSystem>().duration;
    }

    void Update() { sTimer += Time.deltaTime; hTimer += Time.deltaTime; }

    void OnCollisionStay(Collision collision) { HandleCollision(collision); }

    void OnCollisionEnter(Collision collision) { HandleCollision(collision); }

    void HandleCollision (Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            if (sTimer >= sDuration)
            {
                foreach (ContactPoint point in collision.contacts)
                    Instantiate(sandParticles, point.point, Quaternion.Euler(new Vector3(-90f, 0, 0f)));
                sTimer = 0f;
            }
        }
        else
        {
            if (hTimer >= hDuration)
            {
                foreach (ContactPoint point in collision.contacts)
                    Instantiate(sparkParticles, point.point, Quaternion.Euler(new Vector3(-90f, 0, 0f)));
                hTimer = 0f;
            }
        }
    }
}