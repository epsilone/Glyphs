using UnityEngine;

public class OpenURLOnClick2 : MonoBehaviour
{

	public string url;

	void OnClick ()
	{

			if (!string.IsNullOrEmpty(url)) Application.OpenURL(url);

	}
}
