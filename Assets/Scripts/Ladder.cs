using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float climbSpeed = 3f;

    [Header("Ladder Position Settings")]
    [SerializeField] float topY;
    [SerializeField] float bottomY;
    [SerializeField] Vector3 bottomTeleportPosition;
    [SerializeField] Vector3 topTeleportPosition;
    [SerializeField] Vector3 playerOffset;

    [Header("References")]
    [SerializeField] PlayerController player;

    bool isOnLadder = false;
    bool hasClimbed = false;

    private void Update()
    {
        if (isOnLadder)
        {

            if (Input.GetKey(KeyCode.W))
            {
                player.transform.Translate(climbSpeed * Time.deltaTime * transform.up);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                player.transform.Translate(-climbSpeed * Time.deltaTime * transform.up);
            }

            //Make sure player doesn't immediately gets set off the ladder
            if (hasClimbed)
            {
                if (Mathf.Abs(player.transform.position.y - topY) <= 1f)
                {
                    GetOffLadder(topTeleportPosition);
                }
                else if (Mathf.Abs(player.transform.position.y - bottomY) <= 1f) 
                {
                    GetOffLadder(bottomTeleportPosition);
                }
            }
            else
            {
                if(Mathf.Abs(player.transform.position.y - topY) > 1.5f && Mathf.Abs(player.transform.position.y - bottomY) > 1.5f)
                {
                    hasClimbed = true;
                }
            }
        }
        else
        {
            CheckInteractWithLadder();
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

            if (horizontalDistance < 2.5f && dot > 0.75f)
            {
                if(Mathf.Abs(player.transform.position.y - topY) < Mathf.Abs(player.transform.position.y - bottomY))
                {
                    //Get on at top of the ladder
                    GetOnLadder(new Vector3(transform.position.x, topY, transform.position.z) + playerOffset);
                }
                else
                {
                    //Get on at the bottom of the ladder
                    GetOnLadder(new Vector3(transform.position.x, bottomY, transform.position.z) + playerOffset);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Red spheres are player teleport positions
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(topTeleportPosition, 0.5f);
        Gizmos.DrawSphere(bottomTeleportPosition, 0.5f);

        //Blue spheres are top and bottom of ladder
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(transform.position.x, topY, transform.position.z) + playerOffset, 0.5f);
        Gizmos.DrawSphere(new Vector3(transform.position.x, bottomY, transform.position.z) + playerOffset, 0.5f);
    }


}
