using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagment
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int _sceneToLoad = -1;
        [SerializeField] private Transform _spawnPoint;
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                StartCoroutine(Transition());
            }  
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(_sceneToLoad);

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);


            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
               if(portal == this) continue;
               return portal;
            }
            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal._spawnPoint.position);
            player.transform.rotation = otherPortal._spawnPoint.rotation;

        }
    }
}

