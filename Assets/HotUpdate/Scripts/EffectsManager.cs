using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
     public static EffectsManager instance;

     public GameObject hitEffect;
     GameObject[] instances = new GameObject[3];
     private void Awake()
     {
          instance = this;

          InstantiateThreeInactive(hitEffect);
     }

     public GameObject[] InstantiateThreeInactive(GameObject original, Transform parent = null)
     {
          for (int i = 0; i < 3; i++)
          {
               // 使用Inactive状态实例化
               instances[i] = Instantiate(original, parent, false);
               instances[i].SetActive(false);
          }

          return instances;
     }

     public  GameObject GetFirstInactiveEffect()
     {
          foreach (GameObject effect in instances)
          {
               if (effect != null && !effect.activeSelf)
               {
                    return effect;
               }
          }
          return null;
     }

     public GameObject ActivateEffectFromPool( Vector3 position, Quaternion rotation)
     {
          GameObject effect = GetFirstInactiveEffect();
          if (effect != null)
          {
               effect.SetActive(true);
               effect.transform.position = position;
               effect.transform.rotation = rotation;
          }
          return effect;
     }

     public void PlayEffect(Vector3 pos, Quaternion rot)
     {
          //ActivateEffectFromPool(pos, rot);
          //Instantiate(effect, pos, rot);
     }
}
