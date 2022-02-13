using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dice : MonoBehaviour 
{
    #region Consts
    private const int MAX_SPEED = 200;
	private const float X_AXIS_LEFT_BORDER = -12f;
	private const float X_AXIS_RIGHT_BORDER = 9f;
	private const float Z_AXIS_DOWN_BORDER = -8.5f;
	private const float Z_AXIS_UP_BORDER = 9f;
	private const float Y_AXIS_BORDER = 2f;
    #endregion

    [System.Serializable]
	public struct DiceSideNumbers
    {
		public TextMeshPro TextObject;

		[Range(1, 99)]
		public int SideDigit; // from 1 to 99
    }

	[SerializeField] List<DiceSideNumbers> _DiceSideNumbers;

	private Rigidbody rb;
	private static int diceNumber;
	private static Vector3 diceVelocity;
	private static List<int> listOfSideNumbers = new List<int>();
	private float speed;
	private Vector3 startMousePosition;
	private Quaternion startRotation;
	private bool dragEnabled = false;
	private Vector3 mouseDragPosition = Vector3.zero;
	private bool isMouseUp = false;
	private bool isDragCancelled = false;

	public static int DiceNumber { get { return diceNumber; } set { diceNumber = value; } }
	public static Vector3 DiceVelocity { get { return diceVelocity; } private set { diceVelocity = value; } }
	public static List<int> ListOfSideNumbers { get { return listOfSideNumbers; } }

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		startRotation = rb.rotation;
		InitializeSides();
	}
    private void Update() 
	{
		DiceVelocity = rb.velocity;
		
		if (isMouseUp && rb.position.y >= Y_AXIS_BORDER && DiceManager.Instance.isRollActionDisabled)
        {
			rb.position = new Vector3(rb.position.x, Y_AXIS_BORDER, rb.position.z);
        }
	}

    private void OnMouseDown()
    {
		isMouseUp = false;
		isDragCancelled = false;
	}

    private void OnMouseDrag()
    {
		if (!isDragCancelled)
        {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit rayCastHit))
			{
				DiceManager.Instance.ClearResult();
				if (!dragEnabled)
				{
					GetComponent<BoxCollider>().enabled = false;
					rb.isKinematic = true;
					dragEnabled = true;
					rb.rotation = startRotation;
				}

				rb.position = new Vector3(rayCastHit.point.x, rayCastHit.point.y + .8f, rayCastHit.point.z);

				if ((Input.mousePosition - mouseDragPosition).normalized.x == 0f || (Input.mousePosition - mouseDragPosition).normalized.y == 0f)
				{
					startMousePosition = Input.mousePosition;
				}

				if (rb.position.x <= X_AXIS_LEFT_BORDER || rb.position.x >= X_AXIS_RIGHT_BORDER || rb.position.z <= Z_AXIS_DOWN_BORDER || rb.position.z >= Z_AXIS_UP_BORDER)
				{
					rb.isKinematic = false;
					dragEnabled = false;
					GetComponent<BoxCollider>().enabled = true;
					isDragCancelled = true;
					StartCoroutine(WaitForDiceStop());
				}

				mouseDragPosition = Input.mousePosition;
			}
		}	
	}

    private void OnMouseUp()
    {
		if (!isDragCancelled)
        {
			speed = Mathf.Abs(Input.mousePosition.x - startMousePosition.x) + Mathf.Abs(Input.mousePosition.y - startMousePosition.y);
			rb.isKinematic = false;
			dragEnabled = false;
			isMouseUp = true;

			if (speed > 200)
				speed = MAX_SPEED;

			GetComponent<BoxCollider>().enabled = true;
			Vector3 direction = (Input.mousePosition - startMousePosition).normalized;
			direction = new Vector3(direction.x, direction.z, direction.y);

			if (direction.x != 0f || direction.y != 0f || direction.z != 0f)
			{
				DiceManager.Instance.EnableFrontWallRaycasts(false);
				rb.rotation = Quaternion.identity;
				rb.AddTorque(Random.Range(0, 1000), Random.Range(0, 100), Random.Range(0, 1000), ForceMode.Impulse);
				rb.velocity = direction * speed;
				StartCoroutine(WaitForDiceStop());
			}
			else
			{
				rb.transform.position = new Vector3(Random.Range(X_AXIS_LEFT_BORDER, X_AXIS_RIGHT_BORDER), 1f, Random.Range(Z_AXIS_DOWN_BORDER, Z_AXIS_UP_BORDER));
			}
		}	
    }

	/// <summary>
	/// Wait for dice stop and get result
	/// </summary>
	/// <returns></returns>
	private IEnumerator WaitForDiceStop()
    {
		yield return new WaitUntil(() =>
		DiceVelocity.x == 0f && 
		DiceVelocity.y == 0f && 
		DiceVelocity.z == 0f);
		yield return new WaitUntil(() => DiceNumber > 0);
		DiceManager.Instance.EnableFrontWallRaycasts(true);
		DiceManager.Instance.GetResult();
    }

	/// <summary>
	/// Initialize sides of dice
	/// </summary>
	private void InitializeSides()
    {
        for (int i = 0; i < _DiceSideNumbers.Count; i++)
        {
			_DiceSideNumbers[i].TextObject.text = _DiceSideNumbers[i].SideDigit.ToString();
			listOfSideNumbers.Add(_DiceSideNumbers[i].SideDigit);
        }
    }
}
