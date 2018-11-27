using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;
using NetworkLibrary;
using NetworkLibrary.File;
using Newtonsoft.Json.Linq;

public class FileDragAndDrop : MonoBehaviour
{
    // important to keep the instance alive while the hook is active.
    UnityDragAndDropHook hook;
    void OnEnable ()
    {
        // must be created on the main thread to get the right thread id.
        hook = new UnityDragAndDropHook();
        hook.InstallHook();
        hook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
        hook.UninstallHook();
    }
    public void Test()
    {
        List<string> list = new List<string>();
        list.Add("C:\\Users\\seung\\Pictures\\8079071366_6d2e01e6e5_k.jpg");
        OnFiles(list, new POINT());
    }
    void OnFiles(List<string> aFiles, POINT aPos)
    {
        foreach (string path in aFiles)
        {
            NClientFile file = new NClientFile(NetworkMain.server, path);
            file.Start += delegate (NetworkFile thisfile)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(delegate() {

                    NetworkMain.gameObjects[NetworkMain.myNo].ChatMessage("파일 전송 시작");
                });
            };
            file.Process += delegate (NetworkFile thisfile)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(delegate () {

                    NetworkMain.gameObjects[NetworkMain.myNo].ChatMessage("파일 전송중 " + thisfile.FinishByte);
                });
            };
            file.End += delegate (NetworkFile thisfile)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(delegate () {
                    NetworkMain.gameObjects[NetworkMain.myNo].ChatMessage("파일 전송 완료");
                });
            };
            NetworkMain.server.SendFile(new JObject(), file);
        }
        NetworkMain.gameObjects[NetworkMain.myNo].ChatMessage("Dropped " + aFiles.Count + " files at: " + aPos + "\n" +
            aFiles.Aggregate((a, b) => a + "\n" + b));
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.
        Debug.Log("Dropped "+aFiles.Count+" files at: " + aPos + "\n"+
            aFiles.Aggregate((a, b) => a + "\n" + b));
    }
}
