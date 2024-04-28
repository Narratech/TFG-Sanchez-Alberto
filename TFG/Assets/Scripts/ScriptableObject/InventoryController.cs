using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;
    public List<Item> items = new List<Item>();

    public Transform itemContent;
    public GameObject inventoryItem;

    public void Awake()
    {
        Instance = this;
    }

    public void Add(Item item)
    {
        items.Add(item);
    }

    public void Remove(Item item)
    {
        items.Remove(item);
    }

    public void ListItems()
    {
        foreach (Transform item in itemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in items)
        {
            GameObject obj = Instantiate(inventoryItem, itemContent);
            var itemName = obj.transform.Find("itemName").GetComponent<TMPro.TextMeshProUGUI>();
            var itemIcon= obj.transform.Find("itemIcon").GetComponent<Image>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;
        }
    }
}
