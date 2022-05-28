using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //プレイヤーを変数に格納
    public Transform Target;
 
    //回転させるスピード
    public float rotateSpeed = 3.0f;

    private Vector3 StartRelativePosition;
    private Vector3 StartRelativeForward;

 
    // Use this for initialization	
    void Start () {

        StartRelativePosition = Target.InverseTransformPoint(this.transform.position);
        StartRelativeForward = Target.InverseTransformDirection(this.transform.forward);
    }
    // Update is called once per frame
    void Update () {
        Vector3 playerPos = Target.position;

        if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow)){
            float v = Input.GetAxis("Vertical") * rotateSpeed;
            transform.RotateAround(playerPos, transform.right, v);
        }
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)){
            float h = Input.GetAxis("Horizontal") * rotateSpeed;
            transform.RotateAround(playerPos, Vector3.up, h);
        }

        if(Input.GetKey(KeyCode.W)){
            this.transform.position += this.transform.forward * Time.deltaTime * rotateSpeed;
        }
        if(Input.GetKey(KeyCode.S)){
            this.transform.position -= this.transform.forward * Time.deltaTime * rotateSpeed;
        }
        if(Input.GetKey(KeyCode.R)){
            Vector3 ResetWorldPosition = Target.TransformPoint(StartRelativePosition);
            this.transform.position = ResetWorldPosition;
            Vector3 ResetWorldForward = Target.TransformDirection(StartRelativeForward);
            this.transform.forward = ResetWorldForward;
        }
    }
}
