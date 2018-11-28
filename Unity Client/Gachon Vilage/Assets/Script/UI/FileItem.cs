using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
public class FileItem : MonoBehaviour {
    public int no;
    public String owner;
    public UILabel UI_title;
    public UILabel UI_size;
    public UILabel UI_date;
    void OnClick()
    {
        // 우클릭이면
        if (UICamera.currentTouchID == -2)
        {
            FileMenu file = Preset.objects.fileMenu;
            file.no = this.no;
            Vector3 p2 = Input.mousePosition;
            p2.y = -Screen.height + p2.y;
            file.transform.localPosition = p2;
            file.title.text = "From " + owner;
            file.gameObject.SetActive(true);
        }
    }
}
