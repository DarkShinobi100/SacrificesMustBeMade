using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {
    public float MoveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D RB2D;
    private float InverseMoveTime;


    // Use this for initialization
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        RB2D = GetComponent<Rigidbody2D>();
        InverseMoveTime = 1f / MoveTime;
    }

    protected bool Move (int XDir,int YDir, out RaycastHit2D hit)
    {
        Vector2 Start = transform.position;
        Vector2 End = Start + new Vector2(XDir, YDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(Start, End, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(End));
            return true;
        }
     return false;
        
    }

    protected virtual void AttemptMove<T>(int XDir,int YDir)
        where T :Component
    {
        RaycastHit2D Hit;
        bool CanMove = Move(XDir,YDir, out Hit);

        if (Hit.transform == null)
        {
            return;
        }

        T HitComponent = Hit.transform.GetComponent<T>();

        if(!CanMove && HitComponent != null)
        {
            OnCantMove(HitComponent);
        }
      
    }


    protected IEnumerator SmoothMovement(Vector3 End)
    {
        float sqrRemainingDistace = (transform.position - End).sqrMagnitude;

        while (sqrRemainingDistace > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(RB2D.position, End, InverseMoveTime * Time.deltaTime);
            RB2D.MovePosition(newPosition);
            sqrRemainingDistace = (transform.position - End).sqrMagnitude;

            yield return null;

        }

    }

    protected abstract void OnCantMove<T>(T Component)
     where T : Component;
}
