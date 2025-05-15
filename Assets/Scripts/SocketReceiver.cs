using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using System.Linq;

public class SocketReceiver : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public int port = 6000;
    [HideInInspector]
    public Texture2D outputTexture;

    private TcpClient client;
    private NetworkStream stream;
    private byte[] lengthBuffer = new byte[4];

    public static SocketReceiver Instance { get; private set; }

    void Awake()
    {
        // Singleton 設置
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        client = new TcpClient(serverIP, port);
        stream = client.GetStream();
        Debug.Log("Connected to Python server!");

        outputTexture = new Texture2D(2, 2, TextureFormat.RGB24, false);
    }

    void Update()
    {
        if (stream.DataAvailable)
        {
            stream.Read(lengthBuffer, 0, 4);
            int imageLength = BitConverter.ToInt32(lengthBuffer.Reverse().ToArray(), 0);

            byte[] imageBuffer = new byte[imageLength];
            int totalRead = 0;
            while (totalRead < imageLength)
            {
                int read = stream.Read(imageBuffer, totalRead, imageLength - totalRead);
                if (read == 0) break;
                totalRead += read;
            }

            outputTexture.LoadImage(imageBuffer);
        }
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
        outputTexture = new Texture2D(2, 2, TextureFormat.RGB24, false);
    }
}