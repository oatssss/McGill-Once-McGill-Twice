using UnityEngine;


namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
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
    }
}