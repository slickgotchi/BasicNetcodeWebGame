using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    private static ConnectionManager _instance;
    public static ConnectionManager Instance {
        get {
            if (_instance == null) {
                _instance = FindAnyObjectByType<ConnectionManager>() ?? new GameObject("ConnectionManager").AddComponent<ConnectionManager>();
            }
            return _instance;
        }
    }

    // these are self-signed only and not used anywhere else...
    private string _serverCertificate = "-----BEGIN CERTIFICATE-----\r\nMIIDOTCCAiECFD/cVzhCppyxAizJqDLmyALoTs/eMA0GCSqGSIb3DQEBCwUAMFkx\r\nCzAJBgNVBAYTAkFVMRMwEQYDVQQIDApTb21lLVN0YXRlMSEwHwYDVQQKDBhJbnRl\r\ncm5ldCBXaWRnaXRzIFB0eSBMdGQxEjAQBgNVBAMMCWxvY2FsaG9zdDAeFw0yNDA1\r\nMjMwNjQ2MjNaFw0yNTA1MjMwNjQ2MjNaMFkxCzAJBgNVBAYTAkFVMRMwEQYDVQQI\r\nDApTb21lLVN0YXRlMSEwHwYDVQQKDBhJbnRlcm5ldCBXaWRnaXRzIFB0eSBMdGQx\r\nEjAQBgNVBAMMCWxvY2FsaG9zdDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoC\r\nggEBAJEsc+6SNLtnjzgIiDYPQT5hy4GpcN7rcwQWx06CqT/BBmPTsMpbswjxHo70\r\nxMUupkow2C40fSLdaTu3IjrW2Zs0uRq3KDp63h0/wHrz6ZRO6oQidD6GZRq4GWAR\r\nbUwhPmCKd/cwaPdMP1YEDQYTcIWL3hvn1yJyxtASqnQMKarbwSZ1VcFnpS5+vUWC\r\ncuuCKbPurXsqUA6wXd+kQ94vdNoELRBM2+ocd7Aakw3XHgtz/uhgzhOxj8C33vqW\r\n0sN4GC8eSzHmj16gydIGCooVaI1Z1FAm4CuR+6wMXxuh7+yIVBvBLB2UlauF2mdX\r\nkWUELcbh9ZK36FTIrEAQSBeQ2ukCAwEAATANBgkqhkiG9w0BAQsFAAOCAQEAfvfp\r\n1tBRQP+JEn+0Fwp9JA7U0BDWPvsS0l686Pke1t/z8hOeWXVgo+x7ptlPp8179zl4\r\nMtUZJcpLoMva4jh6LOQIU+ahZT22EUhnE20nbkKB0tn5Jqba8sC33fhIkZolL8ox\r\nfbu/TEyqlTi0HL+7hoRnvVxLRzFwzT+3OzO1oD0637moeqarTNtZR3FxcFVeYE3L\r\n5pSvPihS8HBpiIAUkZ1C0Xtmq7MC5ffZnZ1H309AKtxIjvIuEoXsjSc+b8zcnENR\r\naII6RzZD0JFd6b/HU/WhJCnJey74+w3Ge7qLHVGYkPYngWZaq4JC4I6t70TvLvDc\r\nsTC4iDPuB1KEm0Ue2Q==\r\n-----END CERTIFICATE-----"; 
    private string _serverName = "localhost";
    private string _serverPrivateKey = "-----BEGIN PRIVATE KEY-----\r\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCRLHPukjS7Z484\r\nCIg2D0E+YcuBqXDe63MEFsdOgqk/wQZj07DKW7MI8R6O9MTFLqZKMNguNH0i3Wk7\r\ntyI61tmbNLkatyg6et4dP8B68+mUTuqEInQ+hmUauBlgEW1MIT5ginf3MGj3TD9W\r\nBA0GE3CFi94b59cicsbQEqp0DCmq28EmdVXBZ6Uufr1FgnLrgimz7q17KlAOsF3f\r\npEPeL3TaBC0QTNvqHHewGpMN1x4Lc/7oYM4TsY/At976ltLDeBgvHksx5o9eoMnS\r\nBgqKFWiNWdRQJuArkfusDF8boe/siFQbwSwdlJWrhdpnV5FlBC3G4fWSt+hUyKxA\r\nEEgXkNrpAgMBAAECggEACgxl5OJFWKBpKyN6DCXfqfn6bFnmWGO7NRH/jylfnixV\r\n7SR6tPMGcCzCZPo/wXdCZtm9KOuUqsxhC5NYQPLcoo/SbS020V9uTt96CvillxCk\r\nTmuVGLvUNTZ/eSUTp+SYLPYAqkE1TsK+Eo3auewwCmpdQBi6zSVdKgtUtY4e9za+\r\nlPiSseIu9ql2yvqvdCCz/UswIT8zuSjUB9WqV8df4h5Oc/XRXygBolW/rvfMbcVW\r\njvjhTJFYCziuOAcDjTm1hw1zY1Nm0gaVqTTNvbUlocn0VCGpDwO33R0Tzkh5Q7Pk\r\n8DUKR8ztYm6tzeyb8HucHLufvskz2U87aVlsK4UggQKBgQDE8IwjyrNKSmF7OixN\r\nM6SAgLQDpLqQUClXf+fcpzRpQLbzyZqRvAGiiZPG8FhTPKjNTEbbkAXjx7Cvs+Bu\r\niR3UgVRCtuUF8oskD6PEsKNZ0emLtvz36/rPE/ZBIvevJ0mz6/+r2TPWzLX6SEwA\r\ng+wvQ+pAHlGfBL8VWm0xtcxZqQKBgQC8tbnPKfCJZtH2jXJsuRzYu2i/D4PiNIdT\r\nFlFoasoF4qCWVdkONL4jFMGhB462ZyMZjR8cH5rDbn9ZyaezNVRPdXgQ0TFpvQt+\r\nLrYjHQvHi0DCYO10k5PLhcM/dXfBeRd7CG++GY+PvdYyZYlWYURuSd76S6SRb1qa\r\nPHTaQSG/QQKBgHK13hrW7XmfO4qIRtes9et3i+L3Z0e5uWri8I3AwrFv5WC4lBTA\r\n2n7u/amJwxiwo235OOiYyiVz4gFll5squLpXnlnBqEK/lLYreeuOK4ec0hj0PAK4\r\na/2EFhK4qGZYTwDCvMg/GofJ81FWHRbLwJ2DRIWWY1ppbFemtSWYS/AJAoGAchAx\r\n4JtMDfE9RhbXLApz+jjFJn89SzRO+5TI3iF2PTpvsI7xZNnSd6frJedIhs4udBpN\r\nzJT52djnVyFBoVvBu0mYqimYAX7H/JxFcVH0Ncfg+9zUiptNQT539tqM6T/FRpOh\r\n3zjaAptPZiTjb3fZggRap9WjAllZXVLfmrC1skECgYAauVlRhm5yazRnBqQhGxRO\r\n8Hf/m3Pttg+8vetKtHo+0uISdnAm3rtHf/2LIcInVfb5qu36yOjPZEqaDCNh7DeN\r\ndtBZOPGiDjLGFMI6KjPTf7WImz5gyQKMa9wyg26Lpvq1KRljLqA9PoLjCMCCYT17\r\nyFhZtLW8kVyH41FvIL5xMA==\r\n-----END PRIVATE KEY-----";

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public ConnectionMode ConnectionMode;
    public string ListenIp = "0.0.0.0";
    public string ConnectIp = "127.0.0.1";
    public ushort Port = 8484;

    public static World ServerWorld = null;
    public static World ClientWorld = null;


    public void Connect() {
        DestroyDefaultWorld();

        switch (ConnectionMode) {
            case ConnectionMode.ServerClient: {
                    StartServer();
                    StartClient();
                    break;
                }
            case ConnectionMode.Server: {
                    StartServer();
                    break;
                }
            case ConnectionMode.Client: {
                    StartClient();
                    break;
                }
            default: break;
        }
    }

    /*
    private void StartServer() {
        ServerWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        var listenEndpoint = NetworkEndpoint.Parse(ListenIp, Port);
        {
            using var networkDriverQuery = ServerWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(listenEndpoint);
        }
    }

    private void StartClient() {
        ClientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
        var connectionEndpoint = NetworkEndpoint.Parse(ConnectIp, Port);
        {
            using var networkDriverQuery = ClientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(ClientWorld.EntityManager, connectionEndpoint);
        }
        World.DefaultGameObjectInjectionWorld = ClientWorld;
    }
    */

    public void StartServer() {
        NetworkStreamReceiveSystem.DriverConstructor = new MyCustomDriverConstructor() {
            serverCertificate = _serverCertificate,
            serverPrivateKey = _serverPrivateKey,
        };

        ServerWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");

        Debug.Log($"Starting server on port: {Port}");

        var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);
        {
            using var networkDriverQuery = ServerWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
        }
    }

    public void StartClient() {
        Debug.Log($"Connecting to {_serverName}:{Port} secure: {true} networkfamily: {NetworkFamily.Ipv4}");

        NetworkStreamReceiveSystem.DriverConstructor = new MyCustomDriverConstructor() {
            connectWithSsl = true,
            hostname = _serverName
        };

        ClientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        var connectionEndpoint = NetworkEndpoint.Parse(ConnectIp, Port, NetworkFamily.Ipv4);
        {
            using var networkDriverQuery = ClientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(ClientWorld.EntityManager, connectionEndpoint);
        }

        World.DefaultGameObjectInjectionWorld = ClientWorld;
    }

    private void DestroyDefaultWorld() {
        foreach (var world in World.All) {
            if (world.Flags == WorldFlags.Game) {
                world.Dispose();
                break;
            }
        }
    }
}
