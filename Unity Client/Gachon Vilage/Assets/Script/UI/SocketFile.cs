using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkLibrary.File;
public class SocketFile : MonoBehaviour {
    public static Dictionary<NetworkFile, SocketFile> Items = new Dictionary<NetworkFile, SocketFile>();
    public UILabel state;
    public UISlider Progressbar;
    public UILabel file_name;
    public NetworkFile file;
    public void Set(NetworkFile file)
    {
        this.file = file;
        Items.Add(file, this);
        if (file.upload)
        {
            file_name.text = file.FileName;
            state.text = "업로드중";
        }
        else
        {
            string[] temp = file.Path.Split('\\');
            file_name.text = temp[temp.Length - 1];
            state.text = "다운로드중";
        }
    }
	void OnDestory()
    {
        Items.Remove(file);
    }
	// Update is called once per frame
	void Update () {
        if (file != null)
            Progressbar.value = (float)((double)file.FinishByte / (double)file.FileSize);
	}
    public static void NewFile(NetworkFile file)
    {
        file.Start += delegate (NetworkFile thisfile)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(delegate () {
                SocketFile newobject = Instantiate(Preset.objects.SocketFile, Preset.objects.SocketFileGrid.transform);
                newobject.Set(thisfile);
                Preset.objects.SocketFileGrid.repositionNow = true;
            });
        };
        file.End += delegate (NetworkFile thisfile)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(delegate () {
                DestroyImmediate(SocketFile.Items[thisfile].gameObject);
                Preset.objects.SocketFileGrid.repositionNow = true;
            });
        };
    }
}
