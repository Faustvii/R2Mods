using R2API;
using RoR2;
using UnityEngine.Networking;

namespace Faust.SetItems.Auras
{
    public class FireGodItemBehavior : CharacterBody.ItemBehavior
    {
        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            var hasItem = stack > 0;
            if (AuraController != hasItem)
            {
                if (hasItem)
                {
                    var aura = PrefabAPI.InstantiateClone(Assets.fireAuraCirclePrefab, "Aura");
                    aura.transform.SetParent(gameObject.transform);
                    AuraController = aura.GetComponent<AuraController>();
                    AuraController.NetworkedBodyAttachment.AttachToGameObjectAndSpawn(body.gameObject);
                    return;
                }
                else
                {
                    OnDisable();
                }
            }
        }

        private void OnDisable()
        {
            if (AuraController)
            {
                Destroy(AuraController);
                AuraController = null;
            }
        }

        private AuraController AuraController;
    }
}
