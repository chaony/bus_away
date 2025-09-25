using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TJ.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public MaterialHolder vehMaterialHolder;
        public MaterialHolder stickmanMaterialHolder;
        public int winCount = 0;

        public bool gameOver = false;

        // Start is called before the first frame update
        private void Awake()
        {
            instance = this;
            Vibration.Init();
            //MaterialHolder.InitializeMaterialDictionary();
            adjustExposure();
        }

        private void adjustExposure()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // 获取主摄像机上的 Color Adjustments 组件
               // ColorAdjustments colorAdjustments = mainCamera.GetComponent<ColorAdjustments>();
                Volume volume = mainCamera.GetComponent<Volume>();

                ColorAdjustments colorAdjustments = null;

                if (volume != null && volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
                {
                    colorAdjustments.postExposure.value = -2f;
                    Debug.Log("ColorAdjustments component found!");
                    // 你可以在这里访问colorAdjustments的属性
                }
                else
                {
                    Debug.LogWarning("ColorAdjustments component not found on the volume.");
                }
                //ColorAdjustments colorAdjustments=volume.profile.Get<ColorAdjustments>();
            }
            else
            {
                Debug.LogError("Main camera not found.");
            }
        }

        private void Start()
        {
            Application.targetFrameRate = 120;
        }

        private bool IfSameColorVehicleParked()
        {
            var vehicles = ParkingController.current.vehiclesInParking;
            if (vehicles.Count > 0 && CharacterManager.singleton.activeCharacterList.Count > 0)
            {
                foreach (var VARIABLE in vehicles)
                {
                    if (VARIABLE.vehicleColor == CharacterManager.singleton.activeCharacterList[0].playerColor)
                    {
                        return true;
                    }
                }
            }
            else if (vehicles.Count <= 0)
            {
                return true;
            }
            else if (CharacterManager.singleton.activeCharacterList.Count <= 0)
            {
                return true;
            }

            return false;
        }

        public bool ChekIfSlotFull()
        {
            var vehicles = ParkingController.current.vehiclesInParking;
            if (vehicles.Count == ParkingController.current.parkingSpaces.Count - 1)
            {
                Debug.Log("<color=yellow>Warning: Only One Slot Left</color>");
            }

            if (vehicles.Count == ParkingController.current.parkingSpaces.Count)
                return true;
            return false;
        }

        public IEnumerator CheckIfGameOver()
        {
            yield return new WaitForSeconds(3f);
            if (ChekIfSlotFull() && IfSameColorVehicleParked() == false)
            {
                gameOver = true;
                SoundController.Instance.HandleGameResultAudio(true);
                UIManager.instance.TogglePanel(UIManager.instance.gameOverPanle, true);
                //Debug.Log("<color=red>Warning: Game Over</color>");
            }
        }

        private bool alreaduCalled;

        public void CheckGameWin()
        {
            if (alreaduCalled)
                return;

            winCount++;
            if (winCount == VehicleController.instance.totalVehicles)
            {
                Debug.Log("Activating win panel");
                alreaduCalled = true;
                
                DOVirtual.DelayedCall(1.5f, () => SoundController.Instance.HandleGameResultAudio(false));
                DOVirtual.DelayedCall(2f, () => UIManager.instance.TogglePanel(UIManager.instance.winPanel, true));
                LevelManager.LevelProgressed();
                //Debug.Log("<color=Green>Success: Game Win</color>");
            }
        }
    }
}