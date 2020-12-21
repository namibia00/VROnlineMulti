using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.DefaultCompany.PhotonTutorial
{
    public class PlayerUI : MonoBehaviour
    {
        #region Public Fileds

        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f,30f,0f);

        #endregion

        #region Private Fields
        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;
        private PlayerManager target;

        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private Text playerNameText;

        [Tooltip("UI Slider to display Player's Health")]
        [SerializeField]
        private Slider playerHealthSlider;
        #endregion
        
        #region MonoBehaviour Callbacks

        void OnDestroy()
        {
            Debug.Log("OnDestroy!!!!!!!!!!!!!!!!!");
        }

        void LateUpdate()
        {
            if(targetRenderer != null)
            {
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            if(targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }


        void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
            Debug.Log("Awake on PlayerUI!!!!!!!!!!!!!!!!!");
        }

        void Update()
        {
            if(target == null)
            {
                Destroy(this.gameObject);
                Debug.Log("Destroyed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return;
            }

            if(playerHealthSlider != null)
            {
                //Debug.Log("target = " + target);
                //Debug.Log("target.Health = " + target.Health);
                playerHealthSlider.value = target.Health;
            }


        }

        #endregion

        #region Public methods

        public void SetTarget(PlayerManager _target)
        {
            Debug.Log("SetTarget");

            if(_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            target = _target;
            if(playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }

            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            
            if(characterController != null)
            {
                characterControllerHeight = characterController.height;
            }
        }

        #endregion

    }

}