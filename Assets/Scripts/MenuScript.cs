using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Net
{
    public class MenuScript : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        private Button _createButton;
        [SerializeField]
        private Button _joinButton;
        [SerializeField]
        private Button _quitButton;

        private void Start()
        {
            _createButton.onClick.AddListener(CreateGame);
            _joinButton.onClick.AddListener(JoinGame);
            _quitButton.onClick.AddListener(QuitGame);

#if UNITY_EDITOR
            PhotonNetwork.NickName = "1";
            
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            PhotonNetwork.NickName = "2";        
#endif
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "0.0.1";
            PhotonNetwork.ConnectUsingSettings();
        }
        public override void OnConnectedToMaster()
        {
           // Debugger.Log("Ready To Connecting"); // ошибка при выходе из комнаты
        }
        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("Game");
        }
        private void CreateGame()
        {
            PhotonNetwork.CreateRoom(null,new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        }
        private void JoinGame()
        {
            PhotonNetwork.JoinRandomRoom();
        }
        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Application.Quit();        
#endif
        }
    }
}
