using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public string characterName;
	public int hp;
	public int damage;
	public int xPosition;
    public int zPosition;    
    public bool isFriendly;
    public bool hasMoved = false; //for tutorial usage
    public bool hasAttacked = false; //for tutorial usage
    public bool basicAttackDisabled = false; //for tutorial usage
    public bool movementDisabled = false; //for tutorial usage
    protected AudioManager audioManager;
    public BoardScript BoardManager;
    private List<List<Material>> originalMaterials;
    public TurnManager turnManager;
    private void Awake()
    {       
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        BoardManager = GameObject.Find("Board").GetComponent<BoardScript>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }

    public abstract bool CanMove(TileScript tile);

	private IEnumerator MoveToTile(TileScript targetTile)
	{
        if (audioManager != null)
            StartCoroutine(audioManager.PlaySound(audioManager.moving, 0.0f));
        else
            Debug.Log("AudioManager is null");

		/// Update parent to the new tile at the beginning of the movement
		if (!isFriendly)
			gameObject.transform.SetParent(targetTile.gameObject.transform.parent, true);

		Vector3 startPosition = transform.position; // Starting position
		float yOffset = startPosition.y - targetTile.transform.position.y;
		Vector3 endPosition = targetTile.transform.position + new Vector3(0, yOffset, 0); // Adjusted destination to maintain height

        float timeToMove = 1.8f; //0.8f; // Duration of the move in seconds, adjust as needed
		float elapsedTime = 0;

		while (elapsedTime < timeToMove)
		{
			float t = elapsedTime / timeToMove; // Normalized time
												// Apply easing function for smooth start and end
			float smoothStepT = t * t * (3f - 2f * t);

			// Interpolate position with easing
			transform.position = Vector3.Lerp(startPosition, endPosition, smoothStepT);
			elapsedTime += Time.deltaTime; // Update elapsed time
			yield return null; // Wait until next frame
		}
		gameObject.transform.SetParent(targetTile.gameObject.transform, true);
		// Ensure the character is exactly at the target position
		transform.position = endPosition;
	}

	public void Move(TileScript tile)
    {
        if (movementDisabled) return;
        if (tile.IsOccupied() || !CanMove(tile) || tile.IsDisabled())
        {
            Debug.LogWarning("Cant move here.");
            return;
        }
        
        TileScript originalTile = gameObject.transform.parent.GetComponentInChildren<TileScript>();

        if (isFriendly)
        {
            originalTile.SetFriendlyPresence(false);
            tile.SetFriendlyPresence(true);
        }
        else
        {
            originalTile.SetEnemyPresence(false);
            tile.SetEnemyPresence(true);
        }

        HideCharacterInfoWindow();

        originalTile.IsSelected = false;//
        Debug.Log("Is Tile_" + originalTile.xPosition + "_" + originalTile.zPosition + " selected? " + originalTile.IsSelected);//

        TileScript.ResetTileHighlights();
		StartCoroutine(MoveToTile(tile));
        hasMoved = true; //for tutorial

        xPosition = tile.xPosition;
        zPosition = tile.zPosition;

        if(isFriendly)
        {
            int MovesLeft=PlayerPrefs.GetInt("MovesLeft");
            MovesLeft--;
            PlayerPrefs.SetInt("MovesLeft",MovesLeft);            
        }
    }
    
    public abstract void NormalAttackSound();
    public abstract void SpecialAttack();

    public abstract void IdleSound();
    public void Attack(Character character)
	{
        if (basicAttackDisabled) return;

        HideCharacterInfoWindow();
        TileScript.ResetTileHighlights();
        NormalAttackSound();
        //character.TakeDamage(damage);
        hp--;
        int DAMAGE = damage;
        if (turnManager.effectActive[0]) DAMAGE+=10;
        character.TakeDamage(DAMAGE);
        //Debug.Log(DAMAGE);
        //int MovesLeft = PlayerPrefs.GetInt("MovesLeft");
        //MovesLeft--;
        //PlayerPrefs.SetInt("MovesLeft", MovesLeft);
        
        if(isFriendly)
        {
            int MovesLeft = PlayerPrefs.GetInt("MovesLeft");
            MovesLeft--;
            PlayerPrefs.SetInt("MovesLeft", MovesLeft);            
        }
        
        if(hp <= 0)
        {
            Destroy(this.gameObject);
            GetComponentInParent<TileScript>().ClearCharacterPresence();
        }

        hasAttacked = true;
    }

    public void TakeDamage(int damage)
    {
        hp = hp - damage;
        
        PaintCharacterSomeColor(this,Color.red);
        gameObject.TryGetComponent<Enemy>(out Enemy Enemycomponent);
        gameObject.TryGetComponent<DendriticCell>(out DendriticCell DendriticCellcomponent);
        gameObject.TryGetComponent<NeutrophilCell>(out NeutrophilCell NeutrophilCellcomponent);
        gameObject.TryGetComponent<TCell>(out TCell TCellcomponent);


        if(Enemycomponent!=null)
        {
            Enemycomponent.SpawnDamageTakenParticles();
            Enemycomponent.playDamageAnimation();
        }
        else if(DendriticCellcomponent!=null)
        {
            DendriticCellcomponent.SpawnDamageTakenParticles();
            DendriticCellcomponent.playDamageAnimation();
        }
        else if(NeutrophilCellcomponent!=null)
        {
            NeutrophilCellcomponent.SpawnDamageTakenParticles();
            NeutrophilCellcomponent.playDamageAnimation();
        }
        else if(TCellcomponent!=null)
        {
            TCellcomponent.SpawnDamageTakenParticles();
            TCellcomponent.playDamageAnimation();
        }

        if(hp<=0)
        {
            BoardManager.enemies.Remove(this);
            BoardManager.Frendlies.Remove(this);
            this.transform.parent.GetComponentInChildren<TileScript>().ClearCharacterPresence();
            StartCoroutine(WaitBeforeDestroying(0.5f));

            if(Enemycomponent != null)
            {
                PlayerPrefs.SetInt("IsVirusUnlocked", 1);
                PlayerPrefs.Save();
                Debug.Log("Killed virus, bestiary log update");
            }
        }
    }

    public IEnumerator WaitBeforeDestroying(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    //Find the first child with the specified tag.
    public Transform GetChildWithTag(Transform transform,string tag)
    {
        if (transform.childCount == 0)
        {
            return null;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag(tag))
            {
                return transform.GetChild(i);
            }
        }
        return null;
    }

    //Takes the character and paints it whole the specified color
    public void PaintCharacterSomeColor(Character character, Color color)
    {

        Transform model=GetChildWithTag(character.transform,"CharacterModel");
        if(model==null)
        {
            model=character.transform;
        } 
        for(int i=0; i<model.childCount;i++)
        {
            if(model.GetChild(i).TryGetComponent<SkinnedMeshRenderer>(out var rend))
            {
                foreach(Material material in rend.materials)
                {
                    StartCoroutine(ChangeMaterialColor(material,color));
                }
            }
        }
    }

    //Colors the material some specified color for a short time and turns it back;
    public IEnumerator ChangeMaterialColor(Material material, Color color)
    {
        Color OldColor = material.color;

        material.color=color;
        yield return new WaitForSeconds(0.2f);
        material.color=OldColor;
    }


    Material CreateBrightenedMaterial(Material originalMaterial, float brightnessIncrease)
    {
        Material newMaterial = new Material(originalMaterial);
        Color originalColor = newMaterial.color;
        Color newColor = originalColor * (1 + brightnessIncrease);
        newMaterial.color = newColor;

        return newMaterial;
    }

	void OnMouseEnter()
	{
		if (!isFriendly)
		{
			return;
		}

		originalMaterials = new List<List<Material>>();

		// Highlight character's body parts
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			Transform child = gameObject.transform.GetChild(i);
			SkinnedMeshRenderer renderer = child.GetComponentInChildren<SkinnedMeshRenderer>();

			if (!renderer)
			{
				continue;
			}

			originalMaterials.Add(renderer.materials.ToList());

			List<Material> newMaterials = new List<Material>();
			foreach (Material originalMaterial in renderer.materials)
			{
				// Create a new material with increased brightness
				Material newMaterial = CreateBrightenedMaterial(originalMaterial, 3);
				newMaterials.Add(newMaterial);
			}
			renderer.materials = newMaterials.ToArray();
		}

		// Highlight parent object
		if (gameObject.transform.parent != null)
		{
			Transform parentTransform = gameObject.transform.parent;
			HighlightParentObject(parentTransform);
		}
	}

	void HighlightParentObject(Transform parentTransform)
	{
		Renderer parentRenderer = parentTransform.GetComponent<Renderer>();

		if (parentRenderer != null)
		{
			// Store original materials
			originalMaterials.Add(parentRenderer.materials.ToList());

			List<Material> newMaterials = new List<Material>();
			foreach (Material originalMaterial in parentRenderer.materials)
			{
				// Create a new material with increased brightness
				Material newMaterial = CreateBrightenedMaterial(originalMaterial, 3);
				newMaterials.Add(newMaterial);
			}
			parentRenderer.materials = newMaterials.ToArray();
		}
	}

	void OnMouseExit()
    {
        if (!isFriendly)
        {
            return;
        }
        int originalMaterialsIndex = 0;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            SkinnedMeshRenderer renderer = child.GetComponentInChildren<SkinnedMeshRenderer>();
            if (!renderer)
            {
                continue;
            }
            
            renderer.materials = originalMaterials[originalMaterialsIndex].ToArray();
            originalMaterialsIndex++;
        }

		// De-highlight parent object
		if (gameObject.transform.parent != null)
		{
			Transform parentTransform = gameObject.transform.parent;
			Renderer parentRenderer = parentTransform.GetComponent<Renderer>();

			if (parentRenderer != null)
			{
				parentRenderer.materials = originalMaterials[originalMaterialsIndex].ToArray();
				originalMaterialsIndex++;
			}
		}
	}

    public Transform GetChildWithName(Transform transform,string Name)
    {
        if (transform.childCount == 0)
        {
            return null;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name==Name)
            {
                return transform.GetChild(i);
            }
        }
        return null;
    }

    //displays clicked character's information window
    abstract public void ShowCharacterInfoWindow();
    abstract public void HideCharacterInfoWindow();
    //hides all info windows
    public static void HideAllInfoWindows()
    {
        Transform neutrCardInfo = GameObject.Find("MenuUI's").transform.Find("NeutrophilCardInformation");
        Transform dendrCardInfo = GameObject.Find("MenuUI's").transform.Find("DendriticCellCardInformation");
        Transform TempCardInfo = GameObject.Find("MenuUI's").transform.Find("TemperatureCardInformation");

        //these are null in tutorial
        if (neutrCardInfo){
            neutrCardInfo.gameObject.SetActive(false);
        }
        if (dendrCardInfo){
            dendrCardInfo.gameObject.SetActive(false);
        }
        if (TempCardInfo){
            TempCardInfo.gameObject.SetActive(false);
        }
    }
}