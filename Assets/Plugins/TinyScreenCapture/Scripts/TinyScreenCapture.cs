using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Assets.GifAssets.PowerGif;

public class TinyScreenCapture : MonoBehaviour
{
    private static TinyScreenCapture Instance { get; set; }

    [Header ("Prefix for saved filed.")]
    public string fileName = "screenshoot";
    private Texture2D texture;
    List<Texture2D> Frames = new List<Texture2D>();
    void Awake(){
		if (Instance){
			Destroy (gameObject);
		} else {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	void Update()
    {
		if (Input.GetKeyDown("s"))
			StartCoroutine(TinyCapture());
	}

    IEnumerator TinyCapture() {
        bool rer = true;
        int num = 0;
        while (rer)
        {
            yield return new WaitForEndOfFrame();

            texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            //texture.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
            texture.Apply();
            Vector2 tewer = new Vector2(192,192);
            texture = ResizeTexture(texture, tewer);
            Frames.Add(texture);

            yield return 0;
            byte[] bytes = texture.EncodeToPNG();
            string timestamp = System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");

            Debug.Log(Application.dataPath + "/ScreenCapture/" + fileName + "_" + num + ".png");
            File.WriteAllBytes(Application.dataPath + "/ScreenCapture/" + fileName + "_" + num + ".png", bytes);
            num++;
            yield return new WaitForSeconds(0.1f);

            if (num >= 10)
            {
                rer = false;
            }
        }
        var frames = Frames.Select(f => new GifFrame(f, 0.1f)).ToList();
        var gif = new Gif(frames);
        /*var path = UnityEditor.EditorUtility.SaveFilePanel("Save", "Assets/GifAssets/PowerGif/Examples/Samples", "EncodeExample", "gif");
        if (path == "") return;*/

        var bytes2 = gif.Encode();
        //File.WriteAllBytes(path, bytes2);
        File.WriteAllBytes(Application.dataPath + "/ScreenCapture/" + fileName + "_" + "" + ".gif", bytes2);
        //Debug.LogFormat("Saved to: {0}", path);
    }
    public static Texture2D ResizeTexture(Texture2D source, Vector2 size)
    {
        //*** Get All the source pixels
        Color[] aSourceColor = source.GetPixels(0);
        Vector2 vSourceSize = new Vector2(source.width, source.height);

        //*** Calculate New Size
        float xWidth = size.x;
        float xHeight = size.y;

        //*** Make New
        Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

        //*** Make destination array
        int xLength = (int)xWidth * (int)xHeight;
        Color[] aColor = new Color[xLength];

        Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

        //*** Loop through destination pixels and process
        Vector2 vCenter = new Vector2();
        for (int ii = 0; ii < xLength; ii++)
        {
            //*** Figure out x&y
            float xX = (float)ii % xWidth;
            float xY = Mathf.Floor((float)ii / xWidth);

            //*** Calculate Center
            vCenter.x = (xX / xWidth) * vSourceSize.x;
            vCenter.y = (xY / xHeight) * vSourceSize.y;

            //*** Average
            //*** Calculate grid around point
            int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
            int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
            int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
            int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

            //*** Loop and accumulate
            Color oColorTemp = new Color();
            float xGridCount = 0;
            for (int iy = xYFrom; iy < xYTo; iy++)
            {
                for (int ix = xXFrom; ix < xXTo; ix++)
                {

                    //*** Get Color
                    oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                    //*** Sum
                    xGridCount++;
                }
            }

            //*** Average Color
            aColor[ii] = oColorTemp / (float)xGridCount;
        }
        Debug.Log(aColor.Length);
        //*** Set Pixels
        oNewTex.SetPixels(aColor);
        oNewTex.Apply();

        //*** Return
        return oNewTex;
    }
}
