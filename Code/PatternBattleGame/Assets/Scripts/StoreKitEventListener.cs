using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prime31;


public class StoreKitEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	
	private Dictionary<string, int> _products;

	[SerializeField]
	public string[] productIds;

	[SerializeField]
	public int[] artifactValues;

	void Start()
	{
		_products = new Dictionary<string,int>();

		for(int i=0; i<productIds.Length; i++)
			_products.Add(productIds[i], artifactValues[i]);

		StoreKitBinding.requestProductData( productIds );
	}

	void OnEnable()
	{
		// Listens to all the StoreKit events. All event listeners MUST be removed before this object is disposed!
		StoreKitManager.transactionUpdatedEvent += transactionUpdatedEvent;
		StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
		StoreKitManager.purchaseCancelledEvent += purchaseCancelledEvent;
		StoreKitManager.purchaseFailedEvent += purchaseFailedEvent;
		StoreKitManager.productListReceivedEvent += productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent += productListRequestFailedEvent;
		StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailedEvent;
		StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinishedEvent;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;
	}
	
	
	void OnDisable()
	{
		// Remove all the event handlers
		StoreKitManager.transactionUpdatedEvent -= transactionUpdatedEvent;
		StoreKitManager.productPurchaseAwaitingConfirmationEvent -= productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessfulEvent;
		StoreKitManager.purchaseCancelledEvent -= purchaseCancelledEvent;
		StoreKitManager.purchaseFailedEvent -= purchaseFailedEvent;
		StoreKitManager.productListReceivedEvent -= productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent -= productListRequestFailedEvent;
		StoreKitManager.restoreTransactionsFailedEvent -= restoreTransactionsFailedEvent;
		StoreKitManager.restoreTransactionsFinishedEvent -= restoreTransactionsFinishedEvent;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent -= paymentQueueUpdatedDownloadsEvent;
	}
	
	
	
	void transactionUpdatedEvent( StoreKitTransaction transaction )
	{
		Debug.Log( "transactionUpdatedEvent: " + transaction );
	}

	
	void productListReceivedEvent( List<StoreKitProduct> productList )
	{
		Debug.Log( "productListReceivedEvent. total products received: " + productList.Count );
		
		// print the products to the console
		foreach( StoreKitProduct product in productList )
			Debug.Log( product.ToString() + "\n" );
	}
	
	
	void productListRequestFailedEvent( string error )
	{
		Debug.Log( "productListRequestFailedEvent: " + error );
	}
	

	void purchaseFailedEvent( string error )
	{
		Debug.Log( "purchaseFailedEvent: " + error );
		Analytics.Flurry.Instance.LogError("purchaseFailed",error);
	}
	

	void purchaseCancelledEvent( string error )
	{
		Debug.Log( "purchaseCancelledEvent: " + error );
	}
	
	
	void productPurchaseAwaitingConfirmationEvent( StoreKitTransaction transaction )
	{
		Debug.Log( "productPurchaseAwaitingConfirmationEvent: " + transaction );
	}
	
	
	void purchaseSuccessfulEvent( StoreKitTransaction transaction )
	{
		Debug.Log( "purchaseSuccessfulEvent: " + transaction );
		SinglePlayerProgression.money+=_products[transaction.productIdentifier];

		var dict = new Dictionary<string,string>();
		dict.Add ("productId", transaction.productIdentifier.ToString());
		Analytics.Flurry.Instance.LogEvent("purchaseSucceeded",dict);
	}
	
	
	void restoreTransactionsFailedEvent( string error )
	{
		Debug.Log( "restoreTransactionsFailedEvent: " + error );
	}
	
	
	void restoreTransactionsFinishedEvent()
	{
		Debug.Log( "restoreTransactionsFinished" );
	}
	
	
	void paymentQueueUpdatedDownloadsEvent( List<StoreKitDownload> downloads )
	{
		Debug.Log( "paymentQueueUpdatedDownloadsEvent: " );
		foreach( var dl in downloads )
			Debug.Log( dl );
	}
	
#endif
}

