using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ImmersedPhysics : MonoBehaviour
{
    public const float oceanWaterDensity = 1027f;
    public const float oceanWaterViscosity = 0.00188f;
    public enum VolumeType { Sphere, Cube }
    public enum ForceType { NoForce, Constant, Impulsion }

    public ForceType forceType = ForceType.NoForce;
    public VolumeType volType = VolumeType.Sphere;
    public Vector3 constForce = Vector3.up;
    public float radius = 1.0f;
    public Vector3 dimensions = Vector3.one;

    public float dragMultiplier = 2000f; // magic number
    public float angularDragMultiplier = 5000f; // magic number

    private Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void FixedUpdate()
    {
        float surfaceArea;
        if (volType == VolumeType.Cube)
            surfaceArea = 2 * dimensions.x + 2 * dimensions.y + 2 * dimensions.z;
        else surfaceArea = 4 * Mathf.PI * Mathf.Pow(radius, 2);

        // just to make drag dynamic, modication from stookes law. This doesn't follow physics laws!
        rb.drag = 0.5f / surfaceArea * oceanWaterViscosity * rb.velocity.magnitude * dragMultiplier;
        rb.angularDrag = 0.5f / surfaceArea * oceanWaterViscosity * rb.angularVelocity.magnitude * angularDragMultiplier;

        if (forceType == ForceType.Constant) rb.AddForce(constForce);
        else if (forceType == ForceType.Impulsion)
        {
            Vector3 impulsion = Vector3.up * -Physics.gravity.y * oceanWaterDensity * radius;
            impulsion.y += Physics.gravity.y * rb.mass;
            rb.AddForce(impulsion);
        }
    }
}
