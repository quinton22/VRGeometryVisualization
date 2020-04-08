using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour
{
    public SteamVR_Action_Vector2 input;
    public float speed = 1;
    private Player player;
    private CharacterController characterController;
    // Start is called before the first frame update
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        player = gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = player.hmdTransform.TransformDirection(input.axis.x, 0, input.axis.y);
        characterController.Move(speed * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up) - (9.81f * Vector3.up * Time.deltaTime));
    }
}
