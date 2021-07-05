using RoR2;

namespace Faust.SetItems.Extensions
{
    public static class CharacterBodyExtensions
    {
        public static void RemoveItemBehavior<T>(this CharacterBody body) where T : CharacterBody.ItemBehavior
        {
            body.AddItemBehavior<T>(0);
        }
    }
}
