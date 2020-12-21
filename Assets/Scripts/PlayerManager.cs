using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Photon.Pun;

namespace com.DefaultCompany.PhotonTutorial
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else{
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }
        #endregion

        #region Public Fields

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        private GameObject playerUiPrefab;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The current Health of our player")]
        public float Health = 1f;
        #endregion

        #region Private Fields
        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;
        bool IsFiring;

        private GameObject _uiGo;
        #endregion

        #region MonoBefaviour CallBacks

        #if !UNITY_5_4_OR_NEWER
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
        #endif

        void CalledOnLevelWasLoaded(int level)
        {
            if(level==0)
                return;

            Debug.Log("Instantiate playerUiPrefab on CalledOnLevelWasLoaded");
            _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            Debug.Log("SendMessage!!!!!!!!!!!!!!!!");

            if(this == null)
                return;

            if(!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

        }

        void OnTriggerEnter(Collider other)
        {
            if(!photonView.IsMine)
            {
                return;
            }
            if(!other.name.Contains("Beams"))
            {
                return;
            }
            Health -= 0.1f;
        }

        void OnTriggerStay(Collider other)
        {
            if(!photonView.IsMine)
            {
                return;
            }

            if(!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f*Time.deltaTime;
        }

        void Awake()
        {

            Debug.Log("Awake on PlayerManager!!!!!!!!!!!!!!!!!!");
            if(beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else{
                beams.SetActive(false);
            }
            
            if(photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if(_cameraWork != null)
            {
                if(photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else{
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }
            #if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
            {
                this.CalledOnLevelWasLoaded(scene.buildIndex);
            };

            if(playerUiPrefab != null)
            {
                Debug.Log("_uiGo = " + _uiGo);
                //if(_uiGo == null)
                {
                    _uiGo = Instantiate(playerUiPrefab);
                    Debug.Log("Instantiate _uiGo!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Debug.Log("_uiGo = " + _uiGo);
                }

                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                Debug.Log("Instantiate playUiPrefab!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
            else{
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

            #endif
        }

        // Update is called once per frame
        void Update()
        {
            if(photonView.IsMine)
                ProcessInputs();

            if(Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }

            if(beams != null && IsFiring != beams.activeInHierarchy)
            {
                beams.SetActive(IsFiring);
            }
        
        }

        #endregion

        #region Custom
        void ProcessInputs()
        {
            if(Input.GetButtonDown("Fire1"))
            {
                if(!IsFiring)
                {
                    IsFiring = true;
                }
            }
            if(Input.GetButtonUp("Fire1"))
            {
                if(IsFiring)
                {
                    IsFiring = false;
                }
            }
        }

        #endregion
    }

}