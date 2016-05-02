using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rope))]
public class RopeEditor : Editor
{
    #region rope props
    SerializedProperty PropRopeEnd;
    SerializedProperty PropRopeLengh;
    SerializedProperty PropRopeNumLinks;
    SerializedProperty PropRopeDiameter;

    SerializedProperty PropRopeFaces;
    SerializedProperty PropRopeMaterial;

    SerializedProperty PropDynamicFriction;
    SerializedProperty PropStaticFriction;
    SerializedProperty PropBounciness;
    SerializedProperty PropNodeMass;
    SerializedProperty PropNodeUseGrav;
    SerializedProperty PropSolverIterations;

    SerializedProperty PropAngularXSpring;
    SerializedProperty PropAngularXDamper;
    SerializedProperty PropAngularYZSpring;
    SerializedProperty PropAngularYZDamper;
    SerializedProperty PropAngularXLimit;
    SerializedProperty PropAngularXBounce;
    SerializedProperty PropAngularXContact;
    SerializedProperty PropAngularYLimit;
    SerializedProperty PropAngularYBounce;
    SerializedProperty PropAngularYContact;
    SerializedProperty PropAngularZLimit;
    SerializedProperty PropAngularZBounce;
    SerializedProperty PropAngularZContact;
    SerializedProperty PropDriveXSpring;
    SerializedProperty PropDriveXDamper;
    SerializedProperty PropDriveXMaxF;
    SerializedProperty PropDriveYZSpring;
    SerializedProperty PropDriveYZDamper;
    SerializedProperty PropDriveYZMaxF;
    #endregion

    #region menu item
    [MenuItem("GameObject/Create Other/Rope")]
    static void CreateRope()
    {
        GameObject rope = new GameObject();
        rope.name = "Rope";
        rope.transform.position = Selection.activeTransform != null ? Selection.activeTransform.position : Vector3.zero;
        rope.AddComponent<Rigidbody>();
        rope.GetComponent<Rigidbody>().isKinematic = true;
        rope.AddComponent<Rope>();

        GameObject tip = new GameObject();
        tip.name = "Rope Tip";
        tip.transform.position = Selection.activeTransform != null ? new Vector3(Selection.activeTransform.position.x + 2.0f,
            Selection.activeTransform.position.y, Selection.activeTransform.position.z) : new Vector3(2.0f, 0.0f, 0.0f);
        tip.AddComponent<Rigidbody>();
        rope.GetComponent<Rope>().ropeEnd = tip;

        rope.GetComponent<Rope>().Build();

        Material defaultMat = Resources.Load<Material>("Materials/DefaultRopeMaterial");
        if (defaultMat != null) { rope.GetComponent<Rope>().meshMaterial = defaultMat; rope.GetComponent<Rope>().SetMaterial(); }

        Selection.activeGameObject = rope;
    }
    #endregion

    #region editor logic
    void OnEnable()
    {
        PropRopeEnd = serializedObject.FindProperty("ropeEnd");
        PropRopeLengh = serializedObject.FindProperty("ropeLength");
        PropRopeNumLinks = serializedObject.FindProperty("nNodes");
        PropRopeDiameter = serializedObject.FindProperty("diameter");

        PropRopeFaces = serializedObject.FindProperty("nFaces");
        PropRopeMaterial = serializedObject.FindProperty("meshMaterial");

        PropDynamicFriction = serializedObject.FindProperty("dFriction");
        PropStaticFriction = serializedObject.FindProperty("sFriction");
        PropBounciness = serializedObject.FindProperty("bounciness");
        PropNodeMass = serializedObject.FindProperty("nodeMass");
        PropNodeUseGrav = serializedObject.FindProperty("useGravity");
        PropSolverIterations = serializedObject.FindProperty("solverIterations");

        PropAngularXSpring = serializedObject.FindProperty("angularXSpring");
        PropAngularXDamper = serializedObject.FindProperty("angularXDamper");
        PropAngularYZSpring = serializedObject.FindProperty("angularYZSpring");
        PropAngularYZDamper = serializedObject.FindProperty("angularYZDamper");
        PropAngularXLimit = serializedObject.FindProperty("angularXLimit");
        PropAngularXBounce = serializedObject.FindProperty("angularXBounce");
        PropAngularXContact = serializedObject.FindProperty("angularXContact");
        PropAngularYLimit = serializedObject.FindProperty("angularYLimit");
        PropAngularYBounce = serializedObject.FindProperty("angularYBounce");
        PropAngularYContact = serializedObject.FindProperty("angularYContact");
        PropAngularZLimit = serializedObject.FindProperty("angularZLimit");
        PropAngularZBounce = serializedObject.FindProperty("angularZBounce");
        PropAngularZContact = serializedObject.FindProperty("angularZContact");
        PropDriveXSpring = serializedObject.FindProperty("driveXSpring");
        PropDriveXDamper = serializedObject.FindProperty("driveXDamper");
        PropDriveXMaxF = serializedObject.FindProperty("driveXMaxF");
        PropDriveYZSpring = serializedObject.FindProperty("driveYZSpring");
        PropDriveYZDamper = serializedObject.FindProperty("driveYZDamper");
        PropDriveYZMaxF = serializedObject.FindProperty("driveYZMaxF");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Rope rope = target as Rope;

        bool rebuild = false;
        bool setNodes = false;
        bool setJoints = false;
        bool setMaterial = false;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Rope"), EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        PropRopeEnd.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("End"), PropRopeEnd.objectReferenceValue, typeof(GameObject), true);
        PropRopeLengh.floatValue = EditorGUILayout.FloatField(new GUIContent("Length"), PropRopeLengh.floatValue, GUILayout.ExpandWidth(true));
        PropRopeNumLinks.intValue = EditorGUILayout.IntField(new GUIContent("Divisions"), PropRopeNumLinks.intValue, GUILayout.ExpandWidth(true));
        PropRopeDiameter.floatValue = EditorGUILayout.FloatField(new GUIContent("Diameter"), PropRopeDiameter.floatValue);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Graphics"), EditorStyles.boldLabel);
        PropRopeFaces.intValue = EditorGUILayout.IntField(new GUIContent("Cylinder Sides"), PropRopeFaces.intValue);
        if (EditorGUI.EndChangeCheck()) rebuild = true;

        EditorGUI.BeginChangeCheck();
        PropRopeMaterial.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Mesh Material"), PropRopeMaterial.objectReferenceValue, typeof(Material), false);
        if (EditorGUI.EndChangeCheck()) setMaterial = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Node Physics"), EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        PropDynamicFriction.floatValue = EditorGUILayout.FloatField(new GUIContent("Dynamic Friction"), PropDynamicFriction.floatValue);
        PropStaticFriction.floatValue = EditorGUILayout.FloatField(new GUIContent("Static Friction"), PropStaticFriction.floatValue);
        EditorGUILayout.Slider(PropBounciness, 0.0f, 1, new GUIContent("Bounciness"));
        rope.frictionMode = (PhysicMaterialCombine)EditorGUILayout.EnumPopup(new GUIContent("Friction Combine"), rope.frictionMode, GUILayout.ExpandWidth(true));
        rope.bounceMode = (PhysicMaterialCombine)EditorGUILayout.EnumPopup(new GUIContent("Bounce Combine"), rope.bounceMode, GUILayout.ExpandWidth(true));
        PropNodeMass.floatValue = EditorGUILayout.FloatField(new GUIContent("Node Mass"), PropNodeMass.floatValue);
        PropNodeUseGrav.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Gravity"), PropNodeUseGrav.boolValue);
        rope.interpolation = (RigidbodyInterpolation)EditorGUILayout.EnumPopup(new GUIContent("Interpolation"), rope.interpolation, GUILayout.ExpandWidth(true));
        rope.colDetection = (CollisionDetectionMode)EditorGUILayout.EnumPopup(new GUIContent("Collision Detection"), rope.colDetection, GUILayout.ExpandWidth(true));
        EditorGUILayout.IntSlider(PropSolverIterations, 1, 255, new GUIContent("Solver Iterations"));
        if (EditorGUI.EndChangeCheck()) setNodes = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(new GUIContent("Joint Physics"), EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField(new GUIContent("Angular Limits Spring Effect"));
        PropAngularXSpring.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular X Spring"), PropAngularXSpring.floatValue);
        PropAngularXDamper.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular X Damper"), PropAngularXDamper.floatValue);
        PropAngularYZSpring.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular YZ Spring"), PropAngularYZSpring.floatValue);
        PropAngularYZDamper.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular YZ Damper"), PropAngularYZDamper.floatValue);
        EditorGUILayout.LabelField(new GUIContent("Angular Limits"));
        EditorGUILayout.Slider(PropAngularXLimit, 0.0f, 177.0f, new GUIContent("Angular X Limit"));
        EditorGUILayout.Slider(PropAngularXBounce, 0.0f, 1.0f, new GUIContent("Angular X Bounciness"));
        PropAngularXContact.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular X Contact"), PropAngularXContact.floatValue);
        EditorGUILayout.Slider(PropAngularYLimit, 0.0f, 177.0f, new GUIContent("Angular Y Limit"));
        EditorGUILayout.Slider(PropAngularYBounce, 0.0f, 1.0f, new GUIContent("Angular Y Bounciness"));
        PropAngularYContact.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular Y Contact"), PropAngularYContact.floatValue);
        EditorGUILayout.Slider(PropAngularZLimit, 0.0f, 177.0f, new GUIContent("Angular Z Limit"));
        EditorGUILayout.Slider(PropAngularZBounce, 0.0f, 1.0f, new GUIContent("Angular Z Bounciness"));
        PropAngularZContact.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular Z Contact"), PropAngularZContact.floatValue);
        EditorGUILayout.LabelField(new GUIContent("Angular Drive"));
        PropDriveXSpring.floatValue = EditorGUILayout.FloatField(new GUIContent("Drive X Spring"), PropDriveXSpring.floatValue);
        PropDriveXDamper.floatValue = EditorGUILayout.FloatField(new GUIContent("Drive X Damper"), PropDriveXDamper.floatValue);
        PropDriveXMaxF.floatValue = EditorGUILayout.FloatField(new GUIContent("Drive X MaxF"), PropDriveXMaxF.floatValue);
        PropDriveYZSpring.floatValue = EditorGUILayout.FloatField(new GUIContent("Drive YZ Spring"), PropDriveYZSpring.floatValue);
        PropDriveYZDamper.floatValue = EditorGUILayout.FloatField(new GUIContent("Drive YZ Damper"), PropDriveYZDamper.floatValue);
        PropDriveYZMaxF.floatValue = EditorGUILayout.FloatField(new GUIContent("Drive YZ MaxF"), PropDriveYZMaxF.floatValue);
        if (EditorGUI.EndChangeCheck()) setJoints = true;

        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();

        if (rebuild)
            rope.Build();
        else
        {
            if (setMaterial) rope.SetMaterial();
            if (setNodes) rope.BuildPhysics();
            if (setJoints) rope.SetJoints();
        }
    }
    #endregion
}