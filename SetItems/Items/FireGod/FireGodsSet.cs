using Faust.SetItems.Auras;
using Faust.SetItems.Extensions;
using RoR2;
using System.Collections.Generic;

namespace Faust.SetItems.Items.FireGod
{
    public class FireGodsSet : ItemBaseSet<FireGodsSet>
    {
        public override List<ItemBase> SetItems => new List<ItemBase>
        {
            FireGodsWand1.Instance,
            FireGodsWand2.Instance,
            FireGodsWand3.Instance,
            FireGodsWand4.Instance,
        };

        public override string SetName => "Fire God";

        public override void ApplyTwoPieceEffect(CharacterBody body)
        {
            base.ApplyTwoPieceEffect(body);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "You feel power surge through you as you don two of the four wands" });
            Log.LogInfo($"Applying two piece buff to {body.name}");
            body.AddBuff(RoR2Content.Buffs.Energized);
            Log.LogInfo($"Applied two piece buff to {body.name}");
        }

        public override void ApplyFourPieceEffect(CharacterBody body)
        {
            base.ApplyFourPieceEffect(body);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "You feel power surge through you as you don all four wands" });
            Log.LogInfo($"Applying four piece buff to {body.name}");
            body.AddBuff(RoR2Content.Buffs.NoCooldowns);
            body.AddItemBehavior<FireGodItemBehavior>(1);
            Log.LogInfo($"Applied four piece buff to {body.name}");
        }

        public override void RemoveTwoPieceEffect(CharacterBody body)
        {
            base.RemoveTwoPieceEffect(body);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "You feel weakened as you only have one of the four wands" });
            Log.LogInfo($"Removing two piece buff from {body.name}");
            body.RemoveBuff(RoR2Content.Buffs.Energized);
            Log.LogInfo($"Removed two piece buff from {body.name}");

        }

        public override void RemoveFourPieceEffect(CharacterBody body)
        {
            base.RemoveFourPieceEffect(body);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "You feel weakened as you loose the power of the four wands" });
            Log.LogInfo($"Removing four piece buff from {body.name}");
            body.RemoveItemBehavior<FireGodItemBehavior>();
            body.RemoveBuff(RoR2Content.Buffs.NoCooldowns);
            Log.LogInfo($"Removed four piece buff from {body.name}");
        }
    }
}
