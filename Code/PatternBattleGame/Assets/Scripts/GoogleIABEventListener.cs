using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GoogleIABEventListener : MonoBehaviour
{
#if UNITY_ANDROID
	void Awake(){
		//GoogleIAB.init( "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjGAWZ+1vvwo3PSmgLSwLXkeWnIRdAeUyz1nPU1oqHJd7yT2dSXYuU3PHEUVOKxeLJdnJImbEbu6KyLpa9l/oMmGn6nfmmucf842s9k57DpJz+MnXmpz9xURf0Jo4Kxh3Wzyk+7fx5B9koXc3K/Tw0NatcD4/xOmg0nRaztakV/7cT8Ef1SDXjAlpbmv+9xNnUx8QMC6GGjwjaaAuiEx0tBniFHvOQ2H+tmJdzE4UDI9a0awCskX9ihc+fr6ga/RSyvNglMZWPeKJqMb3Q3SigGL9+BjTalFR2HzjVweCQW0xaLvKWsVwVDYfq3H/SS58dJ2yqFQCMXE3rlS8gKlABQIDAQAB" );
		GoogleIAB.init( "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3iZocEzZOUOwYkGb2rATHynfyjOdkQ8hQVwDSzYVOO/eAAf2iiWnzDP8vmBR4FgIK/nmrtg5sLB4x7ZzDJakQU/zvRARTyRM85HW8B5oEW7GPnSQUg6tXuWxOrnvzPsXRy1x3qC1X4VCtca6HM4mcG9+1p9cL4mxQ5bHbr1hqVW13P/0hi1DxReRaXDD4Erwv0w+Xek4tX9r7mUHyn2Ckcvvql1z030WC/a8mQVSQijzybk9jvBm1Zm5wo4FtnqBmTxiuy+2QdVOYByCgjrBfjMvQ5T2W1ABgj+Sxem3XgeaLvphL5upCVyemwFWssLD0G/K8R8iXQJZf4j08gMv2QIDAQAB" );
		DontDestroyOnLoad(gameObject);
	}

	void Start(){
		GoogleIAB.queryInventory( new string[6]{"small_artifacts","medium_artifacts","large_artifacts","massive_artifacts","extralarge_artifacts","supermassive_artifacts"} );
	}

//	void OnLevelWasLoaded(int lvl)
//	{
//		if(Application.loadedLevelName!="LoadingScreen")
//			GoogleIAB.queryInventory( new string[6]{"small_artifacts","medium_artifacts","large_artifacts","massive_artifacts","extralarge_artifacts","supermassive_artifacts"} );
//	}

	void OnEnable()
	{
		// Listen to all events for illustration purposes
		GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
		GoogleIABManager.purchaseFailedEvent -= purchaseFailedEvent;
		GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}



	void billingSupportedEvent()
	{
		Debug.Log( "billingSupportedEvent" );
	}


	void billingNotSupportedEvent( string error )
	{
		Debug.Log( "billingNotSupportedEvent: " + error );
		Analytics.Flurry.Instance.LogError("billingNotSupported",error);
	}


	void queryInventorySucceededEvent( List<GooglePurchase> purchases, List<GoogleSkuInfo> skus )
	{
		Debug.Log( string.Format( "queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count ) );
		foreach(GooglePurchase purchase in purchases){
			if(purchase.purchaseState==GooglePurchase.GooglePurchaseState.Purchased)
				GoogleIAB.consumeProduct(purchase.productId);
		}
		Prime31.Utils.logObject( purchases );
		Prime31.Utils.logObject( skus );
	}


	void queryInventoryFailedEvent( string error )
	{
		Debug.Log( "queryInventoryFailedEvent: " + error );
	}


	void purchaseCompleteAwaitingVerificationEvent( string purchaseData, string signature )
	{
		Debug.Log( "purchaseCompleteAwaitingVerificationEvent. purchaseData: " + purchaseData + ", signature: " + signature );
	}
	

	void purchaseSucceededEvent( GooglePurchase purchase )
	{
		Debug.Log( "purchaseSucceededEvent: " + purchase );
		SinglePlayerProgression.money+=int.Parse(purchase.developerPayload);
		GoogleIAB.consumeProduct(purchase.productId);
			var dict = new Dictionary<string,string>();
			dict.Add ("productId", purchase.productId.ToString());
		Analytics.Flurry.Instance.LogEvent("purchaseSucceeded",dict);
	}


	void purchaseFailedEvent( string error, int errNum )
	{
		Debug.Log( "purchaseFailedEvent: " + error );
		Analytics.Flurry.Instance.LogError("purchaseFailed",error);
	}


	void consumePurchaseSucceededEvent( GooglePurchase purchase )
	{
		Debug.Log( "consumePurchaseSucceededEvent: " + purchase );
	}


	void consumePurchaseFailedEvent( string error )
	{
		Debug.Log( "consumePurchaseFailedEvent: " + error );
	}


#endif
}


