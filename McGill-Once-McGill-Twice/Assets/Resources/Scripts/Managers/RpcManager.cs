﻿using System;
using UnityEngine;

[RequireComponent (typeof (PhotonView))]
public class RpcManager : PhotonSingleton<RpcManager> {

    public static void SendRpcToMaster(PhotonView photonView, string methodName, PhotonTargets target, params object[] parameters)
    {
        Instance.photonView.RPC("PerformRpcOnMasterClient", PhotonTargets.MasterClient, photonView, methodName, (byte)target, parameters);
    }
    
    public static void SendRpcToMaster(PhotonView photonView, string methodName, PhotonPlayer targetPlayer, params object[] parameters)
    {
        Instance.photonView.RPC("PerformRpcOnMasterClient", PhotonTargets.MasterClient, photonView, methodName, targetPlayer, parameters);
    }
    
    public static void SendRpcSecureToMaster(PhotonView photonView, string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
    {
        Instance.photonView.RPC("PerformRpcSecureOnMasterClient", PhotonTargets.MasterClient, photonView, methodName, (byte)target, encrypt, parameters);
    }
    
    public static void SendRpcSecureToMaster(PhotonView photonView, string methodName, PhotonPlayer targetPlayer, bool encrypt, params object[] parameters)
    {
        Instance.photonView.RPC("PerformRpcSecureOnMasterClient", PhotonTargets.MasterClient, photonView, methodName, targetPlayer, encrypt, parameters);
    }
    
    [PunRPC]
    public void PerformRpcOnMasterClient(PhotonView photonView, string methodName, byte target, params object[] parameters)
    {
        PhotonTargets realTarget = (PhotonTargets)target;
        if ((PhotonTargets)realTarget == PhotonTargets.Others || realTarget == PhotonTargets.OthersBuffered)
        {
            Debug.LogError("Cannot exclude the requesting player from the RPC targets. Is your PhotonTargets set to Others?");
            throw new NotImplementedException();
        }
        
        photonView.RPC(methodName, realTarget, parameters);
    }
    
    [PunRPC]
    public void PerformRpcOnMasterClient(PhotonView photonView, string methodName, PhotonPlayer targetPlayer, params object[] parameters)
    {
        photonView.RPC(methodName, targetPlayer, parameters);
    }
    
    [PunRPC]
    public void PerformRpcSecureOnMasterClient(PhotonView photonView, string methodName, byte target, bool encrypt, params object[] parameters)
    {
        PhotonTargets realTarget = (PhotonTargets)target;
        if (realTarget == PhotonTargets.Others || realTarget == PhotonTargets.OthersBuffered)
        {
            Debug.LogError("Cannot exclude the requesting player from the RPC targets. Is your PhotonTargets set to Others?");
            throw new NotImplementedException();
        }
        
        photonView.RpcSecure(methodName, realTarget, encrypt, parameters);
    }
    
    [PunRPC]
    public void PerformRpcSecureOnMasterClient(PhotonView photonView, string methodName, PhotonPlayer targetPlayer, bool encrypt, params object[] parameters)
    {
        photonView.RpcSecure(methodName, targetPlayer, encrypt, parameters);
    }
}