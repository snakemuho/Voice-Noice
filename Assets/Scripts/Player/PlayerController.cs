using System.Collections;
using System.Collections.Generic;
using Anathema.Game;
using Anathema.Microphone;
using TMPro;
using UnityEngine;

namespace Anathema.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("PlayerController")]
        [SerializeField] public Transform Camera;
        [SerializeField] public ItemChange Items;
        [SerializeField, Range(1, 10)] float walkingSpeed = 3.0f;
        [Range(0.1f, 5)] public float CroughSpeed = 1.0f;
        [SerializeField, Range(2, 20)] float RuningSpeed = 4.0f;
        [SerializeField, Range(0, 20)] float jumpSpeed = 6.0f;
        [SerializeField, Range(0.5f, 10)] float lookSpeed = 2.0f;
        [SerializeField, Range(10, 120)] float lookXLimit = 80.0f;
        [Space(20)]
        [Header("Advance")]
        [SerializeField] float RunningFOV = 65.0f;
        [SerializeField] float SpeedToFOV = 4.0f;
        [SerializeField] float CroughHeight = 1.0f;
        [SerializeField] float gravity = 20.0f;
        [SerializeField] float timeToRunning = 2.0f;
        [HideInInspector] public bool canMove = true;
        [HideInInspector] public bool CanRunning = true;

        [Space(20)]
        [Header("Climbing")]
        [SerializeField] bool CanClimbing = true;
        [SerializeField, Range(1, 25)] float Speed = 2f;
        bool isClimbing = false;

        [Space(20)]
        [Header("HandsHide")]
        [SerializeField] bool CanHideDistanceWall = true;
        [SerializeField, Range(0.1f, 5)] float HideDistance = 1.5f;
        [SerializeField] int LayerMaskInt = 1;

        [Space(20)]
        [Header("Input")]
        [SerializeField] KeyCode CroughKey = KeyCode.LeftControl;

        [Space(20)]
        [Header("Audio Detector")]
        public MicrophoneLoudnessDetection detector;
        [SerializeField] PlayerData playerData;


        public float[] spectrum;
        float horizontalMovement;
        [SerializeField, Range(0f, 100f)]
        float maxSpeed = 10f;

        [SerializeField] GameObject lightMark;

        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Vector3 moveDirection = Vector3.zero;
        bool isCrough = false;
        float InstallCroughHeight;
        float rotationX = 0;
        [HideInInspector] public bool isRunning = false;
        Vector3 InstallCameraMovement;
        float InstallFOV;
        Camera cam;
        [HideInInspector] public bool Moving;
        [HideInInspector] public float vertical;
        [HideInInspector] public float horizontal;
        [HideInInspector] public float Lookvertical;
        [HideInInspector] public float Lookhorizontal;
        float RunningValue;
        float installGravity;
        public bool WallDistance { get; private set; }
        [HideInInspector] public float WalkingValue;

        RaycastHit CroughCheck;
        public RaycastHit ObjectCheck;
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            if (Items == null && GetComponent<ItemChange>()) Items = GetComponent<ItemChange>();
            cam = GetComponentInChildren<Camera>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            InstallCroughHeight = characterController.height;
            InstallCameraMovement = Camera.localPosition;
            InstallFOV = cam.fieldOfView;
            RunningValue = RuningSpeed;
            installGravity = gravity;
            WalkingValue = walkingSpeed;
            Input.gyro.enabled = true;

        }

        void MoveFromMic()
        {

            float loudness = detector.GetLoudnessFromMic() * playerData.loudnessSensitivity;
            if (loudness < playerData.minVolume - playerData.threshold)
            {
                horizontalMovement = 0;
            }
            else horizontalMovement = Mathf.Clamp(loudness * maxSpeed, 2, 5);
        }


        void Update()
        {
            if (GameManager.Instance.GamePaused || !GameManager.Instance.isAlive)
                return;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

#if UNITY_ANDROID
            transform.Rotate(0, -Input.gyro.rotationRateUnbiased.y * 4, 0);
#endif
            if (!characterController.isGrounded && !isClimbing)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            MoveFromMic();

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            isRunning = !isCrough ? CanRunning ? Input.GetKey(KeyCode.LeftShift) : false : false;
//#if UNITY_STANDALONE_WIN
//            vertical = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Vertical") : 0;
//#endif
//#if UNITY_ANDROID
            vertical = canMove ? (isRunning ? RunningValue : WalkingValue) * horizontalMovement : 0;
//#endif
            horizontal = canMove ? (isRunning ? RunningValue : WalkingValue) * Input.GetAxis("Horizontal") : 0;
            if (isRunning) RunningValue = Mathf.Lerp(RunningValue, RuningSpeed, timeToRunning * Time.deltaTime);
            else RunningValue = WalkingValue;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * vertical) /*+ (right * horizontal)*/;

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded && !isClimbing)
            {
                moveDirection.y = jumpSpeed;
            } 
            else
            {
                moveDirection.y = movementDirectionY;
            }
            characterController.Move(moveDirection * Time.deltaTime);
            Moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;

//#if UNITY_STANDALONE_WIN
            if (Cursor.lockState == CursorLockMode.Locked && canMove)
            {
                //Lookvertical = -Input.GetAxis("Mouse Y");
                Lookhorizontal = Input.GetAxis("Mouse X");


                //rotationX += Lookvertical * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                Camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Lookhorizontal * lookSpeed, 0);

                if (isRunning && Moving) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, RunningFOV, SpeedToFOV * Time.deltaTime);
                else cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, InstallFOV, SpeedToFOV * Time.deltaTime);
            }
//#endif
            if (Input.GetKey(CroughKey))
            {
                isCrough = true;
                float Height = Mathf.Lerp(characterController.height, CroughHeight, 5 * Time.deltaTime);
                characterController.height = Height;
                WalkingValue = Mathf.Lerp(WalkingValue, CroughSpeed, 6 * Time.deltaTime);

            }
            else if (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.up), out CroughCheck, 0.8f, 1))
            {
                if (characterController.height != InstallCroughHeight)
                {
                    isCrough = false;
                    float Height = Mathf.Lerp(characterController.height, InstallCroughHeight, 6 * Time.deltaTime);
                    characterController.height = Height;
                    WalkingValue = Mathf.Lerp(WalkingValue, walkingSpeed, 4 * Time.deltaTime);
                }
            }

            if (WallDistance != Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.forward), out ObjectCheck, HideDistance, LayerMaskInt) && CanHideDistanceWall)
            {
                if (ObjectCheck.collider?.gameObject.tag != "Monster")
                {
                    WallDistance = Physics.Raycast(GetComponentInChildren<Camera>().transform.position, transform.TransformDirection(Vector3.forward), out ObjectCheck, HideDistance, LayerMaskInt);
                    Items.ani.SetBool("Hide", WallDistance);
                    Items.DefiniteHide = WallDistance;
                }
            }
        }

        public void CalibrateVolumeMin(TextMeshProUGUI text)
        {
            StartCoroutine(CalibrateVolume(result => playerData.minVolume = result, text));
        }

        public void CalibrateVolumeMax(TextMeshProUGUI text)
        {
            StartCoroutine(CalibrateVolume(result => playerData.maxVolume = result, text));
        }

        public void CalibrateVolumeThreshold(TextMeshProUGUI text)
        {
            StartCoroutine(CalibrateVolume(result => playerData.threshold = result, text));
        }
        
        IEnumerator CalibrateVolume(System.Action<float> myVariableResult, TextMeshProUGUI text)
        {
            List<float> minMaxVolumes = new();
            for (int i = 0; i < 20; i++)
            {
                minMaxVolumes.Add(detector.GetLoudnessFromMic());
                yield return null;
            }
            float res = Mathf.Max(minMaxVolumes.ToArray());
            myVariableResult(res * playerData.loudnessSensitivity);
            Debug.Log("threshold: " + playerData.threshold);
            Debug.Log("min volume: " + playerData.minVolume);
            Debug.Log("max volume: " + playerData.maxVolume);
            text.text = res.ToString("0.###");
        }
        public void CalibrateLight()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Ladder" && CanClimbing)
            { 
                CanRunning = false;
                isClimbing = true;
                WalkingValue /= 2;
                Items.Hide(true);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Ladder" && CanClimbing)
            {
                moveDirection = new Vector3(0, Input.GetAxis("Vertical") * Speed * (-Camera.localRotation.x / 1.7f), 0);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Ladder" && CanClimbing)
            {
                CanRunning = true;
                isClimbing = false;
                WalkingValue *= 2;
                Items.ani.SetBool("Hide", false);
                Items.Hide(false);
            }
        }

    }
}