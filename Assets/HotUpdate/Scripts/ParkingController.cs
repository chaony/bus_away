using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace TJ.Scripts
{
    public class ParkingController : MonoBehaviour
    {
        public static ParkingController current;
        [FormerlySerializedAs("slots")] public List<ParkingSlots> parkingSpaces;
        [FormerlySerializedAs("parkedVehicles")] public List<Vehicle> vehiclesInParking;
        [FormerlySerializedAs("parkingSlot_Rv")] public ParkingSlots rvParkingSpot;

        [FormerlySerializedAs("exitPoint")] public Transform pointOfExit;

        // Start is called before the first frame update
        private void Awake()
        {
            current = this;
        }

        public ParkingSlots FindAvailableSpace()
        {
            foreach(ParkingSlots space in parkingSpaces)
            {
                if (space.isOccupied == false)
                {
                    return space;
                }
            }
            return null;
        }
    }
}