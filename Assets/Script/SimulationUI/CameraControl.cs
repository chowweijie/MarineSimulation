using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Vector3 currentPos;
    bool moveCamera = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveHorizontal();
        MoveVertical();
        PanCamera();
    }

    void MoveHorizontal()
    {
        Vector3 moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += new Vector3(1, 0, 0);
        }

        float moveSpeed = 10000f;
        transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.World);
    }

    void MoveVertical()
    {
        Vector3 scrollDir = new Vector3(0, 0, 0);
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float scrollSpeed = 100000f;
        scrollDir += new Vector3(0, -scroll, 0);
        transform.Translate(scrollDir.normalized * scrollSpeed * Time.deltaTime, Space.World);
    }

    void PanCamera()
    {
        float rotateSpeed = 100f;
        if (moveCamera == true)
        {
            if (Input.GetMouseButtonUp(1))
            {
                moveCamera = false;
            }
            Vector3 delta = Input.mousePosition - currentPos;
            // Debug.Log(delta);
            transform.Rotate(Vector3.up, delta.x * rotateSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -delta.y * rotateSpeed * Time.deltaTime, Space.Self);
            currentPos = Input.mousePosition;
        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                currentPos = Input.mousePosition;
                moveCamera = true;
                // Debug.Log("Moving Camera");
            }
        }
    }
}
