using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpItem : Singleton<PickUpItem>
{
    public GameObject player;
    public Transform holdPos;
    public float pickUpRange = 5f; //how far the player can pickup the object from
    public GameObject heldObj { get; set; } //object which we pick up
    private Rigidbody heldObjRb; //rigidbody of object we pick up
    private bool canDrop = true;
    private int LayerNumber; //layer index
    RaycastHit hit;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("HoldLayer");

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) //change E to whichever key you want to press to pick up
        {
            if (heldObj == null) //if currently not holding anything
            {
                //perform raycast to check if player is looking at object within pickuprange
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    //make sure pickup tag is attached
                    if (hit.transform.gameObject.CompareTag("CanPickUp"))
                    {
                        //pass in object hit into the PickUpObject function
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                    {
                        if (hit.transform.gameObject.CompareTag("GameController"))
                        {
                            BurnObject();
                        }
                        else
                        {
                            StopClipping();
                            DropObject();//prevents object from clipping through walls
                        }
                        return;
                    }
                    DropObject();
                }
            }
        }
        if (heldObj != null) //if player is holding object
        {
            MoveObject();
        }
    }
    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            heldObj.transform.rotation = holdPos.rotation;
            heldObj.layer = LayerNumber; //change the object layer to the holdLayer
        }
    }
    void DropObject()
    {
        heldObj.layer = 0; //object assigned back to default layer
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; //unparent object
        heldObj = null; //undefine game object
    }
    void MoveObject()
    {
        //keep object position the same as the holdPosition position
        heldObj.transform.position = holdPos.transform.position;
    }
   
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }
    void BurnObject()
    {
        Debug.Log("Burned");
        StartCoroutine(GameManager.Instance.Incinerating(heldObj));
        heldObj.layer = 0;
        heldObj.transform.parent = null;
        heldObj = null;
    }
}
