using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Entities
{
    public class Entity : MonoBehaviour
    {
        public string entityName;
        public string entityAltTitle;
        public int[] healthPts = new int[2]; // [0] = Current HP, [1] = Max HP
        public int[] actionPts = new int[2]; // [0] = Current AP, [1] = Max AP
        public int[] expPts    = new int[2]; // [0] = current XP, [1] = Current Level
        public Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"Guns",      0},
            {"Tech",      0},
            {"Strength",  0},
            {"Dexterity", 0},
            {"Endurance", 0},
            {"Charisma",  0},
            {"Luck",      0},
        };
        
        //
    }
}
