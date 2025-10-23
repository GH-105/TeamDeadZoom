using UnityEngine;

public class Iinteraction : MonoBehaviour
{
    [SerializeField] int speedBoost;
    [SerializeField] int jumpCIncrease;
    [SerializeField] int damageBoost;
    [SerializeField] int distBoost;
    [SerializeField] float rateBoost;
    [SerializeField] int ammoUp;

    public enum PowerUpType
    {
        FlatDamage,
        DamageMultiplier,
        FireRate,
        Ammo,
        Range,
        Speed,
        Jump
    }

    public PowerUpType type;
    public float value;

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
                        PowerUpManager.Instance.ApplyFlatDamage(gunIndex, Mathf.RoundToInt(value));
                        break;
                    case PowerUpType.DamageMultiplier:
                        PowerUpManager.Instance.ApplyDamageMultiplier(gunIndex, 1 + value);
                        break;
                    case PowerUpType.FireRate:
                        PowerUpManager.Instance.ApplyRateMultiplier(gunIndex, 1 + value);
                        break;
                    case PowerUpType.Ammo:
                        PowerUpManager.Instance.ApplyAmmoBonus(gunIndex, Mathf.RoundToInt(value));
                        break;
                    case PowerUpType.Range:
                        PowerUpManager.Instance.ApplyRateMultiplier(gunIndex, Mathf.RoundToInt(value));
                        break;
                    case PowerUpType.Speed:
                        player.Speed += (int)speedBoost;
                        break;
                    case PowerUpType.Jump:
                        player.JumpCountMax += (int)jumpCIncrease;
                        break;
                }
                
                Destroy(gameObject);
            }
        }
    }
}
