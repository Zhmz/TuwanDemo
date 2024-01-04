using System.Collections;
using System.Collections.Generic;
using System.IO;
using Svga;
using UnityEngine;
using UnityEngine.Networking;

public class SVGAContent : MonoBehaviour
{
    public SvgaPlayer Player;
    public string url;
    public bool isBatching;
    
    void Awake()
    {
        string path =
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.streamingAssetsPath + "/image.svga";
#elif UNITY_IPHONE && !UNITY_EDITOR
        "file://" + Application.streamingAssetsPath + "/image.svga";
#elif UNITY_STANDLONE_WIN||UNITY_EDITOR
            "file://" + Application.streamingAssetsPath + "/image.svga";
#else
        string.Empty;
#endif
        //https://img3.tuwandata.com/uploads/play/8418431577015639.svga
        if (!string.IsNullOrEmpty(this.url))
        {
            path = this.url;
        }
        StartCoroutine(LoadSVGA(path));
    }

    IEnumerator LoadSVGA(string path)
    {
        // Download
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        var data = request.downloadHandler.data;
        using (Stream stream = new MemoryStream(data))
        {
            Player.LoadSvgaFileData(path, stream, isBatching);
        }
        yield return new WaitForEndOfFrame();
    }

    public void Play()
    {
        Player.Play(0, () => Debug.Log("Play complete."));
    }

    public void Pause()
    {
        Player.Pause();
    }
}