using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
class Preset : MonoBehaviour
{
    static Preset preset;
    public static Preset objects
    {
        get
        {
            if (preset == null) preset = GameObject.Find("Preset").GetComponent<Preset>();
            return preset;
        }
    }
    public GameObject NewGameObject;
    public GameObject NameUI;
    void Start()
    {
    }
}
