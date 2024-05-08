using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import this namespace to work with UI elements

public class PlayerHealth : MonoBehaviour
{
	public Sprite heartSprite; // Assign your heart sprite in the inspector
	public int numberOfLivesAtTheStart = 2; // Number of lives
	public int currentHealth; // To track the current health of the player

	// Starting position and spacing for the hearts
	public Vector3 startPosition;
	public float spacing = 0.1f;

	private GameObject[] hearts; // Array to hold the heart GameObjects

	private BoardScript boardScript;

	void Start()
	{
		boardScript = GameObject.Find("Board").GetComponent<BoardScript>();
		currentHealth = numberOfLivesAtTheStart; // Initialize current health
		hearts = new GameObject[numberOfLivesAtTheStart]; // Initialize the hearts array
		SpawnHearts();
	}

	void SpawnHearts()
	{
		startPosition = transform.localPosition;

		for (int i = 0; i < numberOfLivesAtTheStart; i++)
		{
			// Create a new GameObject with an Image component
			GameObject heartGO = new GameObject($"Heart_{i + 1}", typeof(Image));

			// Set the new GameObject to be a child of this object (the object this script is attached to)
			heartGO.transform.SetParent(transform, false);

			// Get the Image component and set the sprite
			Image heartImage = heartGO.GetComponent<Image>();
			heartImage.sprite = heartSprite;

			// Optionally adjust the size of the heart image
			RectTransform heartRect = heartImage.rectTransform;
			heartRect.anchoredPosition = new Vector2(0, -(spacing * i));
			heartRect.sizeDelta = new Vector2(0.1f, 0.1f); // Adjust the size as needed

			// Add the heart GameObject to the hearts array
			hearts[i] = heartGO;
		}
	}

	public void TakeDamage(int damage)
	{
		if (currentHealth <= 0) return; // Exit if no health left

		currentHealth -= damage; // Decrease current health by the damage amount

		// Ensure currentHealth doesn't go below 0
		currentHealth = Mathf.Max(0, currentHealth);

		// Deactivate the last active heart
		DeactivateLastHeart();
	}

	private void DeactivateLastHeart()
	{
		if (currentHealth < numberOfLivesAtTheStart && currentHealth >= 0)
		{
			// Deactivate the heart GameObject at the currentHealth index
			hearts[currentHealth].SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Example usage: if (Input.GetKeyDown(KeyCode.Space)) { TakeDamage(1); }
		TakeDamageToPlayerHealth();
		CheckIfPlayerIsDead();
	}

	private void TakeDamageToPlayerHealth()
	{
		if (TileScript.IsEnemyInLastRow())
		{
			StartCoroutine(DelayedDamage(1, 1.7f));
		}
	}

	private IEnumerator DelayedDamage(int damage, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);

		// Now apply the damage after the delay
		TakeDamage(damage);
		Debug.Log("gyvybes: " + currentHealth);
	}

	public int absoluteMaxHearts = 10; // Define an absolute maximum number of hearts to avoid infinite expansion

	public void AddLife()
	{
		if (currentHealth < numberOfLivesAtTheStart)
		{
			// Reactivate the next deactivated heart
			currentHealth++;
			hearts[currentHealth - 1].SetActive(true);
		}
		else if (numberOfLivesAtTheStart < absoluteMaxHearts) // Check against an absolute maximum
		{
			// Increase the total number of lives
			numberOfLivesAtTheStart++;
			currentHealth++;

			// Create and initialize a new heart GameObject
			GameObject newHeart = new GameObject($"Heart_{numberOfLivesAtTheStart}", typeof(Image));
			newHeart.transform.SetParent(transform, false);
			Image heartImage = newHeart.GetComponent<Image>();
			heartImage.sprite = heartSprite;
			RectTransform heartRect = heartImage.rectTransform;
			heartRect.anchoredPosition = new Vector2(0, -(spacing * (numberOfLivesAtTheStart - 1)));
			heartRect.sizeDelta = new Vector2(0.1f, 0.1f); // Adjust the size as needed

			// Expand the hearts array and add the new heart GameObject
			if (hearts.Length < numberOfLivesAtTheStart)
			{
				Array.Resize(ref hearts, numberOfLivesAtTheStart);
			}
			hearts[numberOfLivesAtTheStart - 1] = newHeart;
		}
		else
		{
			Debug.Log("Maximum number of hearts reached.");
		}
	}

	private void CheckIfPlayerIsDead()
	{
		if(currentHealth==0)
		{
        	StartCoroutine(boardScript.ShowLoseScreenAfterDelay());			
		}
    }
}
