using UnityEngine;
using System.Collections;

namespace com.bzr.puzzleBattle {
	public class SimonPlate : MonoBehaviour
	{
		//public GameObject highlight;
		//public Color color;
		//public Color alternateColor;

		public string colour;

		public int plate_id;

		public bool altColour=false;

		public Vector3 initialTransform;
		public Vector3 previousTransform;

		private bool dragging = false;
		private Vector3 targetDragging;

		public Move currentMove = null;
		// Use this for initialization
		void Start ()
		{
			//if (color != null) {
				//renderer.material.color = color;

			//}
			//ChangeColour(color,true);
			GetComponent<UISprite>().alpha=0.5f;

			//highlight.SetActive (false);
			unHighlight();
			//initialTransform = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		}
	
		public void takeInitialTransform(){
			initialTransform = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			takePreviousTransform();
		}

		public void takePreviousTransform(){
			previousTransform = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		}


		public void updateAlpha(float val){
		
			GetComponent<UISprite>().alpha=val;

		}

		public void fadeIn(){
			iTween.StopByName(this.gameObject,"fade");
			iTween.ValueTo(this.gameObject,iTween.Hash("name","fade","from",GetComponent<UISprite>().alpha,"to",1f,"time",1f,"onupdate","updateAlpha","onupdatetarget",this.gameObject));

		}

		public void fadeOut(float delay){
			iTween.StopByName(this.gameObject,"fade");
			iTween.ValueTo(this.gameObject,iTween.Hash("name","fade","delay",delay,"from",GetComponent<UISprite>().alpha,"to",0.5f,"time",1f,"onupdate","updateAlpha","onupdatetarget",this.gameObject));
			
		}

		public void fadeOut(){
			iTween.StopByName(this.gameObject,"fade");
			iTween.ValueTo(this.gameObject,iTween.Hash("name","fade","from",GetComponent<UISprite>().alpha,"to",0.5f,"time",1f,"onupdate","updateAlpha","onupdatetarget",this.gameObject));
			
		}

		public void fadeOutCompletely(){
			iTween.StopByName(this.gameObject,"fade");
			iTween.ValueTo(this.gameObject,iTween.Hash("name","fade","from",GetComponent<UISprite>().alpha,"to",0f,"time",1f,"onupdate","updateAlpha","onupdatetarget",this.gameObject));

		}


		public void rotateToZero(){
			iTween.Stop (this.gameObject,"RotateTo");
			iTween.RotateTo(this.gameObject,Vector3.zero,1f);

		}

		public void ChangeColour(){

		//	altColour=!altColour;

			//Debug.Log ("changeColour");


			//GetComponent<UISprite>().spriteName = colour + "_" + (altColour ? "alt_" : "") + "inactive";
			GetComponent<UIButton>().normalSprite = colour + "_" + (altColour ? "alt_" : "") + "inactive";
			GetComponent<UIButton>().hoverSprite = colour + "_" + (altColour ? "alt_" : "") + "active";


			//GetComponent<UIButton>().defaultColor = newColor;
			//GetComponent<UIButton>().hover = newColor;
			//GetComponent<UIButton>().UpdateColor(immediate);
			//GetComponent<UIButton>().pressed = color;
			//GetComponent<UIButton>().disabledColor = color;


			//GetComponent<UISprite>().color = color;
			//GetComponent<UISprite>().MarkAsChanged();
		}

		// Update is called once per frame
		void Update ()
		{
			if (dragging) {
				//highlight.SetActive (true);
				Highlight();
				//GetComponent<UISprite>().depth=2;
				//float newx = targetDragging.x;// * Time.deltaTime;
				//float newy = targetDragging.y;// * Time.deltaTime;
				//float newz = -1f;// * Time.deltaTime;
				//Debug.DrawRay(transform.position,targetDragging);

				//transform.position = new Vector3(transform.position.x,transform.position.y,-1f);
			}
		}

		public void DisplayMove(Move move) {
			takePreviousTransform();
			if(altColour)
				GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().Play();
			else
				GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().PlayJustOne();
			switch (move.move_id) {
				case MoveDefinition.CLICK:
					Highlight();
					EndMove(move);
					print("click " + move.src_plate_id );
					break;
				case MoveDefinition.DRAG:
					Highlight();
					
					GetComponent<UISprite>().depth=2;

					targetDragging = move.target_plate.transform.position;//transform.InverseTransformPoint(move.target_plate.transform.position);
					Vector3 t2 = new Vector3(targetDragging.x, targetDragging.y, -0.01f);
					Vector3 t1 = new Vector3(transform.position.x, transform.position.y, -0.01f);
					transform.position=t1;
					//Debug.Log ("my local position: " + transform.localPosition.ToString());
					//Debug.Log ("target world position: " + targetDragging.ToString());

					dragging = true;
					
					iTween.MoveTo(this.gameObject,iTween.Hash("position",t2,"time",0.5f,"easeType",iTween.EaseType.easeOutExpo,"oncomplete","EndMove","oncompletetarget",this.gameObject,"oncompleteparams",move,"name","MoveTo"));

					print("drag to " + move.target_plate_id );
					break;
				case MoveDefinition.SWIP:
					Highlight();
					dragging = true;
					print("swipe plate " + move.src_plate_id );
					
					iTween.MoveBy(this.gameObject,iTween.Hash("amount",(transform.position-transform.parent.position),"time",0.5f,"oncomplete","EndMove","oncompletetarget",this.gameObject,"oncompleteparams",move,"space",Space.World,"easetype",iTween.EaseType.easeOutExpo)); // flick away from center?
					//iTween.PunchPosition(this.gameObject,iTween.Hash("amount",transform.position*2f,"time",2f,"oncomplete","EndMove","oncompletetarget",this.gameObject,"oncompleteparams",move,"space",Space.World)); // flick away from center?

					break;
				/*case MoveDefinition.SHAKE:
						Debug.Log("colour swap");

						altColour = !altColour;
						ChangeColour();
						
					break;*/
				default:
					break;
			}
		}

		private bool NoMove() {
			return currentMove == null;
		}

		public bool ValidateMove(TapGesture gesture) {

			if(altColour)
				GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().Play();
			else
				GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().PlayJustOne();

			if (NoMove ()) {
				Debug.Log("tapped wrong glyph");
				return false;
			} else {
				if(currentMove.move_id != MoveDefinition.CLICK)
					Debug.Log("tapped when shouldn't have");
				return currentMove.move_id == MoveDefinition.CLICK;
			}
		}

		public bool ValidateMove(DragGesture gesture) {

			if(altColour)
				GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().Play();
			else
				GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().PlayJustOne();

			Vector3 mousePosition = PlateHandler.GetWorldPos( gesture.Position );
			Vector3 startPosition = PlateHandler.GetWorldPos( gesture.StartPosition );
			if (NoMove ()) {
				Debug.Log("dragged wrong glyph");
				ResetState();
				return false;
			} else {
				if (currentMove.move_id == MoveDefinition.DRAG) {
					float distance = Vector2.Distance ((Vector2)currentMove.target_plate.transform.position,(Vector2) mousePosition);
					//Debug.DrawRay(currentMove.target_plate.transform.position,mousePosition,Color.white,30f);
					//Debug.Log(PlateHandler.GetWorldPos( gesture.Position ));
//					Debug.Log(distance);
					if (distance < 0.2f ) {
						SinglePlayerProgression.Instance.AchievementProgress("drag",1); //note that we're only counting valid moves for achievements
						EndMove(currentMove);
						return true;
					} else {
						EndMove(currentMove);
						return false;
					}
				}
				else if (currentMove.move_id == MoveDefinition.SWIP) {
				


					Debug.Log("swipe move caught by drag handler");
					Debug.Log ("magnitude" + gesture.TotalMove.magnitude);

					float angle = Vector2.Angle((PlateHandler.GetWorldPos(gesture.Position)-transform.parent.position),PlateHandler.GetWorldPos(gesture.StartPosition));
					//Debug.Log(angle);

					ResetState();

					if(angle<45f && gesture.TotalMove.magnitude>1f)
						SinglePlayerProgression.Instance.AchievementProgress("flick",1);

					return angle<45f && gesture.TotalMove.magnitude>1f;


					/*
					float angle = Vector2.Angle(Vector2.zero,(Vector2)mousePosition);
					Debug.Log("angle: " + angle);
					
					float initialDistanceToCenter = Vector2.Distance (Vector2.zero,(Vector2) currentMove.src_plate.initialTransform);
					Debug.Log("starting distance to center: " + initialDistanceToCenter);
					
					float startingAngle = Vector2.Angle(Vector2.zero,(Vector2)currentMove.src_plate.initialTransform);
					Debug.Log("starting angle: " + startingAngle);
					
					return Mathf.Abs(angle-startingAngle)<30f;
*/
					//Debug.Log(mousePosition);
				
					//return false;
				}
				else{
					Debug.Log ("did wrong move to right glyph");
					ResetState();
					return false;
				}
			}
		}

		public bool ValidateMove(SwipeGesture gesture) {

			if(altColour)
				GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().Play();
			else
				GetComponent<PlayTwoSoundsOneOfWhichIsRandom>().PlayJustOne();

			Vector3 mousePosition = PlateHandler.GetWorldPos( gesture.Position );
			if (NoMove ()) {
				Debug.Log("swiped wrong glyph");
				ResetState();
				return false;
			}
			else {
				if (currentMove.move_id == MoveDefinition.SWIP) {

					float angle = Vector2.Angle((PlateHandler.GetWorldPos(gesture.Position)-transform.parent.position),PlateHandler.GetWorldPos(gesture.StartPosition));
					Debug.Log(angle);
					return angle<30f;

				//	float distanceToCenter = Vector2.Distance (Vector2.zero,(Vector2) mousePosition);
			//		Debug.Log("distance to center: " + distanceToCenter);

					//Debug.Log (Vector2.Angle(mousePosition,Vector2.right));

					//float angle = Vector2.Angle(Vector2.zero,(Vector2)mousePosition);
					//Debug.Log("angle: " + angle);

					//float initialDistanceToCenter = Vector2.Distance (Vector2.zero,(Vector2) currentMove.src_plate.initialTransform);
					//Debug.Log("starting distance to center: " + initialDistanceToCenter);

					//float startingAngle = Vector2.Angle(Vector2.zero,(Vector2)currentMove.src_plate.initialTransform);
					//Debug.Log("starting angle: " + startingAngle);

					//return true;

					//return Mathf.Abs(angle-startingAngle)<30f;

					//gesture.Direction;

				} else if(currentMove.move_id==MoveDefinition.DRAG){
				
					Debug.Log("drag move caught by swipe handler");
					//Debug.Log (gesture.Velocity);
					return false;

				}
				else {
					return false;
				}
			}

		}

		public IEnumerator DelayedUnHighlight(float time){
			yield return new WaitForSeconds(time);
			unHighlight();
		}

		public void EndMove(Move move) {
			if (move.move_id == MoveDefinition.DRAG) {
				//Vector3 new_source = new Vector3 (initialTransform.x, initialTransform.y, initialTransform.z);
				/*
				// my new position is the target plate position
				Vector3 p = move.target_plate.transform.position;
				transform.position = new Vector3 (p.x, p.y, p.z);
				initialTransform = new Vector3 (p.x, p.y, p.z);
	
				// the target plate now takes my place.
				move.target_plate.transform.position = new_source;
				move.target_plate.initialTransform = new_source;
				move.target_plate.transform.Translate(new Vector3(0,0,0));*/

				//iTween.MoveTo(this.gameObject,iTween.Hash("position",initialTransform,"easeType",iTween.EaseType.spring,"time",0.4f,"oncomplete","ResetState","concompletetarget",this));

				//transform.position = new Vector3(initialTransform.x, initialTransform.y, initialTransform.z);
				ResetState();
			}
			if (move.move_id == MoveDefinition.SWIP) {
				//transform.position = new Vector3(initialTransform.x, initialTransform.y, initialTransform.z);
				ResetState();
			}
			if(move.move_id==MoveDefinition.CLICK){
				StartCoroutine(DelayedUnHighlight(0.5f));
			}

		}

		public void ReallyResetState() {
			//highlight.SetActive (false);
			unHighlight(true);
			Vector3 t1 = new Vector3(initialTransform.x, initialTransform.y, 0f);
			transform.position=t1;
			if(GetComponent<Rigidbody>())
				GetComponent<Rigidbody>().velocity=Vector3.zero;
			dragging = false;
		}

		public void ResetState(bool immediate = false) {
			//highlight.SetActive (false);
			unHighlight();
			Vector3 t2 = new Vector3(previousTransform.x, previousTransform.y, 0f);
			Vector3 t1 = new Vector3(transform.position.x, transform.position.y, 0f);
			transform.position=t1;
			// transform.position = new Vector3(initialTransform.x, initialTransform.y, initialTransform.z);
			if(GetComponent<Rigidbody>())
				GetComponent<Rigidbody>().velocity=Vector3.zero;
			iTween.MoveTo(this.gameObject,iTween.Hash("position",t2,"easeType",iTween.EaseType.spring,"time",immediate?0f : 0.4f));//"oncomplete","takeInitialTransform","oncompletetarget",this.gameObject));
			dragging = false;
			//transform.position = t;
		}

		public void Highlight(bool immediate = true) {

			//Vector3 t = new Vector3(transform.position.x, transform.position.y, -1);
			//transform.position = t;
			//highlight.SetActive(true);
			GetComponent<UIButton>().SetState (UIButtonColor.State.Hover,immediate);
			ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
			if( emitter )
				emitter.Emit();
		}
		public void unHighlight(bool immediate = true) {
			//Vector3 t = new Vector3(transform.position.x, transform.position.y, 0);
			//transform.position = t;
			//highlight.SetActive(false);
			GetComponent<UIButton>().SetState (UIButtonColor.State.Normal,immediate);
			GetComponent<UISprite>().depth=1;

		}
	}
}