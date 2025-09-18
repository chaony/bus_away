using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace TJ.Scripts
{
    public class Character : MonoBehaviour
    {
        private static readonly int SitParam = Animator.StringToHash("Sit");
        public static readonly int WalkParam = Animator.StringToHash("Walk");
        public ColorEnum playerColor;
        public Renderer characterRenderer;
        public Animator characterAnim;
        public GameObject animationObject;

        private void Awake()
        {
            characterAnim = animationObject.GetComponent<Animator>();
        }

        public void UpdateColor(ColorEnum colorType)
        {
            Material material = VehicleController.instance.stickmanMaterialHolder.FindMaterialByName(colorType);
            characterRenderer.material = material;
            gameObject.name += colorType.ToString();
            playerColor = colorType;
        }

        public IEnumerator NavigateToSlot1(Vector3 mid, Transform pickpoint, Vector3 point, float delay)
        {
            yield return new WaitForSeconds(delay);
            transform.DOMove(mid, 0.3f).OnComplete(() =>
            {
                transform.rotation = pickpoint.rotation;
                transform.DOMove(point, 0.3f).OnComplete(() =>
                {
                    characterAnim.SetBool(WalkParam, false);
                });
            });
        }
        public IEnumerator NavigateToSlot2(Vector3 destination, float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
            transform.DOMove(destination, 0.3f).OnComplete(() =>
            {
                characterAnim.SetBool(WalkParam, false);
            });
        }

        public IEnumerator BoardVehicle(Vehicle targetVehicle)
        {
            CharacterManager.singleton.sceneCharacters.Remove(this);
            var vehicleSeat = targetVehicle.GetFreeSeat();
            transform.parent = vehicleSeat.transform;
            characterAnim.SetBool(WalkParam, true);
            Vector3[] waypoints = new Vector3[]
            {
                transform.position,
                transform.position - new Vector3(0,0,1),
                targetVehicle.transform.position,
                vehicleSeat.transform.position
            };
            transform.DOPath(waypoints, 0.8f, PathType.CatmullRom).OnComplete(() =>
            {
                characterAnim.SetBool(WalkParam, true);
                characterAnim.SetBool(SitParam, true);
                transform.localRotation = Quaternion.identity;
                transform.localPosition += new Vector3(0, -0.34f, 0.2f);
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            });
            yield return new WaitForSeconds(0.1f);
            VehicleController.instance.UpdatePlayerCount();
            CharacterManager.singleton.RepositionPlayers();
            SoundController.Instance.PlayOneShot(SoundController.Instance.sort);
            
        }


    }
}
