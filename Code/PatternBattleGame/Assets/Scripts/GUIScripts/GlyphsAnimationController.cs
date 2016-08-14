using JetBrains.Annotations;

using UnityEngine;

public class GlyphsAnimationController : MonoBehaviour
{
    public GameObject[] glyphObjects;
    public GameObject[] glyphPositions;

    public bool[] glyphMoveFlag;
    public bool[] glyphMoveFlagStarted;

    public bool[] flyInComplete;
    public bool[] flyOutComplete;

    public float circleRadius = 1f;
    public float startScaleSize = 1f;

    [HideInInspector]
    public float endScaleSize = 1f;

    public float scaleRatioToDistance = 0.1f;
    public float moveXSpeed = 10f;

    public bool flyInFinished = true;
    public bool flyOutFinished = true;

    public GameObject[] commenceTransButtons;

    public int numGlyphs = 5;

    public GameObject plateHandler;

    public int offsetAngle;

    [UsedImplicitly]
    private void Start()
    {
        endScaleSize = glyphObjects[0].transform.localScale.x;

        float startDistance = Vector3.Distance(
            glyphObjects[0].transform.position,
            gameObject.GetComponent<ObjectRotate>().rotationObject.transform.position);

        float distanceTravelled = startDistance - circleRadius;

        scaleRatioToDistance = endScaleSize / circleRadius;

        for (int i = 0; i < glyphObjects.Length; ++i)
        {
            glyphObjects[i].transform.position = glyphPositions[i].transform.position;
        }
    }

    [UsedImplicitly]
    private void Update()
    {
        FlyIn();
        FlyOut();
    }

    public void setNumGlyphs(int num)
    {
        numGlyphs = num;
    }

    public void FlyIn()
    {
        if (flyInFinished)
        {
            return;
        }

        for (int i = 0; i < numGlyphs; ++i)
        {
            if (glyphMoveFlag[i] == false && glyphMoveFlagStarted[i] == false)
            {
                glyphObjects[i].transform.position = glyphPositions[i].transform.position;
            }

            float spacingAngle = 360f / numGlyphs;

            if (numGlyphs == 4)
                offsetAngle = 75;
            else if (numGlyphs == 3)
                offsetAngle = 45;
            else if (numGlyphs == 5)
                offsetAngle = 90;

            //Tell them to start moving equally spaced
            if (Mathf.Abs(gameObject.GetComponent<ObjectRotate>().rotationZ - ((i * spacingAngle) + offsetAngle) % 360) < 2f)
            {
                glyphMoveFlagStarted[i] = true;
                glyphMoveFlag[i] = true;
            }

            //Move all objects in who didn't reach their end position yet and scale them
            if (glyphMoveFlag[i] == true && flyInComplete[i] == false)
            {

                glyphObjects[i].transform.localPosition =
                    Vector3.MoveTowards(glyphObjects[i].transform.localPosition,
                                    gameObject.GetComponent<ObjectRotate>().rotationObject.transform.position, moveXSpeed * Time.deltaTime);
                //Scale Them according to distance
                var CurrentDistance = Vector3.Distance(glyphObjects[i].transform.position,
                                                         gameObject.GetComponent<ObjectRotate>().rotationObject.transform.position);

                Vector3 localScale = glyphObjects[i].transform.localScale;
                localScale.x = scaleRatioToDistance * CurrentDistance;
                localScale.y = scaleRatioToDistance * CurrentDistance;

                glyphObjects[i].transform.localScale = localScale;

            }

            if (Vector3.Distance(glyphObjects[i].transform.position,
                                 gameObject.GetComponent<ObjectRotate>().rotationObject.transform.position) < circleRadius)
            {
                //TheFlying for the object is complete
                flyInComplete[i] = true;
                //Debug.Log(Vector3.Distance(GlyphObjects[i].transform.position,
                //				gameObject.GetComponent<ObjectRotate>().RotationObject.transform.position));
            }
        }

        int numComplete = 0;
        for (var j = 0; j < flyInComplete.Length; j++)
            if (flyInComplete[j]) numComplete++;

        //	Debug.Log(numComplete);

        if (numComplete == numGlyphs)
        {
            flyInFinished = true;
            // Check if the last Fly in is complete:
            for (var f = 0; f < numGlyphs; f++)
            {
                flyInFinished = flyInFinished && flyInComplete[f];
            }

            if (flyInFinished)
            {
                EndOfFlyIn();
            }
        }
    }

    public void FlyOut()
    {
        if (flyOutFinished)
        {
            // so that we don't evaluate them all every frame.
            return;
        }
        //Fly out
        for (var i = 0; i < numGlyphs; i++)
        {

            float spacingAngle = 360f / numGlyphs;


            if (Mathf.Abs(gameObject.GetComponent<ObjectRotate>().rotationZ - (((i * spacingAngle) + offsetAngle + 180) % 360)) < 2f)
            {
                glyphMoveFlagStarted[i] = true;
                glyphMoveFlag[i] = true;
            }

            //Move all objects in who didn't reach their end position yet and scale them
            if (glyphMoveFlag[i] == true && flyOutComplete[i] == false)
            {

                glyphObjects[i].transform.position =
                    Vector3.MoveTowards(glyphObjects[i].transform.position, gameObject.GetComponent<ObjectRotate>().rotationObject.transform.position + glyphObjects[i].transform.position * 2f, Time.deltaTime);
                //Scale Them according to distance
                var CurrentDistance = Vector3.Distance(glyphObjects[i].transform.position,
                                                         gameObject.GetComponent<ObjectRotate>().rotationObject.transform.position);

                Vector3 localScale = glyphObjects[i].transform.localScale;
                localScale.x = scaleRatioToDistance * CurrentDistance;
                localScale.y = scaleRatioToDistance * CurrentDistance;
                glyphObjects[i].transform.localScale = localScale;

                var targetDistance = Vector3.Distance(glyphPositions[i].transform.position, gameObject.GetComponent<ObjectRotate>().rotationObject.transform.position);
                if (CurrentDistance > targetDistance)
                {
                    flyOutComplete[i] = true;
                }
            }


            int numComplete = 0;
            for (var j = 0; j < flyOutComplete.Length; j++)
                if (flyOutComplete[j]) numComplete++;

            if (numComplete == numGlyphs)
            {
                flyOutFinished = true;
                // Check if the last Fly in is complete:
                for (var f = 0; f < numGlyphs; f++)
                {
                    flyOutFinished = flyOutFinished && flyOutComplete[f];
                }

            }

            if (flyOutFinished)
            {
                //			for (var h=0;h < numGlyphs;h++){
                //				GlyphObjects[h].transform.position = GlyphPositions[h].transform.position;
                //			}
                //	doFlyIn();
                Debug.Log("flyout complete");
                //			SendMessage("TurnGlyphsAnimationControllerOff",0f);
                //			SendMessage("ForceStopRotating");
                plateHandler.SendMessage("fadeOutCompletely");
                ShowButtons();
            }
        }

    }

    public void EndOfFlyIn()
    {
        Debug.Log("The fly in is complete ");

        //var guicontrol = this.GetComponent(GamePlayGUIControl);
        //guicontrol.TurnGlyphsAnimationControllerOff();

        //Turn on the commence tranmission button
        for (var i = 0; i < commenceTransButtons.Length; i++)
        {
            if (commenceTransButtons[i])
            {
                commenceTransButtons[i].SetActive(true);
            }
        }


        //doFlyOut();
    }

    public void HideButtons()
    {
        for (var i = 0; i < commenceTransButtons.Length; i++)
        {
            if (commenceTransButtons[i])
            {
                commenceTransButtons[i].SetActive(false);
            }
        }
    }

    public void ShowButtons()
    {
        for (var i = 0; i < commenceTransButtons.Length; i++)
        {
            if (commenceTransButtons[i])
            {
                commenceTransButtons[i].SetActive(true);
            }
        }
    }


    public void doFlyIn()
    {
        for (var i = 0; i < numGlyphs; i++)
        {
            glyphMoveFlagStarted[i] = false;
            glyphMoveFlag[i] = false;
            flyInComplete[i] = false;
        }
        flyInFinished = false;
        flyOutFinished = true;
        plateHandler.SendMessage("fadeOut");
    }


    public void doFlyOut()
    {
        flyInFinished = true;
        for (var i = 0; i < glyphObjects.Length; i++)
        {
            glyphMoveFlagStarted[i] = false;
            glyphMoveFlag[i] = false;
            flyOutComplete[i] = false;
            flyInComplete[i] = true;
        }
        flyOutFinished = false;
        flyInFinished = true;
    }

    public void doFlyOutDelay(float delay)
    {
        HideButtons();
        Invoke("doFlyOut", delay);
    }
}
