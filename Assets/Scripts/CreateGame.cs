using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.Instance.Connect();
        SceneManager.LoadScene("Game");
    }
}
