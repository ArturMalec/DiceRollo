using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceManager : MonoBehaviour
{
	private const int DICE_UPWARD_SPEED = 1700;

    [SerializeField] Rigidbody _Dice;
	[SerializeField] TextMeshProUGUI _ResultText;
    [SerializeField] Button _RollButton;
    [SerializeField] GameObject _FrontWall;

    public static DiceManager Instance;
    public bool isRollActionDisabled { get { return _RollButton.interactable; } }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RollButtonAction()
    {
		float dirX = Random.Range(0, 1000);
		float dirY = Random.Range(0, 1000);
		float dirZ = Random.Range(0, 1000);
		_Dice.transform.position = new Vector3(_Dice.transform.position.x, 2, _Dice.transform.position.z);
		_Dice.transform.rotation = Quaternion.identity;
		_Dice.AddForce(transform.up * DICE_UPWARD_SPEED);
		_Dice.AddTorque(dirX, dirY, dirZ);        
        StartCoroutine(WaitForResult());
	}

    private IEnumerator WaitForResult()
    {
        _RollButton.interactable = false;
        ClearResult();
        yield return new WaitUntil(() => Dice.DiceNumber > 0);
        GetResult();
        _RollButton.interactable = true;
    }

    public void ClearResult()
    {
        Dice.DiceNumber = 0;
        _ResultText.text = "Result: ?";
    }

    public void GetResult()
    {
        _ResultText.text = "Result: " + Dice.DiceNumber.ToString();
    }

    public void EnableFrontWallRaycasts(bool state)
    {
        if (state)
            _FrontWall.layer = 2;
        else
            _FrontWall.layer = 0;
    }
}
