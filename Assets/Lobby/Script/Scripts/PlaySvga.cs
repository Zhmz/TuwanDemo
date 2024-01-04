using Svga;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class PlaySvga : MonoBehaviour
{
    public SvgaPlayer Player;
    public bool isBatching = false;

    public static string DEFAULT_SVGA_URL = "https://img3.tuwandata.com/uploads/play/1916771563790770.svga";

    // Start is called before the first frame update
    void Awake()
    {

        //Canvas canvas = this.GetComponentInParent<Canvas>();
        //canvas.worldCamera = Camera.main;
        //canvas.planeDistance = 10;

        //string path = "https://img3.tuwandata.com/uploads/play/1916771563790770.svga";
        //StartCoroutine(ReadData(path));

        //string chrismasPath = "https://img3.tuwandata.com/uploads/play/8418431577015639.svga";

        //string lipsPath = "https://img3.tuwandata.com/uploads/play/1916771563790770.svga";
        //StartCoroutine(LoadSVGA(lipsPath));
    }

    public void PlaySVGAWithURL(string giftUrl = "")
    {
        if (string.IsNullOrEmpty(giftUrl))
        {
            StartCoroutine(LoadSVGA(DEFAULT_SVGA_URL));
        }
        else
        {
            StartCoroutine(LoadSVGA(giftUrl));
        }
    }

    IEnumerator ReadData(string path)
    {
        WWW www = new WWW(path);
        yield return www;
        while (www.isDone == false)
        {
            yield return new WaitForEndOfFrame();
        }

        var data = www.bytes;

        using (Stream stream = new MemoryStream(data))
        {
            Player?.LoadSvgaFileData(stream);
        }

        yield return new WaitForEndOfFrame();
    }

    public void Play()
    {
        Player?.Play(0, () => Debug.Log("Play complete."));
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
}
