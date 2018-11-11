using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    private UILabel label;
    private Collider col;
    public int No;
    private string _name;
    public string Name
    {
        get { return _name; }
        set { _name = value; if (label != null) { label.text = _name; } }
    }
    public string ID;
    void Start()
    {
        col = transform.GetComponent<Collider>();
        label = Instantiate(Preset.objects.NameUI).GetComponent<UILabel>();
        label.transform.parent = GameObject.FindWithTag("CCG").transform;
        label.text = Name;
        label.transform.localScale = new Vector3(1, 1, 1);
    }
    void Update()
    {
        Vector3 aa = col.bounds.center;
        aa.y += col.bounds.size.y / 2 + 0.5f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(aa);
        if (screenPos.z < 0 || screenPos.z > 100)
        {
            label.gameObject.SetActive(false);
        }
        else
        {
            if (screenPos.z > 80)
            {
                Color a = label.color;
                a.a = (100 - screenPos.z) * 5 / 100f;
                label.color = a;
            }

            screenPos.y += 15;
            label.transform.localPosition = screenPos;
            label.gameObject.SetActive(true);
        }

    }
    void OnDestroy()
    {
        Destroy(label.gameObject);
    }
}
