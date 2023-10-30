using Anathema.Game;
using UnityEngine;
using UnityEngine.AI;

namespace Anathema.Monster
{
    public class MonsterAI : MonoBehaviour
    {
        private NavMeshAgent agent;
        Vector3 randomDirection;
        Vector3 finalPosition;
        [SerializeField] Transform playerTransform;
        [SerializeField] private float walkMinRadius = 10, walkMaxRadius = 20;
        float agentSpeed;
        bool destinationReached = true;
        float idleTimer;
        [SerializeField] float currentIdleTimer;
        [SerializeField] float minIdleTime = 1, maxIdleTime = 3;

        [SerializeField] Light playerLamp;

        [SerializeField] Animator Anim;
        [SerializeField] AudioSource playerAudioSource;
        AudioReverbFilter reverbFilter;
        [SerializeField] AudioClip playerFound, playerDead;
        bool playerFoundSoundPlayed = false;
        bool wasHuntingPlayer = false;
        enum State
        {
            Idle,
            Walking,
            Hunting
        }
        State monsterState = State.Idle;
        void Start()
        {
            playerAudioSource = GetComponent<AudioSource>();
            reverbFilter = GetComponent<AudioReverbFilter>();
            agent = GetComponent<NavMeshAgent>();
            currentIdleTimer = idleTimer = 2f;
            agentSpeed = agent.speed;
        }

        void Update()
        {
            if (GameManager.Instance.GamePaused)
                return;
            Anim.SetFloat("velocity", agent.velocity.magnitude);

            if (playerLamp.range > 5)
            {
                CheckForPlayerLight();
            }
            else if (monsterState == State.Hunting)
            {
                //reverbFilter.enabled = false;
                playerFoundSoundPlayed = false;
                monsterState = State.Walking;
            }

            switch (monsterState)
            {
                case State.Idle:
                    if (idleTimer > 0 && !wasHuntingPlayer)
                    {
                        idleTimer -= Time.fixedDeltaTime;
                    }
                    else GoToRandomArea();
                    break;
                case State.Walking:
                    Walking();
                    break;
                case State.Hunting:
                    HuntingPlayer();
                    break;
            }

        }

        void SetIdleTimer()
        {
            currentIdleTimer = Random.Range(minIdleTime, maxIdleTime);
            idleTimer = currentIdleTimer;
        }
        void GoToRandomArea()
        {
            float radius;
            if (wasHuntingPlayer)
            {
                agent.speed = agentSpeed * 1.2f;
                radius = walkMaxRadius * 1.5f;
                wasHuntingPlayer = false;
            }
            else
            {
                agent.speed = agentSpeed;
                radius = Random.Range(walkMinRadius, walkMaxRadius);
            }
            randomDirection = Random.insideUnitSphere * radius;
            randomDirection += playerTransform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);
            finalPosition = hit.position;
            monsterState = State.Walking;
        }
        void Walking()
        {
            agent.SetDestination(finalPosition);
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                SetIdleTimer();
                monsterState = State.Idle;
            }
        }

        void CheckForPlayerLight()
        {
            RaycastHit rayHit;

            if (Physics.Linecast(transform.position, playerTransform.position, out rayHit))
            {
                if (rayHit.collider.tag == "GameController")
                {
                    if (!playerFoundSoundPlayed)
                    {
                        playerAudioSource.PlayOneShot(playerFound, 1.5f);
                        playerFoundSoundPlayed = true;
                    }
                    //reverbFilter.enabled = true;
                    monsterState = State.Hunting;
                }
            }
        }

        void HuntingPlayer()
        {
            agent.speed = agentSpeed;
            wasHuntingPlayer = true;
            finalPosition = playerTransform.position;
            agent.SetDestination(finalPosition);
        }

        public void StopAgent()
        {
            agent.SetDestination(transform.position);
            agent.ResetPath();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (monsterState == State.Hunting && other.tag == "GameController")
            {
                playerAudioSource.PlayOneShot(playerDead, 1.5f);
                GameManager.Instance.isAlive = false;
                GameManager.Instance.ReloadLevel();
            }
        }
    }
}
