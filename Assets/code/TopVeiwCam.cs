using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopVeiwCam : MonoBehaviour
{
    public Transform targetTr;

    public float height = 10.0f;
    public float angle;
    public float vertic_add;
    public float move_scale = 0.2f;
    private Transform this_tr;
    private Rigidbody this_rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        this_tr = GetComponent<Transform>();
        this_rigidbody = GetComponent<Rigidbody>();
        targetTr = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
   
        this_tr.localEulerAngles = Vector3.right * angle;
        this_rigidbody.AddForce(Arrive(targetTr.position*move_scale  + (targetTr.forward * vertic_add) + (targetTr.up * height), 10));
    }

    public Vector3 Arrive(Vector3 targetPos, float MaxSpeed)
    {
        float distance = (targetPos - this.this_tr.position).magnitude;

        if (distance > 0)
        {
            const float DecelerationTweaker = 0.3f;
            float speed = distance / DecelerationTweaker;
            speed = Mathf.Min(speed, MaxSpeed);
            Vector3 desireVelocity = (targetPos - this.this_tr.position) / distance * speed;
            return (desireVelocity - this_rigidbody.velocity);
        }
        return new Vector3(0, 0);
    }
}
