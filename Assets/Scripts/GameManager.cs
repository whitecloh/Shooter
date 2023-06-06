using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Net
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        private string _playerPrefabName;
        [SerializeField]
        private Transform _player1StartPosition;
        [SerializeField]
        private Transform _player2StartPosition;
        [SerializeField]
        private Camera _mainCamera;

        private PlayerController _player1;
        private PlayerController _player2;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            var GO = PhotonNetwork.Instantiate(_playerPrefabName + PhotonNetwork.NickName,new Vector3(),Quaternion.Euler(0,0,0));
            if (GO.name.Contains("1"))
            {
                GO.transform.SetParent(_player1StartPosition);
                GO.transform.localPosition = Vector3.zero;
                SetCameraView(GO.GetComponent<PlayerController>()._camera.transform);
            }
            else
            {
                GO.transform.SetParent(_player2StartPosition);
                GO.transform.localPosition = Vector3.zero;
                SetCameraView(GO.GetComponent<PlayerController>()._camera.transform);
            }

            PhotonPeer.RegisterType(typeof(PlayerData), 100, Debugger.SerializePlayerData, Debugger.DeserializePlayerData);            
        }
        public void AddPlayer(PlayerController player)
        {
            if (player.name.Contains("1"))
                _player1 = player;
            else
                _player2 = player;

            if(_player1!=null&&_player2!=null)
            {
                SetStartPlayersData(_player1);
                SetStartPlayersData(_player2);
            }
        }
        private void SetStartPlayersData(PlayerController player)
        {
            if (player.name.Contains("1"))
            {
                player.transform.SetParent(_player1StartPosition);
                player.transform.eulerAngles = new Vector3(0, 0, 0);
                player.Health = 100;
            }
            else
            {
                player.transform.SetParent(_player2StartPosition);
                player.transform.eulerAngles = new Vector3(0, 180, 0);
                player.Health = 100;
            }
            player.transform.localPosition = Vector3.zero;
        }
        private void SetCameraView(Transform playerCamera)
        {
                _mainCamera.transform.SetParent(playerCamera);
                _mainCamera.transform.localPosition = Vector3.zero;
                _mainCamera.transform.localEulerAngles = Vector3.zero;

        }
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
    }
}