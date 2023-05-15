using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a static class of utility methods that can be used by an object (a space ship) in a 3D space in order to provide 
/// the kinds of information that a ship's sensors would display regarding its own velocity, the velocity of other ships, and times 
/// and distances to key points. It is meant for use on an object using forces to move, and not affected by gravity. I'm not 
/// sure how well these methods would hold up in other circumstances. 
/// </summary>
public static class ShipMathUtilities
{
    //Distance Methods

    /// <summary>
    /// Calculate the distance between an object at the time of call and a Vector3 coordinate. 
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="destination"></param>
    /// <returns>The distance between the object calling the method, and the selected point in space.</returns>
    public static float DistanceToPoint(Rigidbody rb, Vector3 target)
    {
        //Debug.Log("Called method DistanceToPoint");
        return Vector3.Distance(rb.position, target);        
    }

    /// <summary>
    /// Calculate the time it will take a moving object to reach a Vector3 coordinate, given the object's speed and position at time of call. 
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="destination"></param>
    /// <returns>The time in seconds required for the object to reach the selected point in space, given the object's current velocity.</returns>
    public static float TimeToPoint(Rigidbody rb, Vector3 destination)
    {
        //Debug.Log("Called method TimeToPoint");
        float distance = DistanceToPoint(rb, destination);
        return distance / rb.velocity.magnitude;        
    }

    //Velocity Methods

    /// <summary>
    /// Something is wrong with this method. Returns negative when it should be positive. May depend on direction?
    /// This utility method calculates the rate of approach between 2 moving Rigidbodies. The return value will be positive if they are getting 
    /// closer, and negative if they are getting farther away.The return will be 0 if they are not moving relative to each other.
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="object2"></param>
    /// <returns>The rate of approach between two moving bodies.</returns>
    public static float CalculateApproachRate(Rigidbody object1, Rigidbody object2)
    {
        //Debug.Log("Called method CalculateApproachRate");
        Vector3 relativeVelocity = object2.velocity - object1.velocity;
        Vector3 relativePosition = object1.position - object2.position;
        float approachRate = Vector3.Dot(relativeVelocity, relativePosition.normalized);
        return approachRate;
    }

    /// <summary>
    /// Calculates the combined relative speed of 2 moving objects. Objects moving directly towards or away from each other will have a combined 
    /// relative speed of double their individual speed, assuming they are both moving at the same individual speed.Objects moving perpendicular 
    /// to each other will have a lower combined relative speed.
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="object2"></param>
    /// <returns>The combined speed, as a float, of two moving bodies.</returns>
    public static float CalculateCombinedSpeed(Rigidbody object1, Rigidbody object2)
    {
        //Debug.Log("Called method CalculateCombinedSpeed");
        Vector3 combinedVelocity = object1.velocity - object2.velocity;
        return combinedVelocity.magnitude;
    }

    /// <summary>
    /// Calculates the combined relative velocity of 2 moving objects, without converting it to a single float value. Objects moving directly 
    /// towards or away from each other will have a combined relative velocity of double their individual velocity, assuming they are both moving at 
    /// the same individual velocity.Objects moving perpendicular to each other will have a lower combined relative velocity.
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="object2"></param>
    /// <returns>the combined velocity of two moving bodies, without converting it to a float.</returns>
    public static Vector3 CalculateCombinedVelocity(Rigidbody object1, Rigidbody object2)
    {
        //Debug.Log("Called method CalculateCombinedVelocity");
        return object2.velocity - object1.velocity;
    }

    /// <summary>
    /// Calculate the velocity of a single Rigidbody object
    /// </summary>
    /// <param name="rb"></param>
    /// <returns></returns>
    public static Vector3 CalculateVelocity(Rigidbody rb)
    {
        //Debug.Log("Called method CalculateVelocity");
        return rb.velocity;
    }

    /// <summary>
    /// Calculate the velocity of a single Rigidbody object and convert it to a float
    /// </summary>
    /// <param name="rb"></param>
    /// <returns></returns>
    public static float CalculateSpeed(Rigidbody rb)
    {
        //Debug.Log("Called method CalculateSpeed");
        return rb.velocity.magnitude;
    }

    /// <summary>
    /// Calculate the relative forward velocity of a single Rigidbody object and return it as a float
    /// </summary>
    /// <param name="rb"></param>
    /// <returns></returns>
    public static float CalculateForwardVelocity(Rigidbody rb)
    {
        return Vector3.Dot(rb.velocity, rb.transform.forward);
    }

    /// <summary>
    /// Calculate whether or not a rigidbody is moving sideways, and return the absolute value of how much
    /// </summary>
    /// <param name="rb"></param>
    /// <returns></returns>
    public static float CalculateAbsSidewaysMotion(Rigidbody rb)
    {
        float sidewaysMotion = Vector3.Dot(rb.velocity, rb.transform.right);
        return Mathf.Abs(sidewaysMotion);
    }

    /// <summary>
    /// Calculate whether or not a rigidbody is moving vertically, and return the absolute value of how much
    /// </summary>
    /// <param name="rb"></param>
    /// <returns></returns>
    public static float CalculateAbsVerticalMotion(Rigidbody rb)
    {
        float verticalMotion = Vector3.Dot(rb.velocity, rb.transform.up);
        return Mathf.Abs(verticalMotion);
    }

    /// <summary>
    /// Calculate whether or not a rigidbody is moving sideways and by how much. A positive value means it is moving to its own right, a negative 
    /// value means it is moving to its own left. 
    /// </summary>
    /// <param name="rb"></param>
    /// <returns></returns>
    public static float CalculateSidewaysMotionRaw(Rigidbody rb)
    {
        return Vector3.Dot(rb.velocity, rb.transform.right);        
    }

    /// <summary>
    /// Calculate whether or not a rigidbody is moving vertically and by how much. A positive value means it is moving to its own up, a negative 
    /// value means it is moving to its own down. 
    /// </summary>
    /// <param name="rb"></param>
    /// <returns></returns>
    public static float CalculateVerticalMotionRaw(Rigidbody rb)
    {
        return Vector3.Dot(rb.velocity, rb.transform.up);
    }

    //Closest Point of Approach Methods

    /// <summary>
    /// Calculates the position that object 1 will be at, at the moment it reaches the closest point of approach to object2
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="object2"></param>
    /// <returns></returns>
    public static Vector3 CalculateCPAForSelf(Rigidbody object1, Rigidbody object2)
    {
        //Debug.Log("Called method CalculateCPAForSelf");
        Vector3 relativeVelocity = CalculateCombinedVelocity(object1, object2);
        Vector3 relativePosition = object2.transform.position - object1.transform.position;
        float tca = -Vector3.Dot(relativePosition, relativeVelocity) / Mathf.Pow(relativeVelocity.magnitude, 2);
        Vector3 cpa = object1.transform.position + (object1.velocity * tca);
        return cpa;
    }

    /// <summary>
    /// Calculates the position that object2 will be at, at the moment it reaches the closest point of approach to object1
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="object2"></param>
    /// <returns></returns>
    public static Vector3 CalculateCPAForOther(Rigidbody object1, Rigidbody object2)
    {
        //Debug.Log("Called method CalculateCPAForOther");
        Vector3 relativeVelocity = CalculateCombinedVelocity(object1, object2);
        Vector3 relativePosition = object2.transform.position - object1.transform.position;
        float tca = -Vector3.Dot(relativePosition, relativeVelocity) / Mathf.Pow(relativeVelocity.magnitude, 2);
        Vector3 cpa = object2.transform.position + (object2.velocity * tca);
        return cpa;
    }

    /// <summary>
    /// Calculate the distance that an object must travel from its position at the time of call to its CPA with another object. That CPA 
    /// must be calculated externally. 
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="ownCPA"></param>
    /// <returns></returns>
    public static float DistanceToCPA(Rigidbody object1, Vector3 ownCPA)
    {
        //Debug.Log("Called method DistanceToCPA");
        return Vector3.Distance(object1.position, ownCPA);
    }

    /// <summary>
    /// Calculate the distance two objects will be separated by at their CPA
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="object2"></param>
    /// <returns></returns>
    public static float DistanceAtCPA(Rigidbody object1, Rigidbody object2)
    {
        //Debug.Log("Called method DistanceAtCPA");
        Vector3 object1CPA = CalculateCPAForSelf(object1, object2);
        Vector3 object2CPA = CalculateCPAForSelf(object2, object1);
        return Vector3.Distance(object1CPA, object2CPA);
    }

    /// <summary>
    /// Calculates the time it will take an object to reach its CPA with another object
    /// </summary>
    /// <param name="object1"></param>
    /// <param name="object2"></param>
    /// <returns></returns>
    public static float CalculateTimeToCPA(Rigidbody object1, Rigidbody object2)
    {
        //Debug.Log("Called method CalculateTimeToCPA");
        Vector3 relativeVelocity = CalculateCombinedVelocity(object1, object2);
        Vector3 relativePosition = object2.transform.position - object1.transform.position;
        float tca = -Vector3.Dot(relativePosition, relativeVelocity) / Mathf.Pow(relativeVelocity.magnitude, 2);
        return tca;
    }
}

