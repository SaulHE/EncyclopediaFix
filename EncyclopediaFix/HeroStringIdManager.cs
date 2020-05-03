using System.Collections.Generic;
using Fasterflect;
using TaleWorlds.CampaignSystem;

namespace EncyclopediaFix
{
    public class HeroStringIdManager
    {
        private static Dictionary<string, CharacterObject> _characterObjectStringIdDict;
        private static HeroStringIdManager _instance;

        public static Dictionary<string, CharacterObject> CharacterObjectStringIdDict
        {
            get
            {
                if (_characterObjectStringIdDict == null)
                {
                    _characterObjectStringIdDict = new Dictionary<string, CharacterObject>();
                }

                return _characterObjectStringIdDict;
            }
            private set => _characterObjectStringIdDict = value;
        }

        public static HeroStringIdManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HeroStringIdManager();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        private HeroStringIdManager()
        {
            _characterObjectStringIdDict ??= new Dictionary<string, CharacterObject>();
        }

        public static int SyncMBCharacterStringIdToHeroStringIdManager()
        {
            Dictionary<string, CharacterObject> idDict = CharacterObjectStringIdDict;
            int SumOfFailedAddCount = 0;
            foreach (CharacterObject characterObject in CharacterObject.All)
            {
                if (characterObject.StringId.Contains("CharacterObject"))
                {
                    if (idDict.ContainsKey(characterObject.StringId))
                    {
                        SumOfFailedAddCount++;
                        continue;
                    }

                    idDict.Add(characterObject.StringId, characterObject);
                }
            }
#if DEBUG_MODE_G
                    GrowthDebug.LogLine($"Sync finished, the fail result is {SumOfFailedAddCount}");
#endif
            return SumOfFailedAddCount;
        }

        public static int GetMBCharacterStringIdNumberSum()
        {
            return CharacterObjectStringIdDict.Count;
        }

        public static bool IsIdDuplicate(CharacterObject charaObj)
        {
            if (GetMBCharacterStringIdNumberSum() == 0)
            {
                SyncMBCharacterStringIdToHeroStringIdManager();
            }

            if (CharacterObjectStringIdDict.ContainsKey(charaObj.StringId))
            {
                return false;
            }

            return true;
        }

        public static bool AddCharacterStringIdToIdManager(CharacterObject charaObj)
        {
            if (!charaObj.StringId.Contains("CharacterObject"))
            {
                // GrowthDebug.LogInfo($"The hero {charaObj.Name} you add right now doesn't contain 'CharacterObject'");
                return false;
            }

            if (CharacterObjectStringIdDict.ContainsKey(charaObj.StringId))
            {
                return false;
            }

            CharacterObjectStringIdDict.Add(charaObj.StringId, charaObj);
            return true;
        }

        public static int GetMaxStringIdAsNumOfCurrentManagerRecord()
        {
            if (GetMBCharacterStringIdNumberSum() == 0)
            {
                int failNum = SyncMBCharacterStringIdToHeroStringIdManager();
            }

            int tempMax = -1;
            foreach (KeyValuePair<string, CharacterObject> keyValuePair in CharacterObjectStringIdDict)
            {
                int currIdNum = Utils.MBStringIdToInt(keyValuePair.Key);
                if (currIdNum > tempMax)
                {
                    tempMax = currIdNum;
                }
            }

            return tempMax;
        }

        public static string GenerateNonDuplicateStringId()
        {
            string prefix = "TaleWorlds.CampaignSystem.CharacterObject";
            if (GetMBCharacterStringIdNumberSum() == 0)
            {
                SyncMBCharacterStringIdToHeroStringIdManager();
            }

            int currNonDuplicateIdNum = GetMaxStringIdAsNumOfCurrentManagerRecord() + 1;
            return prefix + currNonDuplicateIdNum;
        }

        public static int GenerateNonDuplicateStringIdNum()
        {
            if (GetMBCharacterStringIdNumberSum() == 0)
            {
                SyncMBCharacterStringIdToHeroStringIdManager();
            }

            int currNonDuplicateIdNum = GetMaxStringIdAsNumOfCurrentManagerRecord() + 1;
            return currNonDuplicateIdNum;
        }

        public static void LogAllStringIdofManager()
        {
            if (GetMBCharacterStringIdNumberSum() == 0)
            {
                SyncMBCharacterStringIdToHeroStringIdManager();
            }

            // foreach (var kvp in CharacterObjectStringIdDict)
            // {
            //     GrowthDebug.LogLine($"{kvp.Value.Name}'s string id is {kvp.Key}");
            //     GrowthDebug.LogLine($"{kvp.Value.Name}'s original string id is {kvp.Value.OriginCharacterStringId}");
            // }
        }

        public static bool ChangeCharacterStringIdToNonDuplicate(CharacterObject charaObj, out string newId)
        {
            string newStringId = GenerateNonDuplicateStringId();

            charaObj.TrySetFieldValue("_originCharacterStringId", newStringId);
            charaObj.TrySetFieldValue("StringId", newStringId);

            if ((string) charaObj.TryGetFieldValue("_originCharacterStringId") == newStringId &&
                (string) charaObj.TryGetFieldValue("StringId") == newStringId)
            {
                newId = newStringId;
                return true;
            }

            newId = "";
            return false;
        }
    }
}