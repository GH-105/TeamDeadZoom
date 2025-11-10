using UnityEngine;

public class GroundIndicator : MonoBehaviour
{
    public void Show(Vector3 pos, float radius, float lifetime = 0.6f)
    {
        transform.position = pos + Vector3.up * 0.02f;
        transform.localScale = new Vector3(radius * 2f, 1f, radius * 2f);
        Destroy(gameObject, lifetime);
    }
}
