using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assembly_CSharp.Assets.Scripts.Slot
{
    public class TreeElement : MonoBehaviour
    {
        [SerializeField] List<GameObject> listApples;
        public void OnOpen()
        {
            gameObject.SetActive(true);
            OnSpawnApple();
            // animator.Play("");
        }
        public void OnClose()
        {
            gameObject.SetActive(false);
        }

        public void OnSpawnApple()
        {
            StartCoroutine(DurationShow());
        }

        public void OnHideApple()
        {
            for (int i = 0; i < listApples.Count; i++)
            {
                listApples[i].SetActive(false);
            }
        }
        IEnumerator DurationShow()
        {
            for (int i = 0; i < listApples.Count; i++)
            {
                yield return new WaitForSeconds(0.2f);
                listApples[i].SetActive(true);
            }
        }
    }
}
