using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace TJ.Scripts
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager singleton;
        [FormerlySerializedAs("playersInScene")] public List<Character> sceneCharacters = new();
        [FormerlySerializedAs("totalPlayerList")] public List<Character> allCharacterList = new();
        [FormerlySerializedAs("activePlayerList")] public List<Character> activeCharacterList = new();
        public GameObject PlayerPrefab;
        public Transform spawnPoint;
        public Transform pickPoint;
        public Vector3 midPoint;
        private System.Random r = new System.Random();

        [Header("Shuffle")] public bool canShuffle = true;
        [Range(0, 1)] public float shuffleIntensity = 0.5f; // Float variable to control shuffle intensity

        public List<Vector3> pointsBetweenMidAndPick;
        public List<Vector3> pointsBetweenMidAndSpawn;
        public List<Vector3> allPoints;

        private void Awake()
        {
            singleton = this;
            GeneratePoints();
        }

        private void OnEnable()
        {
            EventManager.OnNewVehArrived += AnyCarColorMatched;
        }

        private void OnDisable()
        {
            EventManager.OnNewVehArrived -= AnyCarColorMatched;
        }

        public void InstantiatePlayers(Vehicle[] vehicles)
        {
            foreach (Vehicle v in vehicles)
            {
                for (int i = 0; i < v.SeatCount; i++)
                {
                    GameObject obj = Instantiate(PlayerPrefab, spawnPoint);
                    Character plyr = obj.GetComponent<Character>();
                    plyr.UpdateColor(v.vehicleColor);
                    sceneCharacters.Add(plyr);
                }
            }

            StartCoroutine(PlayerMovement());
            //  ShufflePlayerList();
        }

        public void GeneratePoints()
        {
            midPoint = new Vector3(spawnPoint.position.x, pickPoint.position.y, pickPoint.position.z);

            pointsBetweenMidAndPick = GeneratePointsBetween(pickPoint.position, midPoint, 12);

            pointsBetweenMidAndSpawn = GeneratePointsBetween(midPoint, spawnPoint.position, 9);

            allPoints = new();

            allPoints.Add(pickPoint.position);

            allPoints.AddRange(pointsBetweenMidAndPick);

            allPoints.Add(midPoint);

            allPoints.AddRange(pointsBetweenMidAndSpawn);

            allPoints.Add(spawnPoint.position);
        }

        private List<Vector3> GeneratePointsBetween(Vector3 start, Vector3 end, int numberOfPoints)
        {
            List<Vector3> points = new List<Vector3>();
            for (int i = 1; i <= numberOfPoints; i++)
            {
                float t = i / (float)(numberOfPoints + 1);
                Vector3 point = Vector3.Lerp(start, end, t);
                points.Add(point);
            }

            return points;
        }

        public IEnumerator PlayerMovement()
        {
            yield return new WaitForSeconds(0f);
            //shuffle playersinScene list here
            if (canShuffle)
                sceneCharacters = ShufflePlayerListBasedOnColor(sceneCharacters, shuffleIntensity);
            allCharacterList = new List<Character>(sceneCharacters);
            //ShufflePlayerList();
            for (int i = 0; i < allCharacterList.Count; i++)
            {
                allCharacterList[i].transform.gameObject.SetActive(false);
            }

            for (int i = 0; i < 24; i++)
            {
                if (allCharacterList.Count <= 0 || !allCharacterList[0]) continue;
                activeCharacterList.Add(allCharacterList[0]);
                allCharacterList.RemoveAt(0);
            }

            var points = allPoints;


            for (int i = 0; i < activeCharacterList.Count; i++)
            {
                Character currentCharacter = activeCharacterList[i];
                currentCharacter.transform.gameObject.SetActive(true);
                currentCharacter.characterAnim.SetBool(Character.WalkParam, true);
                if (i < 14)
                {
                    StartCoroutine(currentCharacter.NavigateToSlot1(midPoint, pickPoint, points[i], i * .15f));
                }
                else
                {
                    StartCoroutine(currentCharacter.NavigateToSlot2(points[i], i * .15f));
                }
            }
        }

        private List<Character> ShufflePlayerListBasedOnColor(List<Character> list, float intensity)
        {
            // Separate players by color
            var colorGroups = list.GroupBy(player => player.playerColor).ToList();

            // Flatten the color groups back to a list, starting with four unique colors
            var firstHalf = new List<Character>();
            var secondHalf = new List<Character>();

            // Add players from the first four unique color groups to the first half
            foreach (var group in colorGroups.Take(4))
            {
                firstHalf.AddRange(group);
            }

            // Add remaining players to the second half
            foreach (var group in colorGroups.Skip(4))
            {
                secondHalf.AddRange(group);
            }

            // Shuffle each half based on intensity
            firstHalf = ShuffleWithIntensity(firstHalf, intensity);
            secondHalf = ShuffleWithIntensity(secondHalf, intensity);

            // Combine first and second halves
            return firstHalf.Concat(secondHalf).ToList();
        }

        private List<Character> ShuffleWithIntensity(List<Character> list, float intensity)
        {
            // Apply Fisher-Yates shuffle with intensity control
            int n = list.Count;
            float intensityFactor = intensity * n;
            int randomFactor= 0;
            for (int i = 0; i < n - 1; i++)
            {
                float factor = Random.Range(0f, 1f);
                randomFactor=Mathf.FloorToInt(factor * intensity * (n - i));
                int j = Mathf.Min(i +randomFactor, n - 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }


        public bool isColormatched;
        private Coroutine _rout;

        public void AnyCarColorMatched()
        {
            var cars = ParkingController.current.vehiclesInParking;
            if (cars.Count <= 0)
            {
                return;
            }

            foreach (var car in cars)
            {
                if (activeCharacterList.Count > 0 && activeCharacterList[0].playerColor == car.vehicleColor && !car.isFull)
                {
                    isColormatched = true;
                    StartCoroutine(activeCharacterList[0].BoardVehicle(car));
                    return;
                }
            }

            isColormatched = false;
            if (_rout != null)
                StopCoroutine(_rout);
            _rout = StartCoroutine(GameManager.instance.CheckIfGameOver());
        }


        public void RepositionPlayers()
        {
            /*if (count <= totalPlayerList.Count)
                activePlayerList.Add(totalPlayerList[++count]);*/
            activeCharacterList.RemoveAt(0);
            if (allCharacterList.Count > 0)
            {
                activeCharacterList.Add(allCharacterList[0]);
                allCharacterList[0].gameObject.SetActive(true);
                allCharacterList.RemoveAt(0);
            }

            for (int i = 0; i < activeCharacterList.Count; i++)
            {
                Character currentCharacter = activeCharacterList[i];
                currentCharacter.characterAnim.SetBool(Character.WalkParam, true);
                Vector3 startPosition = currentCharacter.transform.position;
                Vector3 endPosition = allPoints[i];
                currentCharacter.transform.DOMove(endPosition, 0.1f)
                    .OnComplete(() => currentCharacter.characterAnim.SetBool(Character.WalkParam, false));
                Vector3 direction = (endPosition - startPosition).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                currentCharacter.transform.DORotate(targetRotation.eulerAngles, 0.2f);
            }

            AnyCarColorMatched();
        }

        public void ShufflePlayerList()
        {
            // Group players by color
            var groupedByColor = sceneCharacters
                .GroupBy(p => p.playerColor)
                .OrderBy(x => r.Next())
                .ToList();

            sceneCharacters.Clear();

            int totalPlayers = groupedByColor.Sum(g => g.Count());
            int segmentSize = Mathf.CeilToInt(totalPlayers / 4f); // Divide into 4 segments

            for (int segment = 0; segment < 4; segment++)
            {
                if (groupedByColor.Count < 3) break;

                // Select three different color groups for this segment
                var selectedGroups = groupedByColor.Take(3).ToList();
                groupedByColor = groupedByColor.Skip(3).ToList();

                // Create a list to hold the players from these three colors
                List<Character> selectedPlayers = new List<Character>();

                // Add players from each selected group to the list
                foreach (var group in selectedGroups)
                {
                    selectedPlayers.AddRange(group.OrderBy(x => r.Next()).Take(segmentSize / 3).ToList());
                }

                // Shuffle the selected players
                selectedPlayers = selectedPlayers.OrderBy(x => r.Next()).ToList();

                // Occasionally insert one or two players from a fourth color
                if (groupedByColor.Count > 0 && r.Next(100) < 25) // 25% chance to insert a fourth color
                {
                    var fourthColorGroup = groupedByColor[0].ToList();
                    int insertCount = r.Next(1, 3); // Randomly insert 1 or 2 players

                    insertCount =
                        Mathf.Min(insertCount,
                            fourthColorGroup.Count); // Ensure we don't take more players than available

                    // Insert players from the fourth color into random positions in selectedPlayers
                    for (int i = 0; i < insertCount; i++)
                    {
                        int insertPosition = r.Next(1, selectedPlayers.Count);
                        selectedPlayers.Insert(insertPosition, fourthColorGroup[i]);
                    }

                    // Remove the inserted players from the fourth color group
                    fourthColorGroup.RemoveRange(0, insertCount);

                    // If there are still players left in the fourth color group, update groupedByColor
                    if (fourthColorGroup.Count > 0)
                    {
                        groupedByColor[0] = fourthColorGroup.GroupBy(p => p.playerColor).First();
                    }
                    else
                    {
                        groupedByColor.RemoveAt(0);
                    }
                }

                // Add the shuffled and possibly enhanced players to the main list
                sceneCharacters.AddRange(selectedPlayers);
            }

            // If any color groups remain, add them to the end of the list
            foreach (var remainingGroup in groupedByColor)
            {
                sceneCharacters.AddRange(remainingGroup.OrderBy(x => r.Next()).ToList());
            }
            Debug.Log($"Shuffled playersInScene Count: {sceneCharacters.Count}");
        }


        public void UpdatePlayerPos(int posCount)
        {
            // Remove the player who has just moved from the queue
            if (activeCharacterList.Count > 0)
            {
                // If there are still players left in the totalPlayerList, add the next one to the queue
                if (allCharacterList.Count > 0)
                {
                    Character newCharacter = allCharacterList[0];
                    activeCharacterList.Add(newCharacter);
                    allCharacterList.RemoveAt(0);
                    newCharacter.transform.gameObject.SetActive(true);
                }

                // Reposition the remaining players in the queue
                for (int i = posCount; i < activeCharacterList.Count; i++)
                {
                    Character currentCharacter = activeCharacterList[i];
                    currentCharacter.transform.DOMove(allPoints[i - posCount], 0.1f);

                    // Adjust the rotation for players before the midpoint
                    if (i <= 14 + posCount)
                    {
                        currentCharacter.transform.rotation = pickPoint.rotation;
                    }
                }
            }
        }


    }
}