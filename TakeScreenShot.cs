using UnityEngine;
using System.Collections;

public class TakeScreenShot : MonoBehaviour
{

    public Texture2D screenshot = null;
    private RenderTexture renderTexture = null;
    private bool grab = false;

    private void Start()
    {
        screenshot = null;
    }

    public void TakeScreenshot()
    {        
        StartCoroutine(WaitForScreenshot());
    }


    private IEnumerator WaitForScreenshot()
    {
        yield return new WaitForEndOfFrame();

        screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false); 
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false); /// Read pixels from screen into the saved texture data.
        screenshot.Apply(); /// Actually apply all previous SetPixel and SetPixels changes.
    }

}
