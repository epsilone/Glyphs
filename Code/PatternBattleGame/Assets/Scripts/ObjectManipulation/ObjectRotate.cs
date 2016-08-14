using JetBrains.Annotations;

using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    public GameObject rotationObject;

    public float rotationX = 0f;
    public float rotationY = 0f;
    public float rotationZ = 0f;

    public float speedX = 0f;
    public float speedY = 0f;
    public float speedZ = 0f;

    [HideInInspector]
    public bool lastTurn;

    public bool rotationDisable = true;

    public GameObject plateHandler;

    [UsedImplicitly]
    private void Awake()
    {
        plateHandler = GameObject.Find("Playfield");
    }

    [UsedImplicitly]
    private void Update()
    {
        if(rotationDisable)
            return;

        bool turnCompleted = false;
        if (rotationX > 360)
        {
            rotationX = 0;
            turnCompleted = true;
        }
        if (rotationX < 0)
        {
            rotationX = 360;
            turnCompleted = true;
        }

        if (rotationY > 360)
        {
            rotationY = 0;
            turnCompleted = true;
        }
        if (rotationY < 0)
        {
            turnCompleted = true;
            rotationY = 360;
        }

        if (rotationZ > 360)
        {
            rotationZ = 0;
            turnCompleted = true;
        }
        if (rotationZ < 0)
        {
            rotationZ = 360;
            turnCompleted = true;
        }

        if (lastTurn && turnCompleted)
        {
            rotationDisable = true;

            
            if (plateHandler != null && GetComponent<GamePlayGUIControl>() != null)
            {
                plateHandler.SendMessage("doneRotating");
            }
            else
            {
                rotationX = rotationX + speedX;
                rotationY = rotationY + speedY;
                rotationZ = rotationZ + speedZ;
            }
            rotationObject.transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
        }
    }

    public void start_rotating()
    {
        lastTurn = false;
        rotationDisable = false;
    }


}
