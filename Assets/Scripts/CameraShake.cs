using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    public IEnumerator camShake(float dur, float mag)
    {
        Vector3 camOrig = transform.localPosition;
        float timeElapsed = 0f;
        while (timeElapsed < dur)
        {
            float camx = Random.Range(-1f, 1f) * mag;
            float camy = Random.Range(-1f, 1f) * mag;
            transform.localPosition = camOrig + new Vector3(camx, camy, 0);

            timeElapsed += Time.deltaTime;
            yield return null;

        }
        transform.localPosition = camOrig;
    }

}
