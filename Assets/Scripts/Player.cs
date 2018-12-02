using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

    public int WallDamage = 1;
    public int PointsPerFood = 10;
    public int PointPerSoda = 20;
    public int PointsPerFriend = 1;
    public float RestartLevelDelay = 1f;
    public Text FriendText;
    public Text StaminaText;
    public Text ExcapeText;


     public AudioClip MoveSound1;
     public AudioClip MoveSound2;
     public AudioClip EatSound1;
     public AudioClip EatSound2;
     public AudioClip DrinkSound1;
     public AudioClip DrinkSound2;
     public AudioClip GameOverSound;


    private Animator animator;
    private int Friend;
    private int Stamina;
    private int ExcapeCounter;

    private Vector2 TouchOrigin = -Vector2.one; 

	// Use this for initialization
	protected override void Start ()
    {

        animator = GetComponent<Animator>();

        //set up player stats

        //friends
        Friend = GameManager.Instance.PlayerFriendPoints;
        FriendText.text = "Friends: " + Friend;

        //stamina
        Stamina = GameManager.Instance.PlayerStamina;
        StaminaText.text = "Stamina: " + Stamina;

        //amount of rooms till you escape
        ExcapeCounter = GameManager.Instance.ExcapeCounter;
        ExcapeText.text = "Rooms Till escape: " + ExcapeCounter;

        base.Start();
	}

    private void OnDisable()
    {
        GameManager.Instance.PlayerFriendPoints = Friend;
        GameManager.Instance.PlayerStamina = Stamina;
        GameManager.Instance.ExcapeCounter = ExcapeCounter;
    }


    // Update is called once per frame
    void Update ()
    {
		if(!GameManager.Instance.PlayersTurn)
        {
            return;
        }

        int Horizontal = 0;
        int Vertical = 0;

#if UNITY_EDITOR|| UNITY_STANDALONE || UNITY_WEBPLAYER

        Horizontal = (int)Input.GetAxisRaw("Horizontal");
        Vertical = (int)Input.GetAxisRaw("Vertical");


        //Animation controls
        if (Horizontal < 0)
        {
            animator.SetInteger("DirectionX", -1);
        }
        else if (Horizontal > 0)
        {
            animator.SetInteger("DirectionX", 1);
        }
        else
        {
            animator.SetInteger("DirectionX", 0);
        }

        if (Vertical < 0)
        {
            animator.SetInteger("DirectionY", -1);
        }
        else if (Vertical > 0)
        {
            animator.SetInteger("DirectionY", 1);
        }
        else
        {
            animator.SetInteger("DirectionY", 0);
        }
        
        if (Horizontal!=0)
        {
            Vertical = 0;
        }

#else

        if (Input.touchCount>0)
        {

            Touch myTouch = Input.touches[0];

            if(myTouch.phase == TouchPhase.Began)
            {
                TouchOrigin = myTouch.position;
            }

            else if(myTouch.phase == TouchPhase.Ended && TouchOrigin.x >= 0)
            {
                Vector2 TouchEnd = myTouch.position;
                float X = TouchEnd.x - TouchOrigin.x;
                float Y = TouchEnd.y - TouchOrigin.y;
                TouchOrigin.x = -1;

                if (Mathf.Abs(X) > Mathf.Abs(Y))
                {
                   // Horizontal = X > 0 ? 1 : -1;
                    if (X>0)
                        { 
                        Horizontal = 1;
                        animator.SetInteger("DirectionX", 1);
                        }
                    else if(X<0)
                        {
                         Horizontal = -1;
                         animator.SetInteger("DirectionX", -1);
                        }
                    else
                        {
                            animator.SetInteger("DirectionX", 0);
                        }
                }
                else
                {
                    Vertical = Y > 0 ? 1 : -1;
                       if (Y>0)
                        { 
                        Horizontal = 1;
                        animator.SetInteger("DirectionY", 1);
                        }
                    else if (Y<0)
                        {
                         Horizontal = -1;
                         animator.SetInteger("DirectionY", -1);
                        }
                 else
                        {
                            animator.SetInteger("DirectionY", 0);
                        }
                }

            }
        }

#endif
        if (Horizontal !=0 || Vertical !=0)
        {
            AttemptMove<Wall>(Horizontal, Vertical);
        }

    }
    protected override void AttemptMove<T>(int XDir, int YDir)
    {
        base.AttemptMove<T>(XDir, YDir);

        RaycastHit2D Hit;
        if (Move(XDir,YDir,out Hit))
        {
            SoundManager.Instance.RandomiseSFX(MoveSound1, MoveSound2);
        }

        //costs 1 stamina to move
        Stamina = Stamina - 1;
        StaminaText.text = "Stamina: " + Stamina;

        //show current amount of friends
        FriendText.text = "Friends: " + Friend;

        CheckIfGameOver();

        GameManager.Instance.PlayersTurn = false;
        animator.SetTrigger("PlayerIdle");
    }

    private void OnTriggerEnter2D(Collider2D Other)
    {
        if(Other.tag == "Exit")
        {
            //counter till freedom
            ExcapeCounter = ExcapeCounter - 1;
            ExcapeText.text = "Rooms Till escape: " + ExcapeCounter;

            if(ExcapeCounter == 0)
            {
                //display win screen
                SceneManager.LoadScene(4);
            }

            enabled = false;
            Invoke("Restart", RestartLevelDelay);
            
        }

        else if(Other.tag == "Food")
        {
            Stamina += PointsPerFood;
            StaminaText.text = "+ " + PointsPerFood + "Stamina: " + Stamina;
            SoundManager.Instance.RandomiseSFX(EatSound1, EatSound2);
            Other.gameObject.SetActive(false);
        }

        else if (Other.tag == "Soda")
        {
            Stamina += PointPerSoda;
            StaminaText.text = "+ " + PointPerSoda + "Stamina: " + Stamina;
            SoundManager.Instance.RandomiseSFX(DrinkSound1, DrinkSound2);
            Other.gameObject.SetActive(false);
        }
        else if (Other.tag == "Friend")
        {
            Friend += PointsPerFriend;
            FriendText.text = "+ " + PointsPerFriend + "Friend: " + Friend;
               SoundManager.Instance.RandomiseSFX(EatSound1, EatSound2);
            Other.gameObject.SetActive(false);
        }
    }


    protected override void OnCantMove<T>(T Component)
    {
        Wall HitWall = Component as Wall;
        HitWall.DamageWall(WallDamage);
        animator.SetTrigger("PlayerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(3);
    }

    public void SacrificeFriend(int Loss)
    {
        animator.SetTrigger("PlayerHit");

        Friend -= Loss;

        FriendText.text = "-" + Loss + "Friend " + Friend;

        CheckIfGameOver();
    
    }


    private void CheckIfGameOver()
    {
        //you need friends AND stamina or you lose
        if(Stamina <= 0 || Friend <=0)
        {
            SoundManager.Instance.PlaySingle(GameOverSound);
            SoundManager.Instance.MusicSource.Stop();
            GameManager.Instance.GameOver();
        }
    }
}
