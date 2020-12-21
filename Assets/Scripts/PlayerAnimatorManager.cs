﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace com.DefaultCompany.PhotonTutorial
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields

        [SerializeField]
        private float directionDampTime = .25f;
        private Animator animator;

        #endregion

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            if(!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            if(!animator)
            {
                return;
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if(stateInfo.IsName("Base Layer.Run"))
            {
                if(Input.GetButtonDown("Jump"))
                {
                    animator.SetTrigger("Jump");
                }
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if(v<0)
            {
                v = 0;
            }
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        
        }
        #endregion
    }

}