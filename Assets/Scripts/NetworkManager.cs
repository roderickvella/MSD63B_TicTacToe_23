using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
     
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
    }

    public void NotifySelectBoardPiece(GameObject gameObject)
    {
        if((int)GetComponent<GameManager>().currentActivePlayer.id == 
            PhotonNetwork.LocalPlayer.ActorNumber)
        {
            photonView.RPC("RPC_NotifySelectedBoardPiece", RpcTarget.All, gameObject.name);

        }
     
    }

    [PunRPC]
    public void RPC_NotifySelectedBoardPiece(string gameObjectName)
    {
        print("received message:" + gameObjectName);
        GetComponent<GameManager>().SelectBoardPiece(GameObject.Find(gameObjectName));
    }



}
