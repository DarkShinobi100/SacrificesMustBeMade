using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public Sprite dmgSprite;
    public int HP = 3;

    public AudioClip ChopSound1;
    public AudioClip ChopSound2;

    private SpriteRenderer spriteRenderer;


	// Use this for initialization
	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    public void DamageWall(int loss)
    {
        SoundManager.Instance.RandomiseSFX(ChopSound1, ChopSound2);
        spriteRenderer.sprite = dmgSprite;
        HP -= loss;
        if(HP<=0)
        {
            gameObject.SetActive(false);

        }
    }
}
