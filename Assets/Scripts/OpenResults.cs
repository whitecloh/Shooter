using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OpenResults : MonoBehaviour
{
    [SerializeField]
    private Text _resultsTxt;
    [SerializeField]
    private GameObject _results;

    public void OpenResultsMenu(string str)
    {        
        _resultsTxt.text = str;
        _results.SetActive(true); 
        StartCoroutine(Leave());
    }
    private IEnumerator Leave()
    {
        yield return new WaitForSeconds(2f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.LeaveRoom();
    }
}
