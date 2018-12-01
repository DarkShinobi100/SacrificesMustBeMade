using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

    public int WallDamage = 1;
    public int PointsPerFood = 10;
    public int PointPerSoda = 20;
    public float RestartLevelDelay = 1f;
    public Text FriendText;

   // public AudioClip MoveSound1;
   // public AudioClip MoveSound2;
   // public AudioClip EatSound1;
   // public AudioClip EatSound2;
   // public AudioClip DrinkSound1;
   // public AudioClip DrinkSound2;
   // public AudioClip GameOverSound;


    private Animator animator;
    private int Friend;

    private Vector2 TouchOrigin = -Vector2.one; 

	// Use this for initialization
	protected override void Start ()
    {

        animator = GetComponent<Animator>();

        Friend = GameManager.Instance.PlayerFoodPoints;

        FriendText.text = "Friends: " + Friend;

        base.Start();
	}

    private void OnDisable()
    {
        GameManager.Instance.PlayerFoodPoints = Friend;
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
                    Horizontal = X > 0 ? 1 : -1;
                }
                else
                {
                    Vertical = Y > 0 ? 1 : -1;
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
        Friend--;
        FriendText.text = "Friends " + Friend;

        base.AttemptMove<T>(XDir, YDir);

        RaycastHit2D Hit;
        if (Move(XDir,YDir,out Hit))
        {
      //      SoundManager.Instance.RandomiseSFX(MoveSound1, MoveSound2);
        }

        CheckIfGameOver();

        GameManager.Instance.PlayersTurn = false;

    }

    private void OnTriggerEnter2D(Collider2D Other)
    {
        if(Other.tag == "Exit")
        {
            enabled = false;
            Invoke("Restart", RestartLevelDelay);
            
        }

        else if(Other.tag == "Food")
        {
            Friend += PointsPerFood;
            FriendText.text = "+ " + PointsPerFood + "Friend: " + Friend;
         //   SoundManager.Instance.RandomiseSFX(EatSound1, EatSound2);
            Other.gameObject.SetActive(false);
        }

        else if (Other.tag == "Soda")
        {
            Friend += PointPerSoda;
            FriendText.text = "+ " + PointPerSoda + "Friends: " + Friend;
       //     SoundManager.Instance.RandomiseSFX(DrinkSound1, DrinkSound2);
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
        SceneManager.LoadScene(1);
    }

    public void LoseFood(int Loss)
    {
        animator.SetTrigger("PlayerHit");

        Friend -= Loss;

        FriendText.text = "-" + Loss + "Friend " + Friend;

        CheckIfGameOver();
    
    }


    private void CheckIfGameOver()
    {
        if(Friend <= 0)
        {
           // SoundManager.Instance.PlaySingle(GameOverSound);
           // SoundManager.Instance.MusicSource.Stop();
            GameManager.Instance.GameOver();
        }
    }
}
