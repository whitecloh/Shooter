using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Net
{

    public class ProjectileController : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed;
        [SerializeField]
        private int _damage;
        [SerializeField]
        private int _lifeTime;
        public PhotonView photonView;

        public int GetDamage => _damage;
        public string Parent { get; set; }

        private void Start()
        {
            StartCoroutine(OnDie());
        }
        private void Update()
        {
            ProjectileMovement();
        }
        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<ProjectileController>();
            if (bullet != null) return;

            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        private void ProjectileMovement()
        {
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }
        private IEnumerator OnDie()
        {
            yield return new WaitForSeconds(_lifeTime);
            Destroy(gameObject);
        }        
    }
}