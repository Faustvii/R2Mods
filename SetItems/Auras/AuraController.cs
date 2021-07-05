using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Faust.SetItems.Auras
{
    [RequireComponent(typeof(NetworkedBodyAttachment))]
    public class AuraController : NetworkBehaviour
    {
        public float baseAttackInterval = 0.25f;
        public float baseRadius = 16f;
        public float RadiusPer = 2f;
        public float DamageCoefficientPerTick = 2f;
        public float DamageCoefficientPerTickPer = 1f;
        public float ProcCoefficientPerTick = 0.2f;
        private float attackStopwatch;

        [SyncVar]
        private int finalIcicleCount;

        public int MaxIcicleCount => 6;

        public NetworkedBodyAttachment NetworkedBodyAttachment { get; private set; }
        public Transform auraEffectTransform;
        private float actualRadius;
        private CameraTargetParams cameraTargetParams;

        private void Awake()
        {
            auraEffectTransform = base.transform;
            NetworkedBodyAttachment = GetComponent<NetworkedBodyAttachment>();
            var identity = GetComponent<NetworkIdentity>();
            if (identity)
            {
                identity.localPlayerAuthority = true;
            }
            auraEffectTransform.SetParent(null);
        }

        private void FixedUpdate()
        {
            UpdateRadius();
            if (NetworkServer.active)
            {
                finalIcicleCount = MaxIcicleCount;
                ServerFixedUpdate();
            }
        }

        private void ServerFixedUpdate()
        {
            attackStopwatch += Time.fixedDeltaTime;
            if (NetworkedBodyAttachment.attachedBody && finalIcicleCount > 0 && attackStopwatch >= baseAttackInterval)
            {
                attackStopwatch = 0f;
                new BlastAttack
                {
                    attacker = NetworkedBodyAttachment.attachedBodyObject,
                    inflictor = gameObject,
                    teamIndex = NetworkedBodyAttachment.attachedBody.teamComponent.teamIndex,
                    position = transform.position,
                    procCoefficient = ProcCoefficientPerTick,
                    radius = actualRadius,
                    baseForce = 0f,
                    baseDamage = NetworkedBodyAttachment.attachedBody.damage * (DamageCoefficientPerTick + (DamageCoefficientPerTickPer * finalIcicleCount)),
                    bonusForce = Vector3.zero,
                    crit = false,
                    damageType = DamageType.IgniteOnHit,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageColorIndex = DamageColorIndex.Item
                }.Fire();
            }
        }

        private void UpdateRadius()
        {
            if (NetworkedBodyAttachment)
            {
                //var oldRadius = actualRadius;
                actualRadius = NetworkedBodyAttachment.attachedBody ? (NetworkedBodyAttachment.attachedBody.radius + baseRadius) : baseRadius;
                //actualRadius = NetworkedBodyAttachment.attachedBody ? (NetworkedBodyAttachment.attachedBody.radius + baseRadius + RadiusPer * finalIcicleCount) : 1f;
                //if(oldRadius != actualRadius)
                //{
                //    Log.LogInfo($"base radius is now {baseRadius + RadiusPer * finalIcicleCount}");
                //}
            }
        }

        private void UpdateVisuals()
        {
            var attachedBody = NetworkedBodyAttachment.attachedBody;
            if (attachedBody)
            {
                auraEffectTransform.position = NetworkedBodyAttachment.attachedBody.corePosition;
                auraEffectTransform.localScale = new Vector3(actualRadius, actualRadius, actualRadius);
                if (!cameraTargetParams)
                {
                    cameraTargetParams = attachedBody.GetComponent<CameraTargetParams>();
                    return;
                }
                cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
            }
        }

        private void LateUpdate()
        {
            UpdateVisuals();
        }

        public void OnDestroy()
        {
            if (auraEffectTransform)
            {
                Destroy(auraEffectTransform.gameObject);
                auraEffectTransform = null;
            }
            if (cameraTargetParams)
            {
                cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            }
        }
    }
}
