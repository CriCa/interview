using UnityEngine;

[ExecuteInEditMode]
public class Rope : MonoBehaviour
{
    #region public parameters
    public const float MIN_LENGTH = 0.0001f;
    public const int MIN_NODES = 2;
    public const float MIN_DIAM = 0.005f;
    public const int MIN_FACES = 2;

    public GameObject ropeEnd;
    public float ropeLength = 2.0f;
    public int nNodes = 20;
    public float diameter = 0.1f;

    public int nFaces = 12;
    public Material meshMaterial;

    public float dFriction = 0.6f;
    public float sFriction = 0.6f;
    public float bounciness = 0f;
    public PhysicMaterialCombine frictionMode = PhysicMaterialCombine.Average;
    public PhysicMaterialCombine bounceMode = PhysicMaterialCombine.Average;
    public float nodeMass = 1.0f;
    public bool useGravity = true;
    public RigidbodyInterpolation interpolation = RigidbodyInterpolation.None;
    public CollisionDetectionMode colDetection = CollisionDetectionMode.Discrete;
    public int solverIterations = 100;

    public float angularXSpring = 0.0f;
    public float angularXDamper = 0.0f;
    public float angularYZSpring = 0.0f;
    public float angularYZDamper = 0.0f;
    public float angularXLimit = 0.0f;
    public float angularXBounce = 0.0f;
    public float angularXContact = 0.0f;
    public float angularYLimit = 0.0f;
    public float angularYBounce = 0.0f;
    public float angularYContact = 0.0f;
    public float angularZLimit = 0.0f;
    public float angularZBounce = 0.0f;
    public float angularZContact = 0.0f;
    public float driveXSpring = 5.0f;
    public float driveXDamper = 0.0f;
    public float driveXMaxF = 20.0f;
    public float driveYZSpring = 5.0f;
    public float driveYZDamper = 0.0f;
    public float driveYZMaxF = 20.0f;
    #endregion

    #region aux data
    [HideInInspector]
    public GameObject[] nodes = null;
    [HideInInspector]
    public ConfigurableJoint[] joints = new ConfigurableJoint[0];
    [HideInInspector]
    public float nodeLength;

    [HideInInspector]
    public SkinnedMeshRenderer skinnedRend = null;

    [HideInInspector]
    public bool backupTips = false;
    [HideInInspector]
    public Vector3 bacLocalPos;
    [HideInInspector]
    public Quaternion bacLocalRot;
    [HideInInspector]
    public Vector3 bacEndLocalPos;
    [HideInInspector]
    public Quaternion bacEndLocalRot;

    [HideInInspector]
    public bool clean = true;
    [HideInInspector]
    public Vector3 forwardDirection;
    [HideInInspector]
    public Vector3 upDirection;
    #endregion

    void Awake()
    {
        if (skinnedRend == null) skinnedRend = gameObject.GetComponent<SkinnedMeshRenderer>() != null ? gameObject.GetComponent<SkinnedMeshRenderer>() : gameObject.AddComponent<SkinnedMeshRenderer>();

        if (Application.isPlaying == true) { CreateJoints(); }
    }

    #region main logic
    public void Build()
    {
        // clear
        Clear();

        if (ropeEnd == null) { Debug.LogError("There is no end gameobject"); return; }

        // check limits
        if (nNodes < MIN_NODES) nNodes = MIN_NODES;
        if (ropeLength < MIN_LENGTH) ropeLength = MIN_LENGTH;
        nodeLength = ropeLength / nNodes;

        // Create links and joints
        if (backupTips == false)
        {
            bacLocalPos = transform.localPosition;
            bacLocalRot = transform.localRotation;

            bacEndLocalPos = ropeEnd.transform.localPosition;
            bacEndLocalRot = ropeEnd.transform.localRotation;
            backupTips = true;
        }

        forwardDirection = transform.InverseTransformDirection((ropeEnd.transform.position - transform.position).normalized);

        nodes = new GameObject[nNodes];
        joints = new ConfigurableJoint[nNodes + 1];

        float fRemainingLength = ((ropeEnd.transform.position - transform.position).magnitude - nodeLength) / (ropeEnd.transform.position - transform.position).magnitude;

        for (int i = 0; i < nNodes; i++)
        {
            float fLinkT = (float)i / (nNodes == 1 ? 1.0f : (nNodes - 1.0f));

            nodes[i] = new GameObject("Node " + i);

            if (Vector3.Distance(transform.position, ropeEnd.transform.position) < 0.001f)
            {
                nodes[i].transform.position = transform.position;
                nodes[i].transform.rotation = transform.rotation;
            }
            else
            {
                nodes[i].transform.position = Vector3.Lerp(transform.position, ropeEnd.transform.position, fLinkT * fRemainingLength);
                nodes[i].transform.rotation = Quaternion.LookRotation((ropeEnd.transform.position - transform.position).normalized);
            }

            if (nodes[i].GetComponent<Rigidbody>() == null)
                nodes[i].AddComponent<Rigidbody>();

            nodes[i].transform.parent = this.transform;
        }

        BuildMesh();

        clean = false;

        if (Application.isPlaying)
            CreateJoints();

        BuildPhysics();
    }

    public void Clear()
    {
        // delete gameobject's children
        while (transform.childCount != 0) DestroyImmediate(transform.GetChild(0).gameObject);

        // delete nodes
        nodes = null;
        backupTips = false;

        // delete skinned mesh
        if (skinnedRend != null) DestroyImmediate(skinnedRend.sharedMesh);

        clean = true;
    }
    #endregion

    #region joints logic
    public void SetJoints()
    {
        // check limits
        if (angularXSpring < 0.0f) angularXSpring = 0.0f;
        if (angularXDamper < 0.0f) angularXDamper = 0.0f;
        if (angularYZSpring < 0.0f) angularYZSpring = 0.0f;
        if (angularYZDamper < 0.0f) angularYZDamper = 0.0f;
        if (angularXContact < 0.0f) angularXContact = 0.0f;
        if (angularYContact < 0.0f) angularYContact = 0.0f;
        if (angularZContact < 0.0f) angularZContact = 0.0f;
        if (driveXSpring < 0.0f) driveXSpring = 0.0f;
        if (driveXDamper < 0.0f) driveXDamper = 0.0f;
        if (driveXMaxF < 0.0f) driveXMaxF = 0.0f;
        if (driveYZSpring < 0.0f) driveYZSpring = 0.0f;
        if (driveYZDamper < 0.0f) driveYZDamper = 0.0f;
        if (driveYZMaxF < 0.0f) driveYZMaxF = 0.0f;

        if (clean == true || nodes == null || !Application.isPlaying) return;

        // backup transforms for later
        Vector3[] tempNodesPos = new Vector3[nNodes];
        Quaternion[] tempNodesRot = new Quaternion[nNodes];
        Vector3 tempPos = transform.localPosition, endTempPos = Vector3.zero;
        Quaternion tempRot = transform.localRotation, endTempRot = Quaternion.identity;

        if (backupTips)
        {
            transform.localPosition = bacLocalPos;
            transform.localRotation = bacLocalRot;

            if (ropeEnd != null)
            {
                endTempPos = ropeEnd.transform.localPosition;
                endTempRot = ropeEnd.transform.localRotation;
                ropeEnd.transform.localPosition = bacEndLocalPos;
                ropeEnd.transform.localRotation = bacEndLocalRot;
            }
        }

        // put them in line so that new motion limits will be referenced from here, not relative to the current position
        float maxZ = nodeLength * (nNodes - 1);

        for (int i = 0; i < nNodes; i++)
        {
            tempNodesPos[i] = nodes[i].transform.position;
            tempNodesRot[i] = nodes[i].transform.rotation;

            nodes[i].transform.position = Vector3.Lerp(Vector3.zero, new Vector3(0.0f, 0.0f, maxZ), (float)i / (nNodes - 1.0f));
            nodes[i].transform.rotation = Quaternion.identity;
        }

        // set properties
        foreach (ConfigurableJoint joint in joints) UpdateJointValues(joint);

        if (backupTips)
        {
            Vector3 fw = transform.TransformDirection(forwardDirection);
            Vector3 up = transform.TransformDirection(upDirection);

            nodes[0].transform.position = transform.position;
            nodes[0].transform.rotation = Quaternion.LookRotation(fw, up);

            nodes[nNodes - 1].transform.position = ropeEnd.transform.position - (fw * nodeLength);
            nodes[nNodes - 1].transform.rotation = Quaternion.LookRotation(fw, up);

            if (joints[0] != null) UpdateJointValues(joints[0]);

            if (joints[joints.Length - 1] != null) UpdateJointValues(joints[joints.Length - 1]);

            // restore transforms
            transform.localPosition = tempPos;
            transform.localRotation = tempRot;

            if (ropeEnd != null)
            {
                ropeEnd.transform.localPosition = endTempPos;
                ropeEnd.transform.localRotation = endTempRot;
            }
        }

        for (int i = 0; i < nNodes; i++)
        {
            nodes[i].transform.position = tempNodesPos[i];
            nodes[i].transform.rotation = tempNodesRot[i];
        }

    }

    void CreateJoints()
    {
        if (clean == true || nodes == null) return;

        // create rigidbodies if necessary
        if (GetComponent<Rigidbody>() == null) gameObject.AddComponent<Rigidbody>().isKinematic = true;
        if (ropeEnd != null) if (ropeEnd.GetComponent<Rigidbody>() == null) ropeEnd.AddComponent<Rigidbody>();

        // backup nodes' transforms
        Vector3[] tempNodesPos = new Vector3[nNodes];
        Quaternion[] tempNodesRot = new Quaternion[nNodes];
        Vector3 tempPos = transform.localPosition, tempEndPos = Vector3.zero;
        Quaternion tempRot = transform.localRotation, tempEndRot = Quaternion.identity;

        if (backupTips)
        {
            transform.localPosition = bacLocalPos;
            transform.localRotation = bacLocalRot;

            if (ropeEnd != null)
            {
                tempEndPos = ropeEnd.transform.localPosition;
                tempEndRot = ropeEnd.transform.localRotation;
                ropeEnd.transform.localPosition = bacEndLocalPos;
                ropeEnd.transform.localRotation = bacEndLocalRot;
            }
        }

        float maxZ = nodeLength * (nNodes - 1);

        for (int i = 0; i < nNodes; i++)
        {
            if (i == 0) upDirection = transform.InverseTransformDirection(nodes[i].transform.up);

            tempNodesPos[i] = nodes[i].transform.position;
            tempNodesRot[i] = nodes[i].transform.rotation;

            nodes[i].transform.position = Vector3.Lerp(Vector3.zero, new Vector3(0.0f, 0.0f, maxZ), (float)i / (nNodes - 1.0f));
            nodes[i].transform.rotation = Quaternion.identity;

            if (i > 0)
            {
                joints[i] = nodes[i].AddComponent<ConfigurableJoint>();

                UpdateJointValues(joints[i]);
                joints[i].connectedBody = nodes[i - 1].GetComponent<Rigidbody>();
                joints[i].anchor = nodes[i].transform.InverseTransformPoint(nodes[i].transform.position);
            }
        }

        // reposition
        float reposLength = ((ropeEnd.transform.position - transform.position).magnitude - nodeLength) / (ropeEnd.transform.position - transform.position).magnitude;
        if (reposLength < 0.0f) reposLength = 0.0f;

        for (int i = 0; i < nNodes; i++)
        {
            nodes[i].transform.position = Vector3.Lerp(transform.position, ropeEnd.transform.position, ((float)i / (nNodes - 1.0f)) * reposLength);
            nodes[i].transform.rotation = Quaternion.LookRotation((ropeEnd.transform.position - transform.position).normalized);
        }

        joints[0] = nodes[0].AddComponent<ConfigurableJoint>();
        UpdateJointValues(joints[0]);
        joints[0].connectedBody = gameObject.GetComponent<Rigidbody>();
        joints[0].anchor = nodes[0].transform.InverseTransformPoint(transform.position);

        joints[nNodes] = nodes[nNodes - 1].AddComponent<ConfigurableJoint>();
        UpdateJointValues(joints[nNodes]);
        joints[nNodes].connectedBody = ropeEnd.GetComponent<Rigidbody>();
        joints[nNodes].anchor = nodes[nNodes - 1].transform.InverseTransformPoint(ropeEnd.transform.position);

        // restore transforms
        if (backupTips)
        {
            transform.localPosition = tempPos;
            transform.localRotation = tempRot;

            if (ropeEnd != null)
            {
                ropeEnd.transform.localPosition = tempEndPos;
                ropeEnd.transform.localRotation = tempEndRot;
            }
        }

        for (int i = 0; i < nNodes; i++)
        {
            nodes[i].transform.position = tempNodesPos[i];
            nodes[i].transform.rotation = tempNodesRot[i];
        }
    }

    void UpdateJointValues(ConfigurableJoint joint)
    {
        joint.axis = Vector3.right;
        joint.secondaryAxis = Vector3.up;

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = Mathf.Approximately(angularXLimit, 0.0f) == false ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
        joint.angularYMotion = Mathf.Approximately(angularYLimit, 0.0f) == false ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
        joint.angularZMotion = Mathf.Approximately(angularZLimit, 0.0f) == false ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;


        SoftJointLimitSpring softSpring = new SoftJointLimitSpring();
        softSpring.spring = angularXSpring;
        softSpring.damper = angularXDamper;
        joint.angularXLimitSpring = softSpring;

        softSpring.spring = angularYZSpring;
        softSpring.damper = angularYZDamper;
        joint.angularYZLimitSpring = softSpring;


        SoftJointLimit softLimit = new SoftJointLimit();
        softLimit.limit = -angularXLimit;
        softLimit.bounciness = angularXBounce;
        softLimit.contactDistance = angularXContact;
        joint.lowAngularXLimit = softLimit;

        softLimit.limit = angularXLimit;
        joint.highAngularXLimit = softLimit;

        softLimit.limit = angularYLimit;
        softLimit.bounciness = angularYBounce;
        softLimit.contactDistance = angularYContact;
        joint.angularYLimit = softLimit;

        softLimit.limit = angularZLimit;
        softLimit.bounciness = angularZBounce;
        softLimit.contactDistance = angularZContact;
        joint.angularZLimit = softLimit;

        JointDrive drive = new JointDrive();
        drive.positionSpring = driveXSpring;
        drive.positionDamper = driveXDamper;
        drive.maximumForce = driveXMaxF;
        joint.angularXDrive = drive;

        drive.positionSpring = driveYZSpring;
        drive.positionDamper = driveYZDamper;
        drive.maximumForce = driveYZMaxF;
        joint.angularYZDrive = drive;
    }
    #endregion

    #region graphics
    public void BuildMesh()
    {
        // Build mesh(es)
        // check limits
        if (diameter < MIN_DIAM) diameter = MIN_DIAM;
        if (nFaces < MIN_FACES) nFaces = MIN_FACES;

        // Create mesh
        Transform[] bones = new Transform[nNodes];
        Matrix4x4[] bindposes = new Matrix4x4[nNodes];

        for (int i = 0; i < nNodes; i++)
        {
            bones[i] = nodes[i].transform;
            bindposes[i] = nodes[i].transform.worldToLocalMatrix * transform.localToWorldMatrix;
        }

        int nVertices = (((nNodes + 1) * (nFaces + 1)) + ((nFaces + 1) * 2));
        int nTriangleFaces = nNodes * nFaces * 2;
        int nTrianglesTops = (2 * (nFaces - 2));

        Vector3[] vertices = new Vector3[nVertices];
        Vector2[] mapping = new Vector2[nVertices];
        Vector4[] tangents = new Vector4[nVertices];
        BoneWeight[] weights = new BoneWeight[nVertices];
        int[] trianglesFaces = new int[nTriangleFaces * 3];
        int[] trianglesTops = new int[nTrianglesTops * 3];

        GetTopsIndices(ref trianglesTops);

        int vIndex = 0;

        for (int i = 0; i < nNodes + 1; i++)
        {
            int boneIndex = i < nNodes ? i : nNodes - 1;

            if (i < nNodes) GetFacesIndices(i, ref trianglesFaces);

            for (int r = 0; r < (i == 0 || i == nNodes ? 2 : 1); r++)
            {
                for (int nFace = 0; nFace < nFaces + 1; nFace++)
                {
                    float fRopeT = (float)i / (float)nNodes;
                    float cos = Mathf.Cos(((float)nFace / (float)nFaces) * Mathf.PI * 2.0f);
                    float sin = Mathf.Sin(((float)nFace / (float)nFaces) * Mathf.PI * 2.0f);

                    vertices[vIndex] = new Vector3(cos * diameter * 0.5f, sin * diameter * 0.5f, i < nNodes ? 0.0f : nodeLength);
                    vertices[vIndex] = (bones[boneIndex].TransformPoint(vertices[vIndex]));
                    vertices[vIndex] = this.transform.InverseTransformPoint(vertices[vIndex]);

                    if ((i == 0 && r == 0) || (i == nNodes && r == 1))
                    {
                        mapping[vIndex] = new Vector2(Mathf.Clamp01((cos + 1.0f) * 0.1f), Mathf.Clamp01((sin + 1.0f) * 0.1f));
                        tangents[vIndex] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
                    }
                    else
                    {
                        mapping[vIndex] = new Vector2(fRopeT * ropeLength, (float)nFace / (float)nFaces);
                        tangents[vIndex] = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
                    }

                    weights[vIndex].boneIndex0 = boneIndex;
                    weights[vIndex].boneIndex1 = boneIndex;
                    weights[vIndex].weight0 = 1f;

                    vIndex++;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = mapping;
        mesh.boneWeights = weights;
        mesh.bindposes = bindposes;
        mesh.subMeshCount = 2;
        mesh.SetTriangles(trianglesFaces, 0);
        mesh.SetTriangles(trianglesTops, 1);
        mesh.RecalculateNormals();
        mesh.tangents = tangents;

        skinnedRend.bones = bones;
        skinnedRend.sharedMesh = mesh;
        skinnedRend.updateWhenOffscreen = true;

        SetMaterial();
    }

    public void GetFacesIndices(int nodeIndex, ref int[] indices)
    {
        int tIndex = nodeIndex * nFaces * 2;
        int initvIndex = (nodeIndex * (nFaces + 1)) + (nFaces + 1);

        for (int nFace = 0; nFace < nFaces; nFace++)
        {
            int vIndex = initvIndex + nFace;

            indices[tIndex * 3 + 2] = vIndex;
            indices[tIndex * 3 + 1] = vIndex + nFaces + 1;
            indices[tIndex * 3 + 0] = vIndex + 1;
            indices[tIndex * 3 + 5] = vIndex + 1;
            indices[tIndex * 3 + 4] = vIndex + nFaces + 1;
            indices[tIndex * 3 + 3] = vIndex + 1 + nFaces + 1;
            tIndex += 2;
        }
    }

    public void GetTopsIndices(ref int[] indices)
    {
        int tIndex = 0;
        for (int i = 0; i < nFaces - 2; i++)
        {
            indices[tIndex * 3 + 0] = 0;
            indices[tIndex * 3 + 1] = (i + 2);
            indices[tIndex * 3 + 2] = (i + 1);
            tIndex++;
        }

        int vIndex = ((nNodes + 1) * (nFaces + 1)) + (nFaces + 1);
        for (int i = 0; i < nFaces - 2; i++)
        {
            indices[tIndex * 3 + 2] = vIndex;
            indices[tIndex * 3 + 1] = vIndex + (i + 2);
            indices[tIndex * 3 + 0] = vIndex + (i + 1);
            tIndex++;
        }
    }

    public void SetMaterial()
    {
        if (skinnedRend != null)
        {
            Material[] materials = new Material[2];
            materials[0] = materials[1] = meshMaterial;
            skinnedRend.materials = materials;
        }
    }
    #endregion

    #region physics
    public void BuildPhysics()
    {
        if (clean == true) return;

        if (sFriction < 0) sFriction = 0;
        if (dFriction < 0) dFriction = 0;

        float capRadius = diameter * 0.5f;
        float capCenter = nodeLength * 0.5f;

        PhysicMaterial physicsMaterial = new PhysicMaterial();
        physicsMaterial.bounceCombine = bounceMode;
        physicsMaterial.frictionCombine = frictionMode;
        physicsMaterial.dynamicFriction = dFriction;
        physicsMaterial.staticFriction = sFriction;
        physicsMaterial.bounciness = bounciness;

        foreach (GameObject node in nodes)
        {
            CapsuleCollider nodeCC = node.GetComponent<CapsuleCollider>() != null ? node.GetComponent<CapsuleCollider>() : node.AddComponent<CapsuleCollider>();
            nodeCC.center = new Vector3(0.0f, 0.0f, capCenter);
            nodeCC.radius = capRadius;
            nodeCC.height = nodeLength;
            nodeCC.direction = 2;
            nodeCC.material = physicsMaterial;

            Rigidbody nodeRB = node.GetComponent<Rigidbody>() != null ? node.GetComponent<Rigidbody>() : node.AddComponent<Rigidbody>();
            nodeRB.mass = nodeMass;
            nodeRB.useGravity = useGravity;
            nodeRB.interpolation = interpolation;
            nodeRB.collisionDetectionMode = colDetection;
            nodeRB.solverIterationCount = solverIterations;
        }
    }
    #endregion
}