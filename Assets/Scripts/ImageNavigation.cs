using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Photon.Pun;

public class ImageNavigation : MonoBehaviour
{
    public RawImage rawImage;
    Texture2D[] tex;
    int currentImageIndex;
    int maxFiles;
    void Start()
    {
        currentImageIndex = 0;
        maxFiles=0;
        if (Directory.Exists(Application.persistentDataPath + "/Upload"))
        {
            DirectoryInfo dirInfo =new DirectoryInfo(Application.persistentDataPath+"/Upload");
            FileInfo[] files= dirInfo.GetFiles();
            maxFiles = files.Length;
            tex = new Texture2D[maxFiles];
            int i = 0;
            foreach(FileInfo file in files)
            {
                byte[] fileData = File.ReadAllBytes(Application.persistentDataPath + "/Upload/"+file.Name);
                tex[i] = new Texture2D(2, 2);
                tex[i].LoadImage(fileData);
                i++;
            }
            SetRawImage(currentImageIndex);
        }
    }
    public void PreviousImage()
    {
        currentImageIndex = (int)Mathf.Repeat(--currentImageIndex,maxFiles);
        SetRawImage(currentImageIndex);
        Debug.Log("prev");
    }
    void SetRawImage(int index)
    {
        PhotonView view = PhotonView.Get(this);
        view.RPC("SetImageRPC", RpcTarget.All, index);
    }
    [PunRPC]
    void SetImageRPC(int index)
    {
        currentImageIndex = index;
        float ratio=(float)tex[currentImageIndex].width/ tex[currentImageIndex].height;
        rawImage.rectTransform.sizeDelta = new Vector2(200,200/ratio);
        rawImage.texture = tex[currentImageIndex];
    }
    public void NextImage()
    {
        currentImageIndex = (int)Mathf.Repeat(++currentImageIndex, maxFiles);
        SetRawImage(currentImageIndex);
    }
}
