using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    private List<Window> Windows = new List<Window>();
    public List<GameObject> Toggle;
    void Start()
    {
        Windows.Add(this);
        if (Toggle.Count > 0)
        {
            Toggle[0].SetActive(true);
            for (int i = 1; i < Toggle.Count; i++)
            {
                Toggle[i].SetActive(false);
            }
        }
    }
    public virtual void TabChange(string name)
    {
        for(int i = 0; i < Toggle.Count;i++)
        {
            if (Toggle[i].name == name) Toggle[i].SetActive(true);
            else Toggle[i].SetActive(false);
        }
    }
    public virtual void Close()
    {
        UIInput.selection = null;
        Input.imeCompositionMode = IMECompositionMode.Auto;
        gameObject.SetActive(false);
    }
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public bool isOpen()
    {
        return gameObject.activeSelf;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }
}