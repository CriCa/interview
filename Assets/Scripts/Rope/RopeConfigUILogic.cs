using UnityEngine;
using UnityEngine.UI;

public class RopeConfigUILogic : MonoBehaviour
{
    #region rope props
    public Rope rope;
    public Transform ropeTip;
    public Material dMat, cMat;
    public Slider lenS, divsS, diamS, facesS, dFrictionS, sFrictionS,
        bounceS, massS, itersS, lspringXS, ldampXS, lspringYZS, ldampYZS,
        limitXS, bounceXS, contXS, limitYS, bounceYS, contYS, limitZS,
        bounceZS, contZS, springXS, dampXS, maxFXS, springYZS, dampYZS, maxFYZS;
    public Text lenVal, divsVal, dFrictionVal, diamVal, facesVal, sFrictionVal,
        bounceVal, massVal, itersVal, lspringXVal, ldampXVal, lspringYZVal,
        ldampYZVal, limitXVal, bounceXVal, contXVal, limitYVal, bounceYVal,
        contYVal, limitZVal, bounceZVal, contZVal, springXVal,
        dampXVal, maxFXVal, springYZVal, dampYZVal, maxFYZVal;
    public Dropdown orientation, mat, fComb, bComb, inter, colDetect;
    public Toggle looseEnd, grav;
    #endregion

    #region collider props
    public Toggle colToggle;
    public GameObject colliderPanel, capCollider, cubeCollider, sphereCollider;
    public Dropdown colliderType;
    public Slider velocity, angVelocity;
    public Vector3 capScale, cubeScale, sphereScale;
    #endregion

    private GameObject activeCollider;

    void Start() { activeCollider = capCollider; RefreshUI(true); SetupCollider(); }

    #region rope UI logic
    public void ChangeRope()
    {
        rope.ropeLength = lenS.value;
        rope.nNodes = (int)divsS.value;
        rope.diameter = diamS.value;
        rope.nFaces = (int)facesS.value;

        if (orientation.value == 0)
        {
            ropeTip.position = new Vector3(-rope.ropeLength / 1.1f, 0.0f, 0.0f);
            ropeTip.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            rope.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
        else
        {
            ropeTip.position = new Vector3(0.0f, -rope.ropeLength, 0.0f);
            ropeTip.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
            rope.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        }

        ropeTip.GetComponent<Rigidbody>().isKinematic = looseEnd.isOn;

        rope.Build();

        SetupCollider();

        RefreshUI();
    }

    public void ChangeMaterial()
    {
        if (mat.value == 0) rope.meshMaterial = dMat;
        else rope.meshMaterial = cMat;

        rope.SetMaterial();
    }

    public void ChangePhysics()
    {
        rope.dFriction = dFrictionS.value;
        rope.sFriction = sFrictionS.value;
        rope.bounciness = bounceS.value;
        rope.frictionMode = (PhysicMaterialCombine)fComb.value;
        rope.bounceMode = (PhysicMaterialCombine)bComb.value;
        rope.nodeMass = massS.value;
        rope.useGravity = grav.isOn;
        rope.interpolation = (RigidbodyInterpolation)inter.value;
        rope.colDetection = (CollisionDetectionMode)colDetect.value;
        rope.solverIterations = (int)itersS.value;

        rope.BuildPhysics();

        RefreshUI();
    }

    public void ChangeJoints()
    {
        rope.angularXSpring = lspringXS.value;
        rope.angularXDamper = ldampXS.value;
        rope.angularYZSpring = lspringYZS.value;
        rope.angularYZDamper = ldampYZS.value;
        rope.angularXLimit = limitXS.value;
        rope.angularXBounce = bounceXS.value;
        rope.angularXContact = contXS.value;
        rope.angularYLimit = limitYS.value;
        rope.angularYBounce = bounceYS.value;
        rope.angularYContact = contYS.value;
        rope.angularZLimit = limitZS.value;
        rope.angularZBounce = bounceZS.value;
        rope.angularZContact = contZS.value;
        rope.driveXSpring = springXS.value;
        rope.driveXDamper = dampXS.value;
        rope.driveXMaxF = maxFXS.value;
        rope.driveYZSpring = springYZS.value;
        rope.driveYZDamper = dampYZS.value;
        rope.driveYZMaxF = maxFYZS.value;

        rope.SetJoints();

        RefreshUI();
    }

    private void RefreshUI(bool sliders = false)
    {
        float len = rope.ropeLength;
        int divs = rope.nNodes;
        float diam = rope.diameter;
        int faces = rope.nFaces;
        float dFriction = rope.dFriction;
        float sFriction = rope.sFriction;
        float bounce = rope.bounciness;
        float mass = rope.nodeMass;
        int iters = rope.solverIterations;
        float lspringX = rope.angularXSpring;
        float ldampX = rope.angularXDamper;
        float lspringYZ = rope.angularYZSpring;
        float ldampYZ = rope.angularYZDamper;
        float limitX = rope.angularXLimit;
        float bounceX = rope.angularXBounce;
        float contX = rope.angularXContact;
        float limitY = rope.angularYLimit;
        float bounceY = rope.angularYBounce;
        float contY = rope.angularYContact;
        float limitZ = rope.angularZLimit;
        float bounceZ = rope.angularZBounce;
        float contZ = rope.angularZContact;
        float springX = rope.driveXSpring;
        float dampX = rope.driveXDamper;
        float maxFX = rope.driveXMaxF;
        float springYZ = rope.driveYZSpring;
        float dampYZ = rope.driveYZDamper;
        float maxFYZ = rope.driveYZMaxF;

        if (sliders)
        {
            lenS.value = len;
            divsS.value = divs;
            diamS.value = diam;
            facesS.value = faces;
            dFrictionS.value = dFriction;
            sFrictionS.value = sFriction;
            bounceS.value = bounce;
            massS.value = mass;
            itersS.value = iters;
            lspringXS.value = lspringX;
            ldampXS.value = ldampX;
            lspringYZS.value = lspringYZ;
            ldampYZS.value = ldampYZ;
            limitXS.value = limitX;
            bounceXS.value = bounceX;
            contXS.value = contX;
            limitYS.value = limitY;
            bounceYS.value = bounceY;
            contYS.value = contY;
            limitZS.value = limitZ;
            bounceZS.value = bounceZ;
            contZS.value = contZ;
            springXS.value = springX;
            dampXS.value = dampX;
            maxFXS.value = maxFX;
            springYZS.value = springYZ;
            dampYZS.value = dampYZ;
            maxFYZS.value = maxFYZ;
        }

        lenVal.text = len.ToString("n1");
        divsVal.text = divs.ToString();
        diamVal.text = diam.ToString("n1");
        facesVal.text = faces.ToString();
        dFrictionVal.text = dFriction.ToString("n1");
        sFrictionVal.text = sFriction.ToString("n1");
        bounceVal.text = bounce.ToString("n1");
        massVal.text = mass.ToString("n1");
        itersVal.text = iters.ToString();
        lspringXVal.text = lspringX.ToString("n1");
        ldampXVal.text = ldampX.ToString("n1");
        lspringYZVal.text = lspringYZ.ToString("n1");
        ldampYZVal.text = ldampYZ.ToString("n1");
        limitXVal.text = limitX.ToString();
        bounceXVal.text = bounceX.ToString("n1");
        contXVal.text = contX.ToString("n1");
        limitYVal.text = limitY.ToString();
        bounceYVal.text = bounceY.ToString("n1");
        contYVal.text = contY.ToString("n1");
        limitZVal.text = limitZ.ToString();
        bounceZVal.text = bounceZ.ToString("n1");
        contZVal.text = contZ.ToString("n1");
        springXVal.text = springX.ToString("n1");
        dampXVal.text = dampX.ToString("n1");
        maxFXVal.text = maxFX.ToString("n1");
        springYZVal.text = springYZ.ToString("n1");
        dampYZVal.text = dampYZ.ToString("n1");
        maxFYZVal.text = maxFYZ.ToString("n1");

        grav.isOn = rope.useGravity;

        if (rope.meshMaterial == dMat) mat.value = 0;
        else mat.value = 1;

        fComb.value = (int)rope.frictionMode;
        bComb.value = (int)rope.bounceMode;
        inter.value = (int)rope.interpolation;
        colDetect.value = (int)rope.colDetection;
    }
    #endregion

    #region collider UI logic
    public void SetupCollider()
    {
        colliderPanel.SetActive(colToggle.isOn);

        if(colToggle.isOn)
        {
            activeCollider.SetActive(false);
            switch (colliderType.value)
            {
                case 0:
                    capCollider.SetActive(true);
                    activeCollider = capCollider;
                    activeCollider.transform.localScale = capScale * (rope.ropeLength / 2.0f);
                    break;
                case 1:
                    cubeCollider.SetActive(true);
                    activeCollider = cubeCollider;
                    activeCollider.transform.localScale = cubeScale * (rope.ropeLength / 2.0f);
                    break;
                case 2:
                    sphereCollider.SetActive(true);
                    activeCollider = sphereCollider;
                    activeCollider.transform.localScale = sphereScale * (rope.ropeLength / 2.0f);
                    break;
            }

            activeCollider.GetComponent<RotateAroundItself>().speed = angVelocity.value * 100f;

            BackAndForward bfScript = activeCollider.GetComponent<BackAndForward>();
            bfScript.speed = velocity.value;
            bfScript.displacement = (orientation.value == 0 ? rope.ropeLength / 2.0f : rope.ropeLength * 0.1f);

            Vector3 newPos = (orientation.value == 0 ? newPos = new Vector3(-rope.ropeLength / 2.0f, -0.15f * rope.ropeLength, 0.0f) :
                newPos = new Vector3(0.0f, -rope.ropeLength * 2.0f / 3.0f, 0.0f));
            bfScript.originalPosition = newPos;
            activeCollider.transform.position = newPos;
            
        }
        else activeCollider.SetActive(false);
    }
    #endregion
}
