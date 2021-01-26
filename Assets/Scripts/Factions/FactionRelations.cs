using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapper.Factions
{
    [CreateAssetMenu(fileName = "Faction Relations", menuName = "Scrapper/Faction Relations", order = 0)]
    public class FactionRelations : ScriptableObject
    {
        public List<Faction> factions = new List<Faction>();

        public List<string> GetFactionNames(string idToExclude)
        {
            List<string> toReturn = new List<string>();

            for (int i = 0; i < factions.Count; i++)
            {
                if (factions[i].factionID != idToExclude)
                    toReturn.Add(factions[i].factionName);
            }
            
            return toReturn;
        }

        public string GetGUIDFromFactionTitle(string factionName)
        {
            for (int i = 0; i < factions.Count; i++)
            {
                if (factions[i].factionName.Equals(factionName))
                    return factions[i].factionID;
            }

            return factions[0].factionID;
        }

        public string GenerateGUID()
        {
            return Guid.NewGuid().ToString();
        }
    }

    [System.Serializable]
    public struct Faction
    {
        public string factionName;
        public string factionID;
        public int playerPartyOpinion;
        public bool folded;

        public List<FactionRelationComponent> relations;

        public Faction(string factionName, string factionID)
        {
            this.factionName = factionName;
            this.factionID = factionID;
            relations = new List<FactionRelationComponent>();
            folded = false;
            playerPartyOpinion = 0;
        }

        public Faction(string factionName, string factionID, List<FactionRelationComponent> relations)
        {
            this.factionName = factionName;
            this.factionID = factionID;
            this.relations = relations;
            folded = false;
            playerPartyOpinion = 0;
        }
    }

    [System.Serializable]
    public struct FactionRelationComponent
    {
        public string factionID;
        public int factionEditorIndex;
        public int factionOpinion;

        //This should take in a faction guid and a number from -100 to 100
        public FactionRelationComponent(string factionID, int factionOpinion)
        {
            this.factionID = factionID;
            this.factionOpinion = factionOpinion;
            factionEditorIndex = 0;
        }

        public string GetSetFactionID(string newFactionID = "")
        {
            return factionID;
            
            if (newFactionID.Length != 0)
                factionID = newFactionID;
        }

        public int GetSetFactionOpinion(int newOpinion = int.MinValue)
        {
            return factionOpinion;

            if (newOpinion != int.MinValue)
                factionOpinion = Mathf.Clamp(newOpinion, -100, 100);
        }
    }
}