using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollGrid : MonoBehaviour {
    public UIGrid Items;
    public UIScrollBar scrollBar;
    public int Offset;
    private int ItemHeight = 0;
	
    public void Reset()
    {
        while (Items.transform.childCount > 0)
        {
            DestroyImmediate(Items.GetChild(0).gameObject);
        }
        Items.repositionNow = true;
        scrollBar.value = 0;
    }

    public T AddItem<T>(T gameObject) where T : UnityEngine.Component
    {
        T newitem = Instantiate(gameObject,Items.transform);
        if (ItemHeight == 0)
        {
            GameObject gobject = newitem.gameObject;
            ItemHeight = gobject.GetComponent<UIWidget>().height + Offset;
            Items.cellHeight = ItemHeight;
        }
        return newitem;
    }
    public void RepositionNow()
    {
        Items.GetComponent<UIWidget>().height = Items.transform.childCount * ItemHeight;
        Items.repositionNow = true;
        scrollBar.value = 0;
    }
}
