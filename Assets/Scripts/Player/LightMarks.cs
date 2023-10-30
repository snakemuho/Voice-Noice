using UnityEngine;

namespace Anathema.Player
{
    public class LightMarks : MonoBehaviour
    {
        PlayerController player;
        [SerializeField] GameObject lightMark;
        [SerializeField] float maxMarkTimer = 4;
        [SerializeField] Light lampLight;
        float markTimer;
        void Start()
        {
            markTimer = maxMarkTimer;
            player = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (markTimer < maxMarkTimer)
            {
                markTimer -= Time.fixedDeltaTime;
                if (markTimer <= 0) markTimer = maxMarkTimer;
            }

            if (player.WallDistance && markTimer == maxMarkTimer && lampLight.range >= 5) LeaveAMark(player.ObjectCheck);
        }
        public void LeaveAMark(RaycastHit hitWall)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Instantiate(lightMark, Vector3.Lerp(transform.position + transform.up * 0.25f, hitWall.point, 0.9f), Quaternion.identity);
                markTimer -= Time.fixedDeltaTime;
            }
        }
    }
}
