using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.UI; // Namespace for TextMeshPro

[System.Serializable]
public class Bonus
{
	public string bonusName;
	public string bonusDescription;
}

public class BonusManager : MonoBehaviour
{
	public List<Bonus> availableBonuses; // Stores all available bonuses
	public TextMeshProUGUI topButtonText;
	public TextMeshProUGUI middleButtonText;
	public TextMeshProUGUI bottomButtonText;
	public Button topButton;
	public Button middleButton;
	public Button bottomButton;

	private List<Bonus> selectedBonuses; // Stores the bonuses selected for display

	void Start()
	{
		InitializeBonuses();
		AssignBonusTextToButtons();
		// You can also load bonuses from a file or database here

		topButton.onClick.AddListener(delegate { ApplyBonus(0); });
		middleButton.onClick.AddListener(delegate { ApplyBonus(1); });
		bottomButton.onClick.AddListener(delegate { ApplyBonus(2); });
	}

	void InitializeBonuses()
	{
		// Initialize your bonuses here
		availableBonuses = new List<Bonus>
		{
			new Bonus { bonusName = "Bonus 1", bonusDescription = "Neutrophil deals +2 damage" },
			new Bonus { bonusName = "Bonus 2", bonusDescription = "Dendritic cell deals +2 damage" },
			new Bonus { bonusName = "Bonus 3", bonusDescription = "Player gains +1 health" },
			new Bonus { bonusName = "Bonus 4", bonusDescription = "There is a small chance to draw 2 cards instead of 1" },
			new Bonus { bonusName = "Bonus 5", bonusDescription = "Neutrophil has a small chance not to damage itself and orher friendly characters with the special attack" },
			new Bonus { bonusName = "Bonus 6", bonusDescription = "Dendritic cell's special attack takes 1 less turn to get ready" },
			// Add as many bonuses as you have
        };
	}

	public void AssignBonusTextToButtons()
	{
		// Select 3 unique bonuses at random
		selectedBonuses = SelectUniqueBonuses(3);

		// Assign the selected bonuses to the buttons
		topButtonText.text = selectedBonuses[0].bonusDescription;
		middleButtonText.text = selectedBonuses[1].bonusDescription;
		bottomButtonText.text = selectedBonuses[2].bonusDescription;
	}

	private List<Bonus> SelectUniqueBonuses(int numberOfBonuses)
	{
		List<Bonus> selected = new List<Bonus>();
		List<Bonus> availableToPick = new List<Bonus>(availableBonuses); // Make a copy of available bonuses

		for (int i = 0; i < numberOfBonuses; i++)
		{
			if (availableToPick.Count == 0)
			{
				break; // Avoid infinite loop if requesting more bonuses than available
			}

			int randomIndex = Random.Range(0, availableToPick.Count);
			selected.Add(availableToPick[randomIndex]);
			availableToPick.RemoveAt(randomIndex); // Remove selected bonus to prevent re-selection
		}

		return selected;
	}

	public void ApplyBonus(int bonusIndex)
	{
		// Check if the bonusIndex is within the range of selectedBonuses
		/*if (bonusIndex < 0 || bonusIndex >= selectedBonuses.Count)
		{
			Debug.LogError("Bonus index out of range");
			return;
		}*/
		FindAnyObjectByType<PauseMenu>().GetComponent<PauseMenu>().BonusSelectUI.SetActive(false);
		// Get the selected bonus based on the index
		Bonus selectedBonus = selectedBonuses[bonusIndex];

		Debug.Log("Applying bonus: " + selectedBonus.bonusName);

		
	}
}
