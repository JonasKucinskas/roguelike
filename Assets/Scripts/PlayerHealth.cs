using UnityEngine;
using UnityEngine.UI; // Import this namespace to work with UI elements

public class PlayerHealth : MonoBehaviour
{
	public Sprite heartSprite; // Assign your heart sprite in the inspector
	public int numberOfLives = 3; // Number of lives
	private int currentHealth; // To track the current health of the player

	// Starting position and spacing for the hearts
	public Vector3 startPosition;
	public float spacing = 0.1f;

	private GameObject[] hearts; // Array to hold the heart GameObjects

	void Start()
	{
		currentHealth = numberOfLives; // Initialize current health
		hearts = new GameObject[numberOfLives]; // Initialize the hearts array
		SpawnHearts();
	}

	void SpawnHearts()
	{
		startPosition = transform.localPosition;

		for (int i = 0; i < numberOfLives; i++)
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
		if (currentHealth < numberOfLives && currentHealth >= 0)
		{
			// Deactivate the heart GameObject at the currentHealth index
			hearts[currentHealth].SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Example usage: if (Input.GetKeyDown(KeyCode.Space)) { TakeDamage(1); }
	}
}
