using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float climbSpeed = 3f;

    [Header("Ladder Position Settings")]
    [SerializeField] Transform bottomLadderT;
    [SerializeField] Transform topLadderT;

    [Header("Teleport Settings")]
    [SerializeField] Transform bottomTeleportT;
    [SerializeField] Transform topTeleportT;

    [Header("References")]
    [SerializeField] PlayerController player;

    Vector3 topLadderPosition;
    Vector3 bottomLadderPosition;

    Vector3 topTeleportPosition;
    Vector3 bottomTeleportPosition;

    Vector3 upDirection;

    bool isOnLadder = false;
    bool hasClimbed = false;

    private void Start()
    {
        upDirection = Vector3.Normalize(topLadderT.position - bottomLadderT.position);

        topLadderPosition = topLadderT.position;
        bottomLadderPosition = bottomLadderT.position;

        topTeleportPosition = topTeleportT.position;
        bottomTeleportPosition = bottomTeleportT.position;
    }

    private void Update()
    {
        if (isOnLadder)
        {
            Move();
            if (hasClimbed)
            {
                CheckGetOffLadder();
            }
            else
            {
                CheckHasClimbed();
            }

        }
        else
        {
            CheckInteractWithLadder();
        }
    }

    void Move()
    {
        if (hasClimbed)
        {
            if (Input.GetKey(KeyCode.W))
            {
                player.transform.Translate(climbSpeed * Time.deltaTime * upDirection, Space.World);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                player.transform.Translate(-climbSpeed * Time.deltaTime * upDirection, Space.World);
            }
        }
        else
        {
            //If player is at top
            if(Vector3.Distance(player.transform.position, topLadderPosition) < Vector3.Distance(player.transform.position, bottomLadderPosition))
            {
                if (Input.GetKey(KeyCode.S))
                {
                    player.transform.Translate(-climbSpeed * Time.deltaTime * upDirection, Space.World);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    player.transform.Translate(climbSpeed * Time.deltaTime * upDirection, Space.World);
                }
            }
        }
    }

    void CheckHasClimbed()
    {
        if (hasClimbed) return;

        if (Vector3.Distance(player.transform.position, topLadderPosition)    > 1.5f &&
            Vector3.Distance(player.transform.position, bottomLadderPosition) > 1.5f)
        {
            hasClimbed = true;
        }
    }

    void CheckGetOffLadder()
    {
        //Make sure player doesn't immediately gets set off the ladder
        if (hasClimbed)
        {
            if (Vector3.Distance(player.transform.position, topLadderPosition) <= 0.5f)
            {
                GetOffLadder(topTeleportPosition);
            }
            else if (Vector3.Distance(player.transform.position, bottomLadderPosition) <= 0.5f)
            {
                GetOffLadder(bottomTeleportPosition);
            }
        }
    }

    void GetOffLadder(Vector3 newPosition)
    {
        player.transform.position = newPosition;
        isOnLadder = false;
        hasClimbed = false;
        player.TogglePlayerMovement(true);
        player.rb.useGravity = true;
        player.GetComponent<CapsuleCollider>().enabled = true;
    }

    void GetOnLadder(Vector3 newPosition)
    {
        isOnLadder = true;
        player.rb.velocity = Vector3.zero;

        player.transform.position = newPosition;

        player.TogglePlayerMovement(false);
        player.rb.useGravity = false;
        player.GetComponent<CapsuleCollider>().enabled = false;
    }

    void CheckInteractWithLadder()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
            Vector2 topLadderXZ = new Vector2(topLadderPosition.x, topLadderPosition.z);
            Vector2 bottomLadderXZ = new Vector2(bottomLadderPosition.x, bottomLadderPosition.z);

            float topDst = Vector3.Distance(playerPos, topLadderXZ);
            float bottomDst = Vector3.Distance(playerPos, bottomLadderXZ);
            Vector3 ladderPos = topDst < bottomDst ? topLadderPosition : bottomLadderPosition;

            float horizontalDistance = Vector2.Distance(new Vector2(ladderPos.x, ladderPos.z), playerPos);

            //Only get on ladder if player look at it
            Vector2 playerLookDirection = new Vector2(player.transform.forward.x, player.transform.forward.z);
            Vector2 ladderDirection = new Vector2(transform.position.x - player.transform.position.x, transform.position.z - player.transform.position.z).normalized;
            float dot = Vector2.Dot(playerLookDirection, ladderDirection);

            if (horizontalDistance < 7.5f && dot > 0.75f)
            {
                if (Vector3.Distance(player.transform.position, topLadderPosition) < Vector3.Distance(player.transform.position, bottomLadderPosition))
                {
                    //Get on at top of the ladder
                    GetOnLadder(topLadderPosition);
                }
                else
                {
                    //Get on at the bottom of the ladder
                    GetOnLadder(bottomLadderPosition);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        upDirection = Vector3.Normalize(topLadderT.position - bottomLadderT.position);

        topLadderPosition = topLadderT.position;
        bottomLadderPosition = bottomLadderT.position;

        topTeleportPosition = topTeleportT.position;
        bottomTeleportPosition = bottomTeleportT.position;


        //Red spheres are player teleport positions
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(topTeleportPosition, 0.25f);
        Gizmos.DrawSphere(bottomTeleportPosition, 0.25f);

        //Blue spheres are top and bottom of ladder
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(topLadderPosition, 0.25f);
        Gizmos.DrawSphere(bottomLadderPosition, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(bottomLadderPosition, topLadderPosition);

    }


}
