using Fasterflect;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace EncyclopediaFix
{
    public class SubModule : MBSubModuleBase
    {
        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            HeroStringIdManager.SyncMBCharacterStringIdToHeroStringIdManager();
            HeroStringIdManager.LogAllStringIdofManager();

            MBObjectManager.Instance.TrySetFieldValue("_lastGeneratedId",
                HeroStringIdManager.GenerateNonDuplicateStringIdNum());
            // GrowthDebug.LogInfo(
            //     $"the last generatedId is {MBObjectManager.Instance.TryGetFieldValue("_lastGeneratedId")}");
        }
    }
}