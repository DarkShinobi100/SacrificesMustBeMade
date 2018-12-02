using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int PlayerDamage;

    private Animator animator;
    private Transform Target;
    private bool SkipMove;

    public AudioClip EnemyAttack1;
    public AudioClip EnemyAttack2;

    // Use this for initialization
    protected override void Start()
    {
        GameManager.Instance.AddEnemiesToList(this);
        animator = GetComponent<Animator>();
        Target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();

    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Call the AttemptMove function from MovingObject.
        base.AttemptMove<T>(xDir, yDir);
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        //update code to allow animations

        //check if player & enemy are in the same collumn
        if(Mathf.Abs(Target.position.x - transform.position.x)< float.Epsilon)
        {
            //now check if our Y Co-ord is greater than or less than the player
            if(Target.position.y > transform.position.y)
            {
                //if this is true then we need to move up 1
                yDir = 1;
                animator.SetTrigger("MoveUp");
            }
            else
            {
                //otherwise move down
                yDir = -1;
                animator.SetTrigger("MoveDown");

            }
        }
        else
        {
            //now check if our x Co-ord is greater than or less than the player
            if (Target.position.x > transform.position.x)
            {
                //if this is true then we need to move right 1
                xDir = 1;
                animator.SetTrigger("MoveRight");
            }
            else
            {
                //otherwise move left
                xDir = -1;
                animator.SetTrigger("MoveLeft");
            }
        }

            AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T Component)
    {
        Player HitPlayer = Component as Player;

        animator.SetTrigger("EnemyAttack");
        SoundManager.Instance.RandomiseSFX(EnemyAttack1, EnemyAttack2);

        HitPlayer.SacrificeFriend(PlayerDamage);
    }
}
