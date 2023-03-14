using System.Collections;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class PlayerMove : RealTimeMonoBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] Animator animator;
    protected Joystick controller;
    protected Button jumpBtn;

    protected Vector3 dirVec = new Vector3(0f, 0f, 0f);
    protected Vector3 prevDirVec = new Vector3(0f, 0f, 0f);

    protected float jumpPower = 7f;
    protected float originalSpeed = 3f;
    protected float maxSpeed = 8f;
    private float speed = 3f;
    protected int jumpCount = 2;
    protected bool slideBool = false;

    public float Speed { get => speed; set => speed = value; }
    public bool isDash = false;

    public void SpeedUp(float _incValue)
    {
        speed += _incValue;
        speed = Mathf.Clamp(speed, originalSpeed, maxSpeed);
    }

    protected void Move()
    {
        if (!slideBool)
        {
            if (realtimeView.IsMine)
            {
                prevDirVec = dirVec;
                dirVec = Vector3.forward * controller.Vertical + Vector3.right * controller.Horizontal;
                if (dirVec == Vector3.zero)
                {
                    if (dirVec != prevDirVec)
                    {
                        realtimeView.RPC("SetBoolAnimation", RpcTarget.All, realtimeView.OwnerId, "walk", false);
                    }
                    return;
                }
                else
                {
                    if (prevDirVec.magnitude <= 0.1f)
                    {
                        realtimeView.RPC("SetBoolAnimation", RpcTarget.All, realtimeView.OwnerId, "walk", true);
                    }
                    transform.rotation = Quaternion.LookRotation(dirVec);
                    if (isDash)
                        transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed * 2);
                    else
                        transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpBtnClick();
        }
    }

    protected void OnJumpBtnClick()
    {
        if (realtimeView.IsMine && !slideBool)
        {
            if (jumpCount == 2)
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                jumpCount--;
            }
            else if (jumpCount == 1)
            {
                rb.velocity = (Vector3.up * 0.6f + dirVec.normalized * 0.4f).normalized * (jumpPower * 0.50f);
                realtimeView.RPC("SetSlideAnimation", RpcTarget.All, realtimeView.OwnerId);
                jumpCount--;
            }
        }
    }

    [RealTimeRPC]
    public void SetBoolAnimation(ulong _ownerId, string _paramName, bool _value)
    {
        if (realtimeView.OwnerId == _ownerId)
        {
            animator.SetBool(_paramName, _value);
        }
    }

    [RealTimeRPC]
    public void SetSlideAnimation(ulong _ownerId)
    {
        if (realtimeView.OwnerId == _ownerId)
        {
            animator.SetTrigger("slide");
            StartCoroutine(SetSlideBool());
        }
    }

    protected IEnumerator SetSlideBool()
    {
        yield return new WaitForSeconds(0.5f);
        slideBool = true;
        yield return new WaitForSeconds(0.8f);
        slideBool = false;
    }
}
