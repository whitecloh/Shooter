using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Net
{
    public class PlayerController : MonoBehaviour,IPunObservable
    {
        [SerializeField]
        private float _speed = 140f;
        [SerializeField]
        private float _jumpPower;
        [SerializeField]
        private float _mouseSensivity;
        [SerializeField]
        private ProjectileController _bullet;
        [SerializeField]
        private Transform _shootPosition;
        [SerializeField]
        private Transform _gun;
        [SerializeField]
        public Camera _camera;
        [SerializeField]
        private Transform _orientation;
        [SerializeField]
        private Rigidbody rb;
        [SerializeField]
        private PhotonView _photonView;
        [SerializeField]
        private Text _healths;

        public float Health;

        private List<ProjectileController> _bullets;

        private float xRotation;
        private float yRotation;
        private Vector3 _moveDirection;
        [SerializeField]
        private Transform _groundCheck;
        private readonly float _groundDrag = 5f;
        private bool isGrounded;
        private readonly float _maxGroundDistance = 0.45f;
        private float _jumpDelay;
        private readonly float _defaultJumpDelay = 1f;

        private float _shootingDelay;
        private readonly float _defaultShootingDelay = 0.3f;

        [SerializeField]
        private Material _matBlink;
        [SerializeField]
        private Material _defaultMaterial;

        private bool _isGameOver = false;

        private void Start()
        {
#if UNITY_EDITOR
            xRotation = 0;
            yRotation = 0;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            xRotation = 180;
            yRotation = 0;       
#endif

            FindObjectOfType<Net.GameManager>().AddPlayer(this);
            gameObject.GetComponent<Renderer>().material = _defaultMaterial;
            _bullets = new List<ProjectileController>();
            if (!_photonView.IsMine) _healths.enabled = false;
            _healths.text = Health.ToString();
        }
        private void FixedUpdate()
        {
            if (!_photonView.IsMine&&_isGameOver!=true) return;
            Movement();
            Jump();
            Shooting();
            Rotation();
            SpeedControl();
            Shifting();
        }
        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<ProjectileController>();
            if (bullet == null) return; 
            if(bullet.photonView.IsMine)
            {
                Health = Mathf.Clamp(Health - bullet.GetDamage, 0, int.MaxValue);
                gameObject.GetComponent<Renderer>().material = _matBlink;
            }
            else
            {
                Health = Mathf.Clamp(Health - bullet.GetDamage, 0, int.MaxValue);
                _healths.text = Health.ToString();
            }              

            if(Health<=0)
            {
                OnDead();
            }
            else
            {
                Invoke(nameof(ResetMaterial), 0.1f);
            }
        }

        private void ResetMaterial()
        {
            gameObject.GetComponent<Renderer>().material = _defaultMaterial;
        }
        private void OnDead()
        {
            var pm = FindObjectOfType<OpenResults>();
            if (_photonView.IsMine)
                pm.OpenResultsMenu("You Lose");
            else
                pm.OpenResultsMenu("You Win");
            Destroy(gameObject);
            _isGameOver = true;

        }
        private void Movement()
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");
            _moveDirection = _orientation.forward * z + _orientation.right * x;
            rb.AddForce(_moveDirection.normalized * _speed);
            if(isGrounded)
            {
                rb.drag = _groundDrag;
                rb.mass = 1;
                rb.AddForce(_orientation.up * -75);
            }
            else
            {
                rb.drag = 2;
                rb.AddForce(_orientation.up * -250);
                rb.mass = 10;
            }

        }
        private void Shifting()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _speed = 90f;
            }
            else
            {
                _speed = 150f;
            }
        }
        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if(flatVel.magnitude>_speed/5)
            {
                Vector3 limitedVel = flatVel.normalized * _speed/5;
                rb.velocity = new Vector3(limitedVel.x, 0f, limitedVel.z);
            }
        }
        private void Jump()
        {
            isGrounded = Physics.Raycast(_groundCheck.transform.position, Vector3.down, _maxGroundDistance);
            _jumpDelay -= Time.deltaTime;

            if (_jumpDelay < 0)
            {
                if (Input.GetKey(KeyCode.Space) && isGrounded)
                {
                    _jumpDelay = _defaultJumpDelay;
                    rb.AddForce(_orientation.up*_jumpPower);
                }
            }
        }
        private void Shooting()
        {
            RaycastHit laserHit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out laserHit))
            {
                Vector3 dir = laserHit.transform.position - ray.origin;
                Debug.DrawRay(ray.origin, ray.direction*dir.magnitude, Color.green);
            }
            _shootingDelay -= Time.deltaTime;
            if (_shootingDelay < 0)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    _shootingDelay =_defaultShootingDelay;
                    var bullet = PhotonNetwork.Instantiate("Bullet", _shootPosition.position, new Quaternion());                    
                    bullet.transform.LookAt(laserHit.point);
                    _bullets.Add(bullet.GetComponent<ProjectileController>());
                }
            }
        }

        private void Rotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensivity;

            xRotation += mouseX;
            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);
            _camera.transform.rotation = Quaternion.Euler(yRotation, xRotation , 0f);
            transform.rotation = Quaternion.Euler(0f, xRotation, 0f);
            _shootPosition.rotation = _camera.transform.rotation;
            _gun.rotation = _camera.transform.rotation;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(PlayerData.Create(this));
            }
            else
            {
                ((PlayerData)stream.ReceiveNext()).Set(this);
            }
        }
    }
}
