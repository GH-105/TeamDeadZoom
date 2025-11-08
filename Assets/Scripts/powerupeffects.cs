using UnityEngine;

public class powerupeffects : MonoBehaviour
{
    [SerializeField] int speedBoost;
    [SerializeField] int jumpCIncrease;
    [SerializeField] int damageBoostFlat;
    [SerializeField] float damageBoostMult;
    [SerializeField] int distBoost;
    [SerializeField] float rateBoost;
    [SerializeField] int ammoUp;
    [SerializeField] int HealthRecov;

    public enum PowerUpType
    {
        FlatDamage,
        DamageMultiplier,
        FireRate,
        Ammo,
        Range,
        Speed,
        Jump,
        HpRecovery
    }

    public PowerUpType type;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent <playerController> ();
            if (player != null)
            {
                int gunIndex = player.gunListPos;
                switch(type)
                {
                    case PowerUpType.FlatDamage:
                        PowerUpManager.Instance.ApplyFlatDamage(gunIndex, damageBoostFlat);
                        break;
                    case PowerUpType.DamageMultiplier:
                        PowerUpManager.Instance.ApplyDamageMultiplier(gunIndex, 1 + damageBoostMult);
                        break;
                    case PowerUpType.FireRate:
                        PowerUpManager.Instance.ApplyRateMultiplier(gunIndex, 1 + rateBoost);
                        break;
                    case PowerUpType.Ammo:
                        PowerUpManager.Instance.ApplyAmmoBonus(gunIndex, ammoUp);
                        break;
                    case PowerUpType.Range:
                        PowerUpManager.Instance.ApplyRangeBonus(gunIndex, distBoost);
                        break;
                    case PowerUpType.Speed:
                        PowerUpManager.Instance.ApplySpeedBonus(speedBoost);
                        break;
                    case PowerUpType.Jump:
                        PowerUpManager.Instance.ApplyJumpInc(jumpCIncrease);
                        break;
                    case PowerUpType.HpRecovery:
                        PowerUpManager.Instance.HpRecovery(HealthRecov);
                        break;

                }

                
                Destroy(gameObject);
            }
        }
    }
}
