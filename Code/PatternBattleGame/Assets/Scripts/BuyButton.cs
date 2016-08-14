using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class BuyButton : MonoBehaviour {

	public string sku = "android.test.purchased";
	public string iosSku = "test";
	public string developerPayload="0"; // how many artifacts to reward
	public UILabel titleLabel;
	public UILabel priceLabel;
	public UILabel quantityLabel;
    public UILabel extraLabel;
	
	void Awake () {
		#if UNITY_ANDROID
		GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
		#endif
		#if UNITY_IOS
		StoreKitManager.productListReceivedEvent += productListReceivedEvent;
		#endif
	}
	
	void Start () {
		quantityLabel.text = developerPayload + " x";
	}

	public void ButtonClick(){
#if UNITY_ANDROID
		//GoogleIAB.consumeProduct(sku);
		GoogleIAB.purchaseProduct( sku,developerPayload );
#endif
#if UNITY_IOS
		StoreKitBinding.purchaseProduct( iosSku, 1 );
#endif

		var dict = new Dictionary<string,string>();
		dict.Add("sku",sku);
		dict.Add("currentBalance",SinglePlayerProgression.money.ToString());
		Analytics.Flurry.Instance.LogEvent("tryPurchase", dict);

	}

	#if UNITY_ANDROID
	public void queryInventorySucceededEvent( List<GooglePurchase> purchases, List<GoogleSkuInfo> skus )
	{

		Debug.Log("buybutton got queryinventorysucceededevent");
		foreach(GoogleSkuInfo m_sku in skus){
			if(m_sku.productId==sku){
				titleLabel.text=m_sku.title.Substring(0,m_sku.title.IndexOf("(Glyphs"));
				priceLabel.text=m_sku.price + " >";
				titleLabel.MarkAsChanged();
				priceLabel.MarkAsChanged();
				break;
			}
		}
	}

	public void queryInventoryFailedEvent( string error )
	{
		Debug.Log("queryInventoryFailed: " + error);
	}
	#endif
	#if UNITY_IOS
	void productListReceivedEvent( List<StoreKitProduct> productList )
	{
		Debug.Log("buybotton got productlistreceivedevent");

		foreach(StoreKitProduct product in productList){
			if(product.productIdentifier==iosSku){
				titleLabel.text=product.title.Substring(0,product.title.IndexOf("(Glyphs"));
				priceLabel.text=product.formattedPrice + " >";
				titleLabel.MarkAsChanged();
				priceLabel.MarkAsChanged();
				break;
			}
		}

	}
	#endif
}
