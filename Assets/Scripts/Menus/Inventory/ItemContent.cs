﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menus
{
    public class ItemContent : MonoBehaviour
    {
        public Item item;

        public void SetUp(Item item)
        {
            GetComponent<Image>().sprite = item.icon;
            this.item = item;
        }

        public void TooltipShow(BaseEventData data)
        {
            Tooltip.Instance.Show(this);
        }

        public void TooltipHide(BaseEventData data)
        {
            StartCoroutine(Tooltip.Instance.Hide());
        }
    }
}