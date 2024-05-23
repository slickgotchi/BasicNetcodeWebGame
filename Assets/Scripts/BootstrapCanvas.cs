using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootstrapCanvas : MonoBehaviour
{
    public Button ServerButton;
    public Button ClientButton;

    private void Start() {
        ServerButton.onClick.AddListener(() => {
            ConnectionManager.Instance.ConnectionMode = ConnectionMode.Server;
            SceneManager.LoadScene("CreateGame");
        });

        ClientButton.onClick.AddListener(() => {
            ConnectionManager.Instance.ConnectionMode = ConnectionMode.Client;
            SceneManager.LoadScene("CreateGame");
        });
    }
}
