#pragma strict
 
var accelerometerUpdateInterval : float = 1.0 / 60.0;
 
// The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).
var lowPassKernelWidthInSeconds : float = 1.0;
 
// This next parameter is initialized to 2.0 per Apple's recommendation, or at least according to Brady! <img src="http://www.lucedigitale.com/blog/wp-includes/images/smilies/icon_wink.gif" alt=";)" class="wp-smiley"> 
var shakeDetectionThreshold : float = 2.0;

var messageTarget : GameObject;
var messageName : String = "OnShake";

var minimumTimeBetweenShakes : float = 1f;
  
private var lowPassFilterFactor : float = accelerometerUpdateInterval / lowPassKernelWidthInSeconds; 
private var lowPassValue : Vector3 = Vector3.zero;
private var acceleration : Vector3;
private var deltaAcceleration : Vector3;

private var lastShake : long;

 
function Start()
 
{
shakeDetectionThreshold *= shakeDetectionThreshold;
lowPassValue = Input.acceleration;
lastShake=Time.realtimeSinceStartup;
}
 
 
function Update()
{
 
acceleration = Input.acceleration;
lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
deltaAcceleration = acceleration - lowPassValue;
    if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
    {
    	if(Time.realtimeSinceStartup-lastShake>=minimumTimeBetweenShakes){
	    	Debug.Log("Shake Detected");
	    	
	    	if(messageTarget)
	        	messageTarget.SendMessage(messageName);
	        lastShake = Time.realtimeSinceStartup;
        }
    }
 
}