using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{

    private static Bootstrap _instance;
    public static Bootstrap Instance {
        get {
            if (_instance == null) {
                _instance = FindAnyObjectByType<Bootstrap>() ?? new GameObject("Bootstrap").AddComponent<Bootstrap>();
            }
            return _instance;
        }
    }

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }



    private void Start() {

    }
}

public enum ConnectionMode {
    ServerClient, Server, Client,
}