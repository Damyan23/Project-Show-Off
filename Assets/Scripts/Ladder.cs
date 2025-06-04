using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float climbSpeed = 3f;

    [Header("Ladder Position Settings")]
    [SerializeField] Vector3 bottomLadderPositionOffset;
    [SerializeField] Vector3 topLadderPositionOffset;

    [Header("Teleport Settings")]
    [SerializeField] Vector3 bottomTeleportPositionOffset;
    [SerializeField] Vector3 topTeleportPositionOffset;

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
        upDirection = Vector3.Normalize(topLadderPositionOffset - bottomLadderPositionOffset);

        topLadderPosition = transform.position + topLadderPositionOffset;
        bottomLadderPosition = transform.position + bottomLadderPositionOffset;

        topTeleportPosition = transform.position + topTeleportPositionOffset;
        bottomTeleportPosition = transform.position + bottomTeleportPositionOffset;
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
        if (Input.GetKey(KeyCode.W))
        {
            player.transform.Translate(climbSpeed * Time.deltaTime * upDirection);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            player.transform.Translate(-climbSpeed * Time.deltaTime * upDirection);
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
    }

    void GetOnLadder(Vector3 newPosition)
    {
        isOnLadder = true;
        player.rb.velocity = Vector3.zero;

        player.transform.position = newPosition;

        player.TogglePlayerMovement(false);
        player.rb.useGravity = false;
    }

    void CheckInteractWithLadder()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
            float horizontalDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), playerPos);

            //Only get on ladder if player look at it
            Vector2 playerLookDirection = new Vector2(player.transform.forward.x, player.transform.forward.z);
            Vector2 ladderDirection = new Vector2(transform.position.x - player.transform.position.x, transform.position.z - player.transform.position.z).normalized;
            float dot = Vector2.Dot(playerLookDirection, ladderDirection);

            if (horizontalDistance < 5f && dot > 0.75f)
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

    private void OnDrawGizmosSelected()
    {
        upDirection = Vector3.Normalize(topLadderPositionOffset - bottomLadderPositionOffset);

        topLadderPosition = transform.position + topLadderPositionOffset;
        bottomLadderPosition = transform.position + bottomLadderPositionOffset;

        topTeleportPosition = transform.position + topTeleportPositionOffset;
        bottomTeleportPosition = transform.position + bottomTeleportPositionOffset;


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
