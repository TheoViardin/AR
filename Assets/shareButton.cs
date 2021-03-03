using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.InteropServices;

public class ScreenShotShare : MonoBehaviour
{

    public string subject, ShareMessage, url;
    private bool isProcessing = false;
    public string ScreenshotName = "DenysScreenshot.png";

    public void ShareScreenshotWithText()
    {
        Share();

    }

    public void Share()
    {

        if (!isProcessing)
        {
            StartCoroutine(ShareScreenshot());
        }

        IEnumerator ShareScreenshot()
        {
            isProcessing = true;

            // wait for graphics to render
            yield return new WaitForEndOfFrame();
            string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
            ScreenCapture.CaptureScreenshot(ScreenshotName);



            yield return new WaitForSeconds(1f);
            if (!Application.isEditor)
            {


                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + screenShotPath);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
                intentObject.Call<AndroidJavaObject>("setType", "image/png");

                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), ShareMessage);

                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

                AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Picture");
                currentActivity.Call("startActivity", jChooser);

            }
            isProcessing = false;
        }
    }
}