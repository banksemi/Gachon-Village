using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameUI : MonoBehaviour
{
    public GameObject GameObject;
    private UILabel label;
    public string name;
    Collider col;
    // Use this for initialization
    void Start () {
        label = Instantiate(GameObject).GetComponent<UILabel>();
        label.transform.parent = GameObject.FindWithTag("CCG").transform;
        label.text = name;
        label.transform.localScale = new Vector3(1, 1, 1);
        col = transform.GetComponent<Collider>();
    }
	
	// Update is called once per frame
	void Update () {
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
}
