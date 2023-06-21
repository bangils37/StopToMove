using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponController : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Transform _target)
    {
        target = _target;
        Vector3 targetPos = new Vector3(target.position.x,this.transform.position.y, target.position.z);
        LeanTween.move(this.gameObject, targetPos, 0.5f).setOnComplete(() => {
            GameObject.Destroy(this.gameObject);
        });
    }
}
