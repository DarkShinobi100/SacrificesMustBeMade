using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {
    public int PlayerDamage;

    private Animator animator;
    private Transform Target;
    private bool SkipMove;

    public AudioClip EnemyAttack1;
    public AudioClip EnemyAttack2;

	// Use this for initialization
	protected override void Start ()
    {
        GameManager.Instance.AddEnemiesToList(this);
        animator = GetComponent<Animator>();
        Target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();

	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        ////Check if skipMove is true, if so set it to false and skip this turn.
        if (SkipMove)
        {
            SkipMove = false;

            return;
        }



        //Call the AttemptMove function from MovingObject.
        base.AttemptMove<T>(xDir, yDir);

        ////Now that Enemy has moved, set skipMove to true to skip next move.
        SkipMove = true;
       
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(Target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = Target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = Target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove<Player>(xDir, yDir);

    }

    protected override void OnCantMove<T>(T Component)
    {
        Player HitPlayer = Component as Player;

        animator.SetTrigger("EnemyAttack");
     //   SoundManager.Instance.RandomiseSFX(EnemyAttack1, EnemyAttack2);

        HitPlayer.LoseFood(PlayerDamage);
    }
}
