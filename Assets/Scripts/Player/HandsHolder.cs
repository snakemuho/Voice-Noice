using Anathema.Game;
using UnityEngine;

namespace Anathema.Player
{
    public class HandsHolder : MonoBehaviour
    {
        [Header("HandsHolder")]
        [SerializeField] bool Enabled = true;
        [Space, Header("Main")]
        [SerializeField, Range(0.0005f, 0.02f)] float Amount = 0.005f;
        [SerializeField, Range(1.0f, 3.0f)] float SprintAmount = 1.4f;

        [SerializeField, Range(5f, 20f)] float Frequency = 13.0f;
        [SerializeField, Range(50f, 10f)] float Smooth = 24.2f;
        [Header("RotationMovement")]
        [SerializeField] bool EnabledRotationMovement = true;
        [SerializeField, Range(0.1f, 10.0f)] float RotationMultipler = 6f;
        float ToggleSpeed = 1.5f;
        float AmountValue;
        Vector3 StartPos;
        Vector3 StartRot;
        Vector3 FinalPos;
        Vector3 FinalRot;
        CharacterController player;

        AudioSource audioSus;
        [SerializeField] AudioClip[] footstepSounds;
        [SerializeField] float footstepCDBASE = 0.3f;
        float footstepCD;

        private void Awake()
        {
            footstepCD = footstepCDBASE;
            audioSus = GetComponent<AudioSource>();
            player = GetComponentInParent<CharacterController>();
            if (player.transform.GetComponent<PlayerController>() != null) ToggleSpeed = player.transform.GetComponent<PlayerController>().CroughSpeed * 1.5f;
            else ToggleSpeed = 1.5f;
            AmountValue = Amount;
            StartPos = transform.localPosition;
            StartRot = transform.localRotation.eulerAngles;
        }

        private void Update()
        {
            if (!GameManager.Instance.isAlive)
                return;
            if (!Enabled) return;
            float speed = new Vector3(player.velocity.x, 0, player.velocity.z).magnitude;
            Reset();
            if (speed > ToggleSpeed && player.isGrounded)
            {
                FinalPos += HeadBobMotion();
                FinalRot += new Vector3(-HeadBobMotion().z, 0, HeadBobMotion().x) * RotationMultipler * 10;
            }
            else if (speed > ToggleSpeed) FinalPos += HeadBobMotion() / 2f;

            if (Input.GetKeyDown(KeyCode.LeftShift)) AmountValue = Amount * SprintAmount;
            else if (Input.GetKeyUp(KeyCode.LeftShift)) AmountValue = Amount / SprintAmount;
            transform.localPosition = Vector3.Lerp(transform.localPosition, FinalPos, Smooth * Time.deltaTime);
            if (EnabledRotationMovement) transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(FinalRot), Smooth / 1.5f * Time.deltaTime);
            if (footstepCD < footstepCDBASE)
            {
                footstepCD -= Time.deltaTime;
                if (footstepCD < 0)
                    footstepCD = footstepCDBASE;
            }

        }

        private Vector3 HeadBobMotion()
        {
            Vector3 pos = Vector3.zero;
            if (Mathf.Abs(Mathf.Sin(Time.time * Frequency)) < 0.25f && footstepCD == footstepCDBASE)
            {
                footstepCD -= Time.deltaTime;
                audioSus.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length - 1)], Random.Range(0.5f, 0.7f));
            }
            pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * Frequency) * AmountValue * 2f, Smooth * Time.deltaTime);
            pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * Frequency / 2f) * AmountValue * 1.3f, Smooth * Time.deltaTime);
            return pos;
        }
        private void Reset()
        {
            if (transform.localPosition == StartPos) return;
            FinalPos = Vector3.Lerp(FinalPos, StartPos, 1 * Time.deltaTime);
            FinalRot = Vector3.Lerp(FinalRot, StartRot, 1 * Time.deltaTime);
        }
    }

}