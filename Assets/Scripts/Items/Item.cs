using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Items
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        Quest,
        Misc
    }

    public enum StatBase
    {
        None,
        Guns,
        Tech,
        Strength
    }
    
    [CreateAssetMenu(fileName = "Item", menuName = "Scrapper/Item", order = 0)]
    public class Item : ScriptableObject
    {
        public string itemName;
        public string itemID = Guid.NewGuid().ToString();
        public ItemType ItemType = ItemType.Misc;
        public StatBase StatBase = StatBase.None;
        public string itemCombatAnim = "";

        public int dam;
        public int apCost;
        public int dt;
        public float maxReach;
        public int[] statChanges = new int[8];
    }
}
