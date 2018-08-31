using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] public float maxSpeed = 7;
    protected Vector2 targetVelocity;
    protected Rigidbody2D rigidBody2D;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    private bool fireIsPressed = false;

    private void Awake() {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate() {
        Vector2 velocityX = new Vector2(); ;
        Vector2 velocityY = new Vector2(); ;
        velocityX.x= targetVelocity.x;
        velocityY.y = targetVelocity.y;
        Vector2 deltaPositionX = velocityX * Time.deltaTime;
        Movement(deltaPositionX, true);
        Vector2 deltaPositionY = velocityY * Time.deltaTime;
        Movement(deltaPositionY, true);
    }
    void Update() {
        ComputeVelocity();
        manageInteraction();
    }
    protected  void ComputeVelocity() {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");
        targetVelocity = move.normalized * maxSpeed;
    }
    void Movement(Vector2 move, bool yMovement) {
        float distance = move.magnitude;

        if (distance > minMoveDistance) {
            int count = rigidBody2D.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }
 
            for (int i = 0; i < hitBufferList.Count; i++) {
                Vector2 currentNormal = hitBufferList[i].normal;
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rigidBody2D.position = rigidBody2D.position + move.normalized * distance;
    }

    private void manageInteraction()
    {
        if (Input.GetAxis("Jump") != 0)
        {
            if (!fireIsPressed)
            {
                print("fire");
                fireIsPressed = true;
                ContactFilter2D contactFilter2DInteraction = new ContactFilter2D();
                contactFilter2DInteraction.useTriggers = false;
                contactFilter2DInteraction.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Interaction")));
                contactFilter2DInteraction.useLayerMask = true;
                RaycastHit2D[] interactionHit = new RaycastHit2D[16];
                List<RaycastHit2D> hitBufferListInteration = new List<RaycastHit2D>();
                hitBufferListInteration.Clear();
                int count = Physics2D.Raycast(gameObject.transform.position, Vector2.up, contactFilter2DInteraction, interactionHit);
                for (int i = 0; i < count; i++)
                {
                    hitBufferListInteration.Add(interactionHit[i]);
                }
                if (hitBufferListInteration.Count > 0)
                {
                    hitBufferListInteration[0].transform.gameObject.GetComponent<Interaction>().Interact();
                }
            }
            else
            {
                fireIsPressed = false;
            }
        }
    }
}
