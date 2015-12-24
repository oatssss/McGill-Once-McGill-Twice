// ----------------------------------------------------------------------------
// <copyright file="CustomTypes.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 1587
/// \file
/// <summary>Sets up support for Unity-specific types. Can be a blueprint how to register your own Custom Types for sending.</summary>
#pragma warning restore 1587


using System.IO;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using System;
using UnityEngine;



/// <summary>
/// Internally used class, containing de/serialization methods for various Unity-specific classes.
/// Adding those to the Photon serialization protocol allows you to send them in events, etc.
/// </summary>
internal static class CustomTypes
{
    /// <summary>Register</summary>
    internal static void Register()
    {
        PhotonPeer.RegisterType(typeof(Vector2), (byte)'W', SerializeVector2, DeserializeVector2);
        PhotonPeer.RegisterType(typeof(Vector3), (byte)'V', SerializeVector3, DeserializeVector3);
        PhotonPeer.RegisterType(typeof(Quaternion), (byte)'Q', SerializeQuaternion, DeserializeQuaternion);
        PhotonPeer.RegisterType(typeof(PhotonPlayer), (byte)'P', SerializePhotonPlayer, DeserializePhotonPlayer);
        PhotonPeer.RegisterType(typeof(PhotonView), (byte)'A', SerializePhotonView, DeserializePhotonView);
        PhotonPeer.RegisterType(typeof(MinigameTeam), (byte)'T', SerializeMinigameTeam, DeserializeMinigameTeam);
    }


    #region Custom De/Serializer Methods


    public static readonly byte[] memVector3 = new byte[3 * 4];
    private static short SerializeVector3(MemoryStream outStream, object customobject)
    {
        Vector3 vo = (Vector3)customobject;

        int index = 0;
        lock (memVector3)
        {
            byte[] bytes = memVector3;
            Protocol.Serialize(vo.x, bytes, ref index);
            Protocol.Serialize(vo.y, bytes, ref index);
            Protocol.Serialize(vo.z, bytes, ref index);
            outStream.Write(bytes, 0, 3 * 4);
        }

        return 3 * 4;
    }

    private static object DeserializeVector3(MemoryStream inStream, short length)
    {
        Vector3 vo = new Vector3();
        lock (memVector3)
        {
            inStream.Read(memVector3, 0, 3 * 4);
            int index = 0;
            Protocol.Deserialize(out vo.x, memVector3, ref index);
            Protocol.Deserialize(out vo.y, memVector3, ref index);
            Protocol.Deserialize(out vo.z, memVector3, ref index);
        }

        return vo;
    }


    public static readonly byte[] memVector2 = new byte[2 * 4];
    private static short SerializeVector2(MemoryStream outStream, object customobject)
    {
        Vector2 vo = (Vector2)customobject;
        lock (memVector2)
        {
            byte[] bytes = memVector2;
            int index = 0;
            Protocol.Serialize(vo.x, bytes, ref index);
            Protocol.Serialize(vo.y, bytes, ref index);
            outStream.Write(bytes, 0, 2 * 4);
        }

        return 2 * 4;
    }

    private static object DeserializeVector2(MemoryStream inStream, short length)
    {
        Vector2 vo = new Vector2();
        lock (memVector2)
        {
            inStream.Read(memVector2, 0, 2 * 4);
            int index = 0;
            Protocol.Deserialize(out vo.x, memVector2, ref index);
            Protocol.Deserialize(out vo.y, memVector2, ref index);
        }

        return vo;
    }


    public static readonly byte[] memQuarternion = new byte[4 * 4];
    private static short SerializeQuaternion(MemoryStream outStream, object customobject)
    {
        Quaternion o = (Quaternion)customobject;

        lock (memQuarternion)
        {
            byte[] bytes = memQuarternion;
            int index = 0;
            Protocol.Serialize(o.w, bytes, ref index);
            Protocol.Serialize(o.x, bytes, ref index);
            Protocol.Serialize(o.y, bytes, ref index);
            Protocol.Serialize(o.z, bytes, ref index);
            outStream.Write(bytes, 0, 4 * 4);
        }

        return 4 * 4;
    }

    private static object DeserializeQuaternion(MemoryStream inStream, short length)
    {
        Quaternion o = new Quaternion();

        lock (memQuarternion)
        {
            inStream.Read(memQuarternion, 0, 4 * 4);
            int index = 0;
            Protocol.Deserialize(out o.w, memQuarternion, ref index);
            Protocol.Deserialize(out o.x, memQuarternion, ref index);
            Protocol.Deserialize(out o.y, memQuarternion, ref index);
            Protocol.Deserialize(out o.z, memQuarternion, ref index);
        }

        return o;
    }

    public static readonly byte[] memPlayer = new byte[4];
    private static short SerializePhotonPlayer(MemoryStream outStream, object customobject)
    {
        int ID = ((PhotonPlayer)customobject).ID;

        lock (memPlayer)
        {
            byte[] bytes = memPlayer;
            int off = 0;
            Protocol.Serialize(ID, bytes, ref off);
            outStream.Write(bytes, 0, 4);
            return 4;
        }
    }

    private static object DeserializePhotonPlayer(MemoryStream inStream, short length)
    {
        int ID;
        lock (memPlayer)
        {
            inStream.Read(memPlayer, 0, length);
            int off = 0;
            Protocol.Deserialize(out ID, memPlayer, ref off);
        }

        if (PhotonNetwork.networkingPeer.mActors.ContainsKey(ID))
        {
            return PhotonNetwork.networkingPeer.mActors[ID];
        }
        else
        {
            return null;
        }
    }
    
    // BEGIN CUSTOM SERIALIZERS
    
    private static byte[] SerializePhotonView(object customobject)
    {
        PhotonView view = (PhotonView)customobject;
        
        byte[] bytes = new byte[4];
        int index = 0;
        Protocol.Serialize(view.viewID, bytes, ref index);
        return bytes;
    }
    
    private static object DeserializePhotonView(byte[] bytes)
    {
        int viewID;
        int index = 0;
        Protocol.Deserialize(out viewID, bytes, ref index);
        return PhotonView.Find(viewID);
    }
    
    private static byte[] SerializeMinigameTeam(object customobject)
    {
        MinigameTeam team = (MinigameTeam)customobject;
        
        int byteSize = 0;
        byteSize += sizeof(int); // sizeof(int) bytes for the team ID
        byteSize += sizeof(int); // sizeof(int) bytes for the score
        byteSize += sizeof(int); // sizeof(int) bytes to hold maximum size
        byteSize += team.MaxSize * sizeof(short); // sizeof(short) bytes per player to indicate whether null or an actual value (short is 0 or 1)
        byteSize += team.MaxSize * sizeof(int); // sizeof(int) bytes per player ID
        byte[] bytes = new byte[byteSize];
        
        int index = 0;
        Protocol.Serialize(team.TeamID, bytes, ref index);
        Protocol.Serialize(team.Score, bytes, ref index);
        Protocol.Serialize(team.MaxSize, bytes, ref index);
        
        foreach (PhotonPlayer player in team)
        {
            if (player != null)
            {
                int indexOfID = index + (team.MaxSize * sizeof(short));
                Protocol.Serialize((short)1, bytes, ref index);
                Protocol.Serialize(player.ID, bytes, ref indexOfID);
            }
            else
                { Protocol.Serialize((short)0, bytes, ref index); }
        }
        
        return bytes;
    }
    
    private static object DeserializeMinigameTeam(byte[] bytes)
    {        
        int index = 0;
        int teamID;
        int score;
        int maxSize;
        
        // Deserialize teamID, score, and maxSize
        Protocol.Deserialize(out teamID, bytes, ref index);
        Protocol.Deserialize(out score, bytes, ref index);
        Protocol.Deserialize(out maxSize, bytes, ref index);
        
        // Make a team with maxSize player slots
        MinigameTeam team = new MinigameTeam(teamID, maxSize);
        team.Score = score; // Sync the score
        
        // Sync the players
        for (int i = 0; i < maxSize; i++)
        {
            short slotFilled;
            int indexOfID = index + (maxSize * sizeof(short));
            Protocol.Deserialize(out slotFilled, bytes, ref index);
            
            int ID;
            if (slotFilled == 1)
            {
                Protocol.Deserialize(out ID, bytes, ref indexOfID);
                PhotonPlayer player = PhotonPlayer.Find(ID);
                team.AddPlayer(player);
            }
        }
        
        return team;
    }

    #endregion
}
