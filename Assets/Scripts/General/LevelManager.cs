using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    public GameObject scenePanel;

    private bool show = false;

    void Start() { show = GetComponentInChildren<Toggle>().isOn; scenePanel.SetActive(show); }

    public void togglePanel() { scenePanel.SetActive(!show); show = !show; }

	public void LoadScene(int id) { SceneManager.LoadScene(id); }

    public void Quit() { Application.Quit(); }
}