using UnityEngine;

[System.Serializable]
public class DamageData
{
    public int damage;
    public Vector2 knockback;
    public GameObject source;

    public DamageData(int damage, Vector2 knockback, GameObject source)
        {
            this.damage = damage;
            this.knockback = knockback;
            this.source = source;
    }
}
