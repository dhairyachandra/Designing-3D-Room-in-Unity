using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CameraController : MonoBehaviour {
    [Header("Movement Parameters")]
    [Tooltip("Controls the mouse wheel camera speed")]
    public float moveSpeed = 10;
    [Tooltip("Controls how quickly the mouse pans")]
    public float panSpeed = 2.0f;
    [Tooltip("Check the invert boolean if you would like to invert the mouse controls")]
    public bool invertControls = false;
    [Space(5)]
    [Header("Camera Reset Parameters")]
    public float resetSpeed = 2.0f;
    public Transform resetLocation;
    public float resetDistance = 10f;

    public delegate void SelectionUpdate();
    public static event SelectionUpdate UpdateSelection;

    private Quaternion resetRot;

    private float x;
    private float y;
    private float wheel;

    private Vector3 rotateValue;
    private Vector3 mouseOrigin;

    [HideInInspector]
    public Transform selectedObj;

    


    private void Awake()
    {
        resetRot = transform.rotation;
    }

    //Returns the camera to the starting position
    IEnumerator ResetCamera(Vector3 newlocation)
    {
        float moveTime = 0;
        Vector3 startLoc = transform.position;
        Quaternion startRot = transform.rotation;

        while (moveTime < resetSpeed)
        {
            transform.position = Vector3.Lerp(startLoc, newlocation, (moveTime / resetSpeed));
            transform.rotation = Quaternion.Lerp(startRot, resetRot, (moveTime / resetSpeed));
            moveTime += Time.deltaTime;
            yield return null;
        }
    }

    //Focuses on the selected object
    IEnumerator CameraFocus ()
    {
        float moveTime = 0;
        Vector3 startLoc = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 adjustedPos = selectedObj.GetComponentInChildren<CameraPosition>().transform.position;
        Quaternion lookAtRot = selectedObj.GetComponentInChildren<CameraPosition>().transform.rotation;

        while (moveTime < resetSpeed)
        {
            transform.position = Vector3.Lerp(startLoc, adjustedPos, (moveTime / resetSpeed));
            transform.rotation = Quaternion.Lerp(startRot, lookAtRot, (moveTime / resetSpeed));
            moveTime += Time.deltaTime;
            yield return null;
        }

    }

    public void CallCamReset(Transform newLoc)
    {       
        StartCoroutine(ResetCamera(newLoc.position));
    }

    public void FocusOnObject ()
    {
        StartCoroutine(CameraFocus());
    }


    void Update()
    {
        y = Input.GetAxis("Mouse X");
        x = Input.GetAxis("Mouse Y");
        wheel = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit raycastHit = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                if (raycastHit.collider.tag == "Kitchen")
                {
                    selectedObj = raycastHit.collider.transform;
                    UpdateSelection();

                }
            }
        }

            //Moves camera with scroll wheel. First checks if invertControls is true.
            if (invertControls)
        {
            if (wheel > 0f)
            {
                transform.Translate(0, 0, -wheel * moveSpeed, Space.Self);              
            }
            else if (wheel < 0f)
            {
                transform.Translate(0, 0, -wheel * moveSpeed, Space.Self);
            }

        } else
        {
            if (wheel > 0f)
            {
                transform.Translate(0, 0, wheel * moveSpeed, Space.Self);
            } else if (wheel < 0f)
            {
                transform.Translate(0, 0, wheel * moveSpeed, Space.Self);
            }
        }

        //Rotates the camera with the right mouse button
        if (Input.GetMouseButton(1))
        {
            rotateValue = new Vector3(x, y * -1, 0);
            transform.eulerAngles = transform.eulerAngles - rotateValue;
        }

        //Pans the camera in, out, left and right with the middle mouse button
        #region
        if (Input.GetMouseButtonDown(2))
        {
            mouseOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
        Vector3 move = new Vector3(pos.x * panSpeed, 0, pos.y * panSpeed);

        transform.Translate(move, Space.Self);
        #endregion
    }


}
