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
    }
}