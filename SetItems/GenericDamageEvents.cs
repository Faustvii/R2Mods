using RoR2;
using UnityEngine;

namespace Faust.SetItems
{
    public class GenericDamageEvents
    {
        public delegate void DamageAttackerVictimEventHandler(DamageInfo damageInfo, GenericCharacterInfo attackerInfo, GenericCharacterInfo victimInfo);
        public delegate void DamageReportEventHandler(DamageReport damageReport);

        public static event DamageAttackerVictimEventHandler BeforeTakeDamage;
        public static event DamageReportEventHandler OnTakeDamage;

        public class DamageEvents : MonoBehaviour, IOnIncomingDamageServerReceiver, IOnTakeDamageServerReceiver
        {
            public HealthComponent healthComponent;
            public CharacterBody victimBody;

            public void Start()
            {
                healthComponent = GetComponent<HealthComponent>();
                if (!healthComponent)
                {
                    Object.Destroy(this);
                    return;
                }
                victimBody = healthComponent.body;
            }

            public void OnIncomingDamageServer(DamageInfo damageInfo)
            {
                GenericCharacterInfo attackerInfo = new GenericCharacterInfo();
                if (damageInfo.attacker) attackerInfo = new GenericCharacterInfo(damageInfo.attacker.GetComponent<CharacterBody>());
                GenericCharacterInfo victimInfo = new GenericCharacterInfo(victimBody);
                if (BeforeTakeDamage != null) BeforeTakeDamage(damageInfo, attackerInfo, victimInfo);
            }

            public void OnTakeDamageServer(DamageReport damageReport)
            {
                if (victimBody && OnTakeDamage != null) OnTakeDamage(damageReport);
            }
        }
    }


    public struct GenericCharacterInfo
    {
        public GameObject gameObject;
        public CharacterBody body;
        public CharacterMaster master;
        public TeamComponent teamComponent;
        public HealthComponent healthComponent;
        public Inventory inventory;
        public TeamIndex teamIndex;
        public Vector3 aimOrigin;

        public GenericCharacterInfo(CharacterBody body)
        {
            this.body = body;
            gameObject = body ? body.gameObject : null;
            master = body ? body.master : null;
            teamComponent = body ? body.teamComponent : null;
            healthComponent = body ? body.healthComponent : null;
            inventory = master ? master.inventory : null;
            teamIndex = teamComponent ? teamComponent.teamIndex : TeamIndex.Neutral;
            aimOrigin = body ? body.aimOrigin : Random.insideUnitSphere.normalized;
        }
    }
}
