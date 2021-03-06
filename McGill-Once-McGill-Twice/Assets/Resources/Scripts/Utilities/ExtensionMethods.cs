﻿using UnityEngine;
using System;
// using UnityEditor;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform) {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static Quaternion RotationDeltaTo(this Quaternion rotation, Quaternion targetRotation)
        {
            return Quaternion.Inverse(rotation) * targetRotation;
        }

        public static Quaternion Clamp(this Quaternion toClamp, Vector3 forward, float xDegrees, float yDegrees, float zDegrees)
        {
            Vector3 toClampRelative = Quaternion.Inverse(toClamp) * forward;

            //  The angle difference between 'forward' and 'toClamp' in world space along the YZ plane
            float angleDifferenceYZ = -Mathf.Atan2(toClampRelative.z, toClampRelative.y) * Mathf.Rad2Deg;
            float angleDifferenceXZ = -Mathf.Atan2(toClampRelative.x, toClampRelative.z) * Mathf.Rad2Deg;
            float angleDifferenceXY = -Mathf.Atan2(toClampRelative.y, toClampRelative.x) * Mathf.Rad2Deg;

            angleDifferenceYZ = Mathf.Clamp(angleDifferenceYZ, -xDegrees, xDegrees);
            angleDifferenceXZ = Mathf.Clamp(angleDifferenceXZ, -yDegrees, yDegrees);
            angleDifferenceXY = Mathf.Clamp(angleDifferenceXY, -zDegrees, zDegrees);

            if (angleDifferenceYZ < 0) angleDifferenceYZ += 360f;
            if (angleDifferenceXZ < 0) angleDifferenceXZ += 360f;
            if (angleDifferenceXY < 0) angleDifferenceXY += 360f;

            Quaternion relativeClampedRotation = Quaternion.Euler(0, angleDifferenceXZ, 0);
            Quaternion relativeForwardRotation = Quaternion.LookRotation(forward);

            return relativeForwardRotation * relativeClampedRotation;
        }

        public static Quaternion Clamp(this Quaternion toClamp, Vector3 forward, float degreeFreedom)
        {
            return toClamp.Clamp(forward, degreeFreedom, degreeFreedom, degreeFreedom);
        }

        public static Vector3 ClampMagnitude(this Vector3 toClamp, float minMagnitude, float maxMagnitude)
        {
            float magnitude = toClamp.magnitude;

            //  Take care with zero so we don't divide by it
            if (Mathf.Approximately(magnitude, 0f))
                { return toClamp.normalized * minMagnitude; }

            float clampedMagnitude = Mathf.Clamp(magnitude, minMagnitude, maxMagnitude);
            float scale = clampedMagnitude/magnitude;
            return toClamp * scale;
        }

        public static Vector3 ClampMagnitude01(this Vector3 toClamp)
        {
            return toClamp.ClampMagnitude(0f, 1f);
        }

        public static Vector3 ClampMaxMagnitude(this Vector3 toClamp, float maxMagnitude)
        {
            return toClamp.ClampMagnitude(0f, maxMagnitude);
        }

        //
        // RPC AS MASTER EXTENSIONS
        //
        public static void RpcAsMaster(this PhotonView photonView, string methodName, PhotonTargets target, params object[] parameters)
        {
            RpcManager.SendRpcToMaster(photonView, methodName, target, parameters);
        }

        public static void RpcAsMaster(this PhotonView photonView, string methodName, PhotonPlayer targetPlayer, params object[] parameters)
        {
            RpcManager.SendRpcToMaster(photonView, methodName, targetPlayer, parameters);
        }

        public static void RpcSecureAsMaster(this PhotonView photonView, string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
        {
            RpcManager.SendRpcSecureToMaster(photonView, methodName, target, encrypt, parameters);
        }

        public static void RpcSecureAsMaster(this PhotonView photonView, string methodName, PhotonPlayer targetPlayer, bool encrypt, params object[] parameters)
        {
            RpcManager.SendRpcSecureToMaster(photonView, methodName, targetPlayer, encrypt, parameters);
        }
        public static void ClearRpcBufferAsMasterClient(this PhotonView photonView)
        {
            RpcManager.SendClearRpcBufferRequestToMaster(photonView);
        }

        public static void ClearRpcBufferAsMasterClient(this PhotonPlayer photonPlayer)
        {
            RpcManager.SendClearRpcBufferRequestToMaster(photonPlayer);
        }

        /*
        //
        // ASSET DATABASE EXTENSIONS
        //
        public static string GetResourcesRelativePath(this GameObject asset)
        {
            string assetsRelativeString = "/" + AssetDatabase.GetAssetPath(asset);
            string resourcesString = "/Assets/Resources/";

            // Strip the file extension off
            int fileExtPos = assetsRelativeString.LastIndexOf(".");
            if (fileExtPos >= 0 )
                { assetsRelativeString = assetsRelativeString.Substring(0, fileExtPos); }

            Uri assetsRelativeUri = new Uri(assetsRelativeString, UriKind.Absolute);
            Uri resourcesUri = new Uri(resourcesString, UriKind.Absolute);

            string resourcesRelativePath = resourcesUri.MakeRelativeUri(assetsRelativeUri).ToString();
            return resourcesRelativePath;
        }
        */
    }
}