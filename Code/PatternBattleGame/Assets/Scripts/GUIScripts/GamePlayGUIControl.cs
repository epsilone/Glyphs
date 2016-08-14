using JetBrains.Annotations;

using UnityEngine;

public class GamePlayGUIControl : MonoBehaviour
{
    public GameObject glyphIn;
    public GameObject mainGameGUI;
    public GameObject symbols;

    public float glyphZoomSpeed = 1f;

    [HideInInspector]
    public float scaleSmall = 1f;

    public float scaleLarge = 1.5f;

    [HideInInspector]
    public bool zoom;

    [HideInInspector]
    public bool shrink;

    [HideInInspector]
    public string state = "";

    public AudioClip zoomSound;
    public AudioClip shrinkSound;

    private AudioSource myAudioSource;

    [UsedImplicitly]
    private void Start()
    {
        myAudioSource = gameObject.AddComponent<AudioSource>();
        myAudioSource.volume = PlayerPrefs.GetInt("sound", 1);
    }

    [UsedImplicitly]
    private void Update()
    {
        ZoomLarge();
        ZoomSmall();
    }

    public void ZoomLarge()
    {
        if (!zoom)
        {
            return;
        }

        if (symbols.transform.localScale.x < scaleLarge)
        {
            Vector3 localScale = symbols.transform.localScale;
            localScale.x = symbols.transform.localScale.x + (glyphZoomSpeed * Time.deltaTime);
            localScale.y = symbols.transform.localScale.y + (glyphZoomSpeed * Time.deltaTime);

            symbols.transform.localScale = localScale;

            if (symbols.transform.localScale.x >= scaleLarge)
            {
                zoom = false;
                doneZooming();
            }
        }
    }

    public void ZoomSmall()
    {
        if (!shrink)
        {
            return;
        }

        if (symbols.transform.localScale.x > scaleSmall)
        {
            Vector3 localScale = symbols.transform.localScale;
            localScale.x = symbols.transform.localScale.x - (glyphZoomSpeed * Time.deltaTime);
            localScale.y = symbols.transform.localScale.y - (glyphZoomSpeed * Time.deltaTime);

            symbols.transform.localScale = localScale;

            if (symbols.transform.localScale.x >= scaleLarge)
            {
                shrink = false;
                doneShrinking();
            }
        }
    }

    public void doShrink(float delay)
    {
        Invoke("_doShrink", delay);
    }

    public void doZoom(float delay)
    {
        Invoke("_doZoom", delay);
    }

    public void _doZoom()
    {
        zoom = true;
        shrink = false;
        myAudioSource.PlayOneShot(zoomSound);
    }

    public void _doShrink()
    {
        shrink = true;
        zoom = false;
        myAudioSource.PlayOneShot(shrinkSound);
    }

    public void doneZooming()
    {
        Debug.Log("done zooming");
    }

    public void doneShrinking()
    {
        Debug.Log("done shrinking");
        gameObject.SendMessage("EndOfFlyIn");
    }

    public void TurnGlyphsAnimationControllerOff()
    {
        if (GetComponent<ObjectRotate>() != null)
            GetComponent<ObjectRotate>().lastTurn = true;
        var rotateScripts = symbols.GetComponentsInChildren<ObjectRotate>();
        for (var r = 0; r < rotateScripts.Length; r++)
        {
            ObjectRotate o = rotateScripts[r];
            if (o.rotationZ != 0)
                o.lastTurn = true;
        }
    }
    
    public void ForceStopRotating()
    {
        GetComponent<ObjectRotate>().rotationDisable = true;
        var rotateScripts = symbols.GetComponentsInChildren<ObjectRotate>();
        for (var r = 0; r < rotateScripts.Length; r++)
        {
            ObjectRotate o = rotateScripts[r];
            o.rotationDisable = true;
        }
    }

    public void _TurnGlyphsAnimationControllerOn()
    {
        glyphIn.GetComponent<ObjectRotate>().start_rotating();
        var rotateScripts = symbols.GetComponentsInChildren<ObjectRotate>();
        for (var r = 0; r < rotateScripts.Length; r++)
        {
            ObjectRotate o = rotateScripts[r];
            o.start_rotating();
        }
    }

    public void TurnGlyphsAnimationControllerOn(float delay)
    {
        Invoke("_TurnGlyphsAnimationControllerOn", delay);
    }
}