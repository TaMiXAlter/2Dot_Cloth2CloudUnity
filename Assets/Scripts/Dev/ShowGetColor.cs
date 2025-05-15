using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGetColor : MonoBehaviour
{
    RawImage rawImage;
    // Start is called before the first frame update
    void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        rawImage.texture = SocketReceiver.Instance.outputTexture;
    }
}
