using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CaptureScreenshot : MonoBehaviour {
    

    [MenuItem("Screenshot/Take screenshot")]


    static void Screenshot()
    {
        ScreenCapture.CaptureScreenshot("Assets/Screenshots/screenshot.png" );
        Debug.Log("Screenshot taken");
    }
}

