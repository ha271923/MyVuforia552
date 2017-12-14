using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeDCursor : MonoBehaviour {

	private static int PointerIcon_TYPE_NULL = 0;
	private static int PointerIcon_TYPE_ARROW = 1000;

	public Camera SceneMainCamera;	//請放入中間的Camera,位置應該是在左右Camera的中間
	public GameObject ThreeDCursorObject;
	public float screenUIDepth = 40;	//實際上顯示的東西所在的深度

	//private bool isFoundObject = false;
	private bool enableDisable = true;
	private float cursorXLimit = 18.2f;	//當顯示的東西在Camera前50單位處(這邊等同於Z=50),3D cursor在scale=0.5時,
	private float cursorYLimit = 8.5f;	//3D cursor的X最多可在+-18.2被Camera看到,Y則是+-8.5,若顯示的東西更遠或更近(Z>50 or Z<50)則會讓cursorXYLimit更大或更小
	private float twoCameraDisScale = 0.2f;	//用來調整左右眼Camera的距離
	private float defaultUIDepth = 50;	//預設顯示的東西深度是在50的地方(Z=50)
	private Vector3 cursorTempPosition;
	private Vector3 cursorPlaneClickPosition;	//這是放cursor真正點擊的座標,unity中物件的座標是在正中間,但cursor點擊的位置是在cursor圖的左上角
	private Vector3 cursorSourceSize = new Vector3(0.075f, 0.075f, 0.075f);	//和cursorXLimit同樣意思,在Z=50時3D cursor大小預設在0.5,更遠或更近時需動態調整大小
	private Vector3 fixSystemCursorPosTo3DCursorPos = new Vector3 (1280, 360, 0);	//in Lumus 3D mode, resolution is 2560x720
	//private string foundObjectName;
	private Ray mRay;
	private RaycastHit hit;

	//private float tempRotate;

	//private ScanBackgroungState mScanBackgroungState;

	public void FarCamera()
	{
		GameObject.Find ("LeftEye").GetComponent<Transform> ().localPosition -= (Vector3.right * twoCameraDisScale);
		GameObject.Find("RightEye").GetComponent<Transform> ().localPosition += (Vector3.right * twoCameraDisScale);
		Debug.Log ("2 Camera position : Left " + GameObject.Find ("LeftEye").GetComponent<Transform> ().localPosition.x + " Right " + GameObject.Find ("RightEye").GetComponent<Transform> ().localPosition.x);
	}

	public void NearCamera()
	{
		GameObject.Find ("LeftEye").GetComponent<Transform> ().localPosition += (Vector3.right * twoCameraDisScale);
		GameObject.Find("RightEye").GetComponent<Transform> ().localPosition -= (Vector3.right * twoCameraDisScale);
		Debug.Log ("2 Camera position : Left " + GameObject.Find ("LeftEye").GetComponent<Transform> ().localPosition.x + " Right " + GameObject.Find ("RightEye").GetComponent<Transform> ().localPosition.x);
	}

	//show/hide system cursor
	public void EnableDisableSystemCursor()
	{
		enableDisable = !enableDisable;

		AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		var window = _activity.Call<AndroidJavaObject> ("getWindow");
		var rootView = window.Call<AndroidJavaObject> ("getDecorView");
		using (var PointerIcon = new AndroidJavaClass("android.view.PointerIcon"))
		{
			if (enableDisable)
			{
				using (var icon = PointerIcon.CallStatic<AndroidJavaObject>("getSystemIcon", _activity, PointerIcon_TYPE_ARROW))
				{
					rootView.Call("setPointerIcon", icon);
				}
			}
			else
			{
				using (var icon = PointerIcon.CallStatic<AndroidJavaObject>("getSystemIcon", _activity, PointerIcon_TYPE_NULL))
				{
					rootView.Call("setPointerIcon", icon);
				}
			}
		}
	}

	/*public void FindVuforiaTarget(bool isFound, string objectName)
	{
		isFoundObject = isFound;
		foundObjectName = objectName;
	}*/

	void Start()
	{
		//mScanBackgroungState = GameObject.Find ("ARCamera").GetComponent<ScanBackgroungState>();
		//EnableDisableSystemCursor();
		HideSystemBar();
	}

	void Update()
	{
		cursorTempPosition = Input.mousePosition;	//取得System Cursor的座標
		cursorTempPosition -= fixSystemCursorPosTo3DCursorPos;	//先減掉差值來讓Cursor置中
		cursorTempPosition.z = screenUIDepth;	//取得要顯示的東西所放的深度(也就是Z)

		/*if(isFoundObject && !mScanBackgroungState.IsForeScreenBusy) {
			tempRotate = (Mathf.Abs (GameObject.Find (foundObjectName).GetComponent<Transform> ().localEulerAngles.y - 90) / 4) + 1.5f;
			cursorTempPosition.z = (GameObject.Find (foundObjectName).GetComponent<Transform> ().position.y * -1) - tempRotate;
		} else {
			cursorTempPosition.z = screenUIDepth - 1;
		}*/
		//依據Z來動態調整Cursor的Scale和XY座標的極限,假設Cursor有越來越近的狀況那Scale和座標的極限要越來越小,越來越遠的話則相反
		ThreeDCursorObject.GetComponent<Transform> ().localScale = (cursorTempPosition.z / screenUIDepth) * cursorSourceSize;
		cursorTempPosition.x /= (fixSystemCursorPosTo3DCursorPos.x / ((cursorXLimit - (defaultUIDepth - cursorTempPosition.z) * 0.3f)));
		cursorTempPosition.y /= (defaultUIDepth / cursorTempPosition.z) * (fixSystemCursorPosTo3DCursorPos.y / cursorYLimit);
		
		//cursorTempPosition.y /= (defaultUIDepth * fixSystemCursorPosTo3DCursorPos.y) / (cursorTempPosition.z * cursorYLimit);
		//先shift一個值來讓cursor的圖座標是正確的,不然cursor的圖移動到最左邊或最右邊時,會看到是整張cursor圖的中心點碰到最邊而不是左上角那個點碰到最邊
		//這個shift值仍在調整中
		cursorTempPosition.x += ThreeDCursorObject.GetComponent<Transform> ().localScale.x / 2;
		cursorTempPosition.y -= ThreeDCursorObject.GetComponent<Transform> ().localScale.y / 2;

		//將算好的座標套在3D物件上,那著物件就是我們的3D Cusor
		ThreeDCursorObject.GetComponent<Transform> ().localPosition = cursorTempPosition;
		//接著在shift一個值來讓click的點是cursor圖的左上角那個點,這個shift值仍在調整中
		cursorPlaneClickPosition = ThreeDCursorObject.GetComponent<Transform> ().position;
		cursorPlaneClickPosition.x -= 0.4f;
		cursorPlaneClickPosition.y -= 0.6f;

		//Debug.Log("Wine " + GameObject.Find ("WineCylinderTarget3").GetComponent<Transform> ().position);
		//Debug.Log ("TestCursor : " + ThreeDCursorObject.GetComponent<Transform> ().localPosition);
		//Debug.Log ("System cursor " + Input.mousePosition);

		//從3D Cursor反推回去實際上點到的螢幕位置,接著在依那個螢幕位置打射線到Unity世界中以判斷是否有打到button或UI之類的,藉此控制場景內的物件
		mRay = SceneMainCamera.ScreenPointToRay (SceneMainCamera.WorldToScreenPoint (cursorPlaneClickPosition));
		if (Physics.Raycast (mRay, out hit)) {
			//Debug.Log ("Hit : " + hit.collider.name);
			if (Input.GetMouseButtonDown (0)) {
				if (hit.collider.GetComponent<Button> () != null) {
					//Debug.Log ("Hit Button");
					hit.collider.GetComponent<Button> ().onClick.Invoke ();
				} else {
					//Debug.Log ("Hit None Button UI");
					hit.collider.gameObject.SendMessage ("OnMouseDown");
				}
			}
		}
			
		/*if (Input.GetMouseButtonDown (0)) {
			//mRay = VuforiaMainCamera.WorldToScreenPoint (tempMousePosition);
			Debug.Log("TestCursor : " + TestCursor.GetComponent<Transform> ().localPosition);
			Debug.Log("tempMousePosition to Screen : " + VuforiaMainCamera.WorldToScreenPoint (TestCursor.GetComponent<Transform> ().position));
			mRay = VuforiaMainCamera.ScreenPointToRay (VuforiaMainCamera.WorldToScreenPoint (TestCursor.GetComponent<Transform> ().position));

			//Debug.Log ("GetMouseButtonDown " + Input.mousePosition);
			if (Physics.Raycast (mRay, out hit)) {
				Debug.Log ("Hit : " + hit.collider.name);
				if (hit.collider.GetComponent<Button>() != null) {
					Debug.Log ("Get Button");
					hit.collider.GetComponent<Button> ().onClick.Invoke ();
				}
			}
		}*/
	}

	void HideSystemBar()
	{
		using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
		{
			var window = activity.Call<AndroidJavaObject>("getWindow");
			var rootView = window.Call<AndroidJavaObject>("getDecorView");

			// hide navigation bar
			int vis = rootView.Call<int>("getSystemUiVisibility");
			vis |= 0x00000002; //View.SYSTEM_UI_FLAG_HIDE_NAVIGATION;
			vis |= 0x00000800; //View.SYSTEM_UI_FLAG_IMMERSIVE;
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
				{
					rootView.Call("setSystemUiVisibility", vis);
					rootView.Dispose();
				}));

			// disble swipe up navigation bar
			var lp = window.Call<AndroidJavaObject>("getAttributes");
			using (var c = lp.Call<AndroidJavaObject>("getClass"))
			using (var f = c.Call<AndroidJavaObject>("getDeclaredField", "customFlags"))
			{
				f.Call("setInt", lp, 0x4 /*CUSTOM_FLAG_FORBID_TRANSIENT_NAVIGATION_BAR*/);
				activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
					{
						window.Call("setAttributes", lp);
						window.Dispose();
						lp.Dispose();
					}));
			}
		}
	}
}
