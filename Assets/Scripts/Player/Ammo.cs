using UnityEngine;

public class Ammo : MonoBehaviour
{

    public Save.MagicType type;
    public int amount;

    SpriteRenderer renderer;

    public SpriteRenderer lightning;
    public SpriteRenderer fire;
    public SpriteRenderer air;
    public SpriteRenderer water;
    public TextMesh ammoAmount;

	public void Init (Save.MagicType type, int amount) {
		this.type = type;
		this.amount = amount;

        lightning.gameObject.SetActive(false);
        fire.gameObject.SetActive(false);
        air.gameObject.SetActive(false);
        water.gameObject.SetActive(false);
        ammoAmount.text = amount.ToString();
        ammoAmount.GetComponent<Renderer>().sortingLayerName = "UI";


        switch (type)
        {
            case Save.MagicType.Fire:
                fire.gameObject.SetActive(true);
                break;
            case Save.MagicType.Lightning:
                lightning.gameObject.SetActive(true);
                break;
            case Save.MagicType.Air:
                air.gameObject.SetActive(true);
                break;
            case Save.MagicType.Water:
                water.gameObject.SetActive(true);
                break;
        }
        ammoAmount.text = amount.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		//Ako prede s misem pokazi kolko ima 
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.tag == "Player") 
		{
			Game.instance.playerCombat.AddAmmo(type, amount);
			Destroy(gameObject);
		}
    }
}
