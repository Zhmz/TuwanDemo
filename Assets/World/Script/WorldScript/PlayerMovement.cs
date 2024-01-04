using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Object;
using GameFramework.Event;
using Tuwan.Const;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    public float turnSpeed = 20f;
    Animator m_Animator;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    Rigidbody m_Rigidbody;

    public bool isDancing = false;

    private Rocker m_Rocker;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = this.GetComponent<Animator>();
        m_Rigidbody = this.GetComponent<Rigidbody>();

        m_Rocker = FindObjectsOfType<Rocker>().FirstOrDefault();
        isDancing = false;

        m_Animator.SetBool("IsDancing", isDancing);


        // m_Animator.Rebind();
    }

    private void OnEnable()
    {
        EventCenter.inst.AddEventListener((int)UIEventTag.EVENT_UI_FROM_WORLD_ENTER_LOBBY, OnFromWorldEnterLobby);
    }

    private void OnDisable()
    {
        EventCenter.inst.RemoveEventListener((int)UIEventTag.EVENT_UI_FROM_WORLD_ENTER_LOBBY, OnFromWorldEnterLobby);
    }

    void OnFromWorldEnterLobby()
    {
        m_Animator.SetFloat("Speed", 0);
        m_Rocker.SetRockerPos(0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (m_Rocker != null)
        {
            horizontal += m_Rocker.MoveDirection.x;
            vertical += m_Rocker.MoveDirection.y;

            if (Mathf.Approximately(m_Rocker.MoveDirection.x, 0f) && Mathf.Approximately(m_Rocker.MoveDirection.y, 0f))
            {
                m_Rocker.SetRockerPos(horizontal, vertical);
            }
        }


        //m_Movement.Set(horizontal, 0f, vertical);
        //m_Movement.Normalize();

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        m_Movement = (cameraForward * vertical + cameraRight * horizontal).normalized;

        m_Movement.y = 0;


        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;



        if (isWalking && !this.isDancing)
        {
            m_Animator.SetFloat("Speed", 3);
        }
        else
        {
            m_Animator.SetFloat("Speed", 0);
        }


        if (Input.GetKey(KeyCode.T))
        {
            this.isDancing = false;

        }
        else if (Input.GetKey(KeyCode.R))
        {
            this.isDancing = true;

        }

        m_Animator.SetBool("IsDancing", this.isDancing);


        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);

        m_Rotation = Quaternion.LookRotation(desiredForward);



    }

    public void ChangeDance()
    {
        this.isDancing = !this.isDancing;
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MoveRotation(m_Rotation);

        if (this.isDancing)
        {
            m_Animator.ApplyBuiltinRootMotion();
        }
        else
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        }



    }
}
