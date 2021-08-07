using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ImageLoader : MonoBehaviourPunCallbacks
{
    int downloadCounter = 0;
    int maxImages = 7;
    public RawImage rawImage;
    public Text message;
    FirebaseStorage storage;
    StorageReference storageReference;
    public GameObject joinLobbyButton;

    void Start()
    {
        //initialize storage reference
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://test-8dc41.appspot.com");
        StorageReference[] images = new StorageReference[maxImages];
        for (int i = 0; i < maxImages; i++)
        {
            images[i] = storageReference.Child("/Images/image_" + i + ".jpg");
            string localFile = Application.persistentDataPath + "/Upload/image_" + i + ".jpg";
            if (!Directory.Exists(Application.persistentDataPath + "/Upload"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Upload");
            }
            images[i].GetFileAsync(localFile).ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted || !task.IsCanceled)
                {
                    downloadCounter++;
                    message.text = "files downloaded "+downloadCounter+"/7";
                }
            });
        }
    }
    private void Update()
    {
        if (downloadCounter == 7)
        {
            downloadCounter = -1;
            JoinLobby();
        }
    }
    public void JoinLobby()
    {
        message.text = "Joining Lobby ...";
        PhotonNetwork.ConnectUsingSettings();
    }    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}