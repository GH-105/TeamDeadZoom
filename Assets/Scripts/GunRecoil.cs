using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public float recoilX; 
    public float recoilY; 
    public float recoilZ; 
    public float kickbackZ; 
    public float snap; 
    public float resetSpeed; 

    Vector3 oriPos;
    Quaternion oriRot;
    Vector3 currentRecoilPos;
    Quaternion currentRecoilRot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       oriPos = transform.localPosition;
       oriRot = transform.localRotation;
       currentRecoilPos = oriPos;
       currentRecoilRot = oriRot;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, currentRecoilPos, snap * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, currentRecoilRot, snap * Time.deltaTime);
        currentRecoilPos = Vector3.Lerp(currentRecoilPos, oriPos, resetSpeed * Time.deltaTime);
        currentRecoilRot = Quaternion.Slerp(currentRecoilRot, oriRot, resetSpeed * Time.deltaTime);
    }
    public void ApplyRecoil()
    {
        float randomX = Random.Range(-recoilX, recoilX);
        float randomY = Random.Range(-recoilY, recoilY);
        float randomZ = Random.Range(-recoilZ, recoilZ);

        currentRecoilRot = Quaternion.Euler(oriRot.eulerAngles.x + randomX, oriRot.eulerAngles.y + randomY, oriRot.eulerAngles.z + randomZ);
        currentRecoilPos = oriPos + (transform.forward * -kickbackZ);
    }
}
