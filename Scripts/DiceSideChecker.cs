using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSideChecker : MonoBehaviour 
{
	void OnTriggerStay(Collider col)
	{
		if (Dice.DiceVelocity.x == 0f && Dice.DiceVelocity.y == 0f && Dice.DiceVelocity.z == 0f && col.gameObject.GetComponent<Side>() != null)
		{
            switch (col.gameObject.GetComponent<Side>().SideID)
            {
                case 1:
                    Dice.DiceNumber = Dice.ListOfSideNumbers[5];
                    break;
                case 2:
                    Dice.DiceNumber = Dice.ListOfSideNumbers[4];
                    break;
                case 3:
                    Dice.DiceNumber = Dice.ListOfSideNumbers[3];
                    break;
                case 4:
                    Dice.DiceNumber = Dice.ListOfSideNumbers[2];
                    break;
                case 5:
                    Dice.DiceNumber = Dice.ListOfSideNumbers[1];
                    break;
                case 6:
                    Dice.DiceNumber = Dice.ListOfSideNumbers[0];
                    break;
            }
        }
    }
}			