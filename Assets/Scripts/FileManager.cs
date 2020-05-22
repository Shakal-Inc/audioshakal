using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
    string path;
    public RawImage image;

    public void OpenExplorer()
    {
        path = EditorUtility.OpenFilePanel("Hello World", "", "jpg");
    }

    void GetImg()
    {
        if (path != null)
        {
            UpdateImg();
        }
    }

    void UpdateImg()
    {
        WWW www = new WWW("file:///" + path);
        image.texture = www.texture;
    }
}
