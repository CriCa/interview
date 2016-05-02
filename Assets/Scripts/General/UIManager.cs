using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    #region public data
    public ROVController rov;
    public Slider[] directionalProps;
    public Slider[] verticalProps;
    public Text crontolMode, info;
    public RawImage compass;
    public RectTransform cameraSelected;
    #endregion

    #region private data
    private CameraController cController;
    #endregion

    void Start () { cController = Camera.main.GetComponent<CameraController>(); }
	
	void Update ()
    {
        for (int i = 0; i < 4; i++)
        {
            if (rov.directionalThrust[i] >= 0.0f) directionalProps[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            else directionalProps[i].transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);

            float value = Mathf.Abs(rov.directionalThrust[i]) / rov.maxThrust;
            directionalProps[i].value = value;
            directionalProps[i].GetComponentInChildren<Image>().color = Color.Lerp(Color.green, Color.red, value);

            if (rov.verticalThrust[i] >= 0.0f) verticalProps[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            else verticalProps[i].transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);

            value = Mathf.Abs(rov.verticalThrust[i]) / rov.maxThrust;
            verticalProps[i].value = value;
            verticalProps[i].GetComponentInChildren<Image>().color = Color.Lerp(Color.green, Color.red, value);
        }

        if (rov.controlMode == ROVController.ControlMode.Normal)
            crontolMode.text = "Control Mode: Normal";
        else crontolMode.text = "Control Mode: Advanced";

        compass.uvRect = new Rect(cController.angle / 360.0f, 0.0f, 1.0f, 1.0f);

        cameraSelected.position = new Vector3(cameraSelected.position.x, cController.selected * 30 + 5, 0);

        UpdateInfo("n2");
    }

    private void UpdateInfo(string precision)
    {
        info.text = "";
        info.text += "Drag: " + rov.baseBody.GetComponent<Rigidbody>().drag.ToString(precision) + "\n";
        info.text += "Velocity: " + rov.baseBody.GetComponent<Rigidbody>().velocity.ToString(precision) + "\n";
        info.text += "Position: " + rov.baseBody.transform.position.ToString(precision) + "\n";
        info.text += "\n";
        info.text += "Angular Drag: " + rov.baseBody.GetComponent<Rigidbody>().angularDrag.ToString(precision) + "\n";
        info.text += "Angular Velocity: " + rov.baseBody.GetComponent<Rigidbody>().angularVelocity.ToString(precision) + "\n";
        info.text += "rotation: " + rov.baseBody.transform.rotation.eulerAngles.ToString(precision);
    }

}
