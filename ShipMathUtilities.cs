using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class ShipMathUtilities
    {
        /// <summary>
        /// This utility method calculates the rate of approach between 2 moving Rigidbodies. The return value will be positive if they are getting 
        /// closer, and negative if they are getting farther away.The return will be 0 if they are not moving relative to each other.
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns></returns>
        public static float CalculateApproachRate(Rigidbody object1, Rigidbody object2)
        {
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
        /// <returns></returns>
        public static float CalculateCombinedSpeed(Rigidbody object1, Rigidbody object2)
        {
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
        /// <returns></returns>
        public static Vector3 CalculateCombinedVelocity(Rigidbody object1, Rigidbody object2)
        {
            return object1.velocity - object2.velocity;
        }

        /// <summary>
        /// Calculate the velocity of a single Rigidbody object
        /// </summary>
        /// <param name="rb"></param>
        /// <returns></returns>
        public static Vector3 CalculateVelocity(Rigidbody rb)
        {
            return rb.velocity;
        }

        /// <summary>
        /// Calculate the velocity of a single Rigidbody object and convert it to a float
        /// </summary>
        /// <param name="rb"></param>
        /// <returns></returns>
        public static float CalculateSpeed(Rigidbody rb)
        {
            return rb.velocity.magnitude;
        }

    }
}
