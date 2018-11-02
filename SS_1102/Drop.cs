using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;

public class Drop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            Drag.draggingItem.transform.SetParent(this.transform);
            Drag.draggingItem.GetComponent<Drag>().inSlot = true;
            Item _item = Drag.draggingItem
                .GetComponent<ItemInfo>().itemData;
            GameManager.instance.AddItem(_item);
        }        
    }
}
