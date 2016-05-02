using UnityEngine;

public class ROVController : MonoBehaviour
{
    #region public data
    public enum ControlMode { Normal, Advanced }

    public GameObject baseBody;
    public GameObject topBody;

    public Transform[] directionalProps;
    public Transform[] verticalProps;

    public Transform[] directionalBlades;
    public Transform[] verticalBlades;

    public ControlMode controlMode = ControlMode.Normal;

    public float thrustForce = 1.0f;
    public float maxThrust = 1.0f;
    public float normalThrustStep = 0.1f;
    public float advThrustStep = 0.01f;
    public float thrustDecay = 10f;
    public float bladeRotMultiplayer = 500f;

    public KeyCode[] directionalTriggers;
    public KeyCode[] verticalTriggers;
    public KeyCode frontTrigger;
    public KeyCode backTrigger;
    public KeyCode leftTrigger;
    public KeyCode rigthTrigger;
    public KeyCode upTrigger;
    public KeyCode downTrigger;
    public KeyCode modKey;
    public KeyCode changeMode;

    [HideInInspector]
    public float[] directionalThrust;
    [HideInInspector]
    public float[] verticalThrust;
    #endregion

    #region private data
    private ParticleSystem[] directionalWaterflow;
    private ParticleSystem[] verticalWaterflow;

    private Rigidbody baseRB;
    private Rigidbody topRB;

    private bool[] dThrusted;
    private bool[] vThrusted;
    #endregion

    void Awake()
    {
        directionalThrust = new float[4];
        verticalThrust = new float[4];
        dThrusted = new bool[4];
        vThrusted = new bool[4];
        baseRB = baseBody.GetComponent<Rigidbody>();
        topRB = topBody.GetComponent<Rigidbody>();
        directionalWaterflow = new ParticleSystem[4];
        verticalWaterflow = new ParticleSystem[4];

        for (int i = 0; i < 4; i++)
        {
            directionalWaterflow[i] = directionalProps[i].GetChild(0).GetComponent<ParticleSystem>();
            verticalWaterflow[i] = verticalProps[i].GetChild(0).GetComponent<ParticleSystem>();
            dThrusted[i] = false;
            vThrusted[i] = false;
        }
    }

    void Update()
    {
        if (Input.anyKey) ResolveInput();
        if (controlMode != ControlMode.Advanced) UpdateThrustDecay();
        for (int i = 0; i < 4; i++) UpdatePropGraphics(i);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            if (directionalThrust[i] != 0.0f)
                baseRB.AddForceAtPosition(directionalProps[i].forward * directionalThrust[i] * thrustForce, directionalProps[i].position);
            if (verticalThrust[i] != 0.0f)
                topRB.AddForceAtPosition(verticalProps[i].up * verticalThrust[i] * thrustForce, verticalProps[i].position);
        }
    }

    #region input and thrust logic
    private void ResolveInput()
    {
        if (Input.GetKeyDown(changeMode)) ChangeControlMode();

        if (controlMode == ControlMode.Normal)
        {
            if (!Input.GetKey(modKey))
            {
                if (Input.GetKey(frontTrigger) && Input.GetKey(leftTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, (i == 2 || i == 3 ? maxThrust : maxThrust / 2.0f), (i == 1 || i == 2 || i == 3));
                else if (Input.GetKey(frontTrigger) && Input.GetKey(rigthTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, (i == 2 || i == 3 ? maxThrust : maxThrust / 2.0f), (i == 0 || i == 2 || i == 3));
                else if (Input.GetKey(backTrigger) && Input.GetKey(leftTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, (i == 2 || i == 3 ? maxThrust : maxThrust / 2.0f), (i == 0));
                else if (Input.GetKey(backTrigger) && Input.GetKey(rigthTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, (i == 2 || i == 3 ? maxThrust : maxThrust / 2.0f), (i == 1));
                else if (Input.GetKey(leftTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 1 || i == 2));
                else if (Input.GetKey(rigthTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 0 || i == 3));
                else if (Input.GetKey(frontTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust);
                else if (Input.GetKey(backTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, false);
            }
            else
            {
                if (Input.GetKey(frontTrigger) && Input.GetKey(leftTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 0 || i == 1 || i == 2), false);
                else if (Input.GetKey(frontTrigger) && Input.GetKey(rigthTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 0 || i == 1 || i == 3), false);
                else if (Input.GetKey(backTrigger) && Input.GetKey(leftTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 1 || i == 2 || i == 3), false);
                else if (Input.GetKey(backTrigger) && Input.GetKey(rigthTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 0 || i == 2 || i == 3), false);
                else if (Input.GetKey(leftTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 1 || i == 2), false);
                else if (Input.GetKey(rigthTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 0 || i == 3), false);
                else if (Input.GetKey(frontTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 0 || i == 1), false);
                else if (Input.GetKey(backTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep / 4.0f, maxThrust, (i == 2 || i == 3), false);
            }

            if (Input.GetKey(upTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep, maxThrust, true, false);
            else if (Input.GetKey(downTrigger)) for (int i = 0; i < 4; i++) AddThrust(i, normalThrustStep, maxThrust, false, false);
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKey(directionalTriggers[i]))
                {
                    if (!Input.GetKey(modKey)) AddThrust(i, advThrustStep, maxThrust);
                    else AddThrust(i, advThrustStep, maxThrust, false);
                }

                if (Input.GetKey(verticalTriggers[i]))
                {
                    if (!Input.GetKey(modKey)) AddThrust(i, advThrustStep, maxThrust, true, false);
                    else AddThrust(i, advThrustStep, maxThrust, false, false);
                }
            }
        }
    }

    private void AddThrust(int i, float step, float max, bool positive = true, bool directional = true)
    {
        if (directional)
        {
            if (positive)
            {
                if (directionalThrust[i] + step <= max) directionalThrust[i] += step;
                else directionalThrust[i] = max;
            }
            else
            {
                if (directionalThrust[i] - step >= -max) directionalThrust[i] -= step;
                else directionalThrust[i] = -max;
            }
            dThrusted[i] = true;
        }
        else
        {
            if (positive)
            {
                if (verticalThrust[i] + step <= max) verticalThrust[i] += step;
                else verticalThrust[i] = max;
            }
            else
            {
                if (verticalThrust[i] - step >= -max) verticalThrust[i] -= step;
                else verticalThrust[i] = -max;
            }
            vThrusted[i] = true;
        }
    }

    private void UpdateThrustDecay()
    {
        float decay = normalThrustStep * thrustDecay * Time.deltaTime;
        
        for (int i = 0; i < 4; i++)
        {
            if (!dThrusted[i])
            {
                if (Mathf.Approximately(directionalThrust[i], 0.0f) || Mathf.Abs(directionalThrust[i]) - decay < 0.0f) directionalThrust[i] = 0.0f;
                else
                {
                    if (directionalThrust[i] > 0f) directionalThrust[i] -= decay;
                    else directionalThrust[i] += decay;
                }
            }
            else dThrusted[i] = false;

            if (!vThrusted[i])
            {
                if (Mathf.Approximately(verticalThrust[i], 0.0f) || Mathf.Abs(verticalThrust[i]) - decay < 0.0f) verticalThrust[i] = 0.0f;
                else
                {
                    if (verticalThrust[i] > 0f) verticalThrust[i] -= decay;
                    else verticalThrust[i] += decay;
                }
            }
            else vThrusted[i] = false;
        }
    }

    private void UpdatePropGraphics(int i)
    {
        ParticleSystem.EmissionModule eModule;

        directionalBlades[i].Rotate(new Vector3(0f, 0f, directionalThrust[i] * bladeRotMultiplayer * Time.deltaTime), Space.Self);
        verticalBlades[i].Rotate(new Vector3(0f, 0f, verticalThrust[i] * bladeRotMultiplayer * Time.deltaTime), Space.Self);

        float dSpeed = directionalThrust[i] / maxThrust;
        float vSpeed = verticalThrust[i] / maxThrust;

        eModule = directionalWaterflow[i].emission;
        if (Mathf.Approximately(dSpeed, 0)) eModule.enabled = false;
        else
        {
            if (!eModule.enabled) eModule.enabled = true;
            directionalWaterflow[i].startSpeed = dSpeed;
        }

        eModule = verticalWaterflow[i].emission;
        if (Mathf.Approximately(vSpeed, 0)) eModule.enabled = false;
        else
        {
            if (!eModule.enabled) eModule.enabled = true;
            verticalWaterflow[i].startSpeed = vSpeed;
        }
    }
    #endregion

    public void ChangeControlMode()
    {
        if (controlMode == ControlMode.Normal)
            controlMode = ControlMode.Advanced;
        else controlMode = ControlMode.Normal;
    }
}