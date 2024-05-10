using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LifeEnnemyComponent))]
public class EnnemyTemplate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected RSE_EnnemyRequest _rseRequestTargetLink;
    [SerializeField] protected SCO_EnemyCharacteristic characteristic;
    [SerializeField] protected LifeSystem lifeSystem;

    protected Vector3 targetPos;
    protected GameObject target;

    [SerializeField] Animator anim;
    public StateEnnemy currentState { get; protected set; } = StateEnnemy.Walk;

    public AudioClip[] attackSounds;

    protected virtual void Awake()
    {
        if (characteristic == null) throw new ArgumentNullException("Caracteristic monster not assign");
    }

    protected void Start()
    {
        Wait();
        ((LifeEnnemyComponent)lifeSystem).onDeath += OnDeath;
        GameStateManager.OnPaused += FreezeEnnemy;
        GameStateManager.OnLoose += FreezeEnnemy;
        GameStateManager.OnGameplay += RefreshEnnemy;
    }

    protected void OnDestroy()
    {
        GameStateManager.OnPaused -= FreezeEnnemy;
        GameStateManager.OnLoose -= FreezeEnnemy;
        GameStateManager.OnGameplay -= RefreshEnnemy;
        ((LifeEnnemyComponent)lifeSystem).onDeath -= OnDeath;
    }

    protected void OnDisable()
    {
        transform.DOKill();
        StopAllCoroutines();
    }

    protected void FreezeEnnemy()
    {
        anim.SetBool("Running",false);
        if (this?.gameObject)
            this.enabled = false;
    }

    protected void RefreshEnnemy()
    {
        //print(this.name + " refreshed");
        this.enabled = true;
        Wait(true);
    }


    public virtual void SelectTarget(Vector3 newTarget, GameObject target)
    {
        StartCoroutine(Delay(() => 
        {
            if (target != null)
            {
                targetPos = newTarget;

                targetPos += (transform.position - newTarget).normalized * characteristic.rangeAttack;

                this.target = target;

                MoveToTarget();
            }
            else
            {
                currentState = StateEnnemy.Death;
                Wait();
            }
        }
        , UnityEngine.Random.Range(0.15f, 1f)));
    }
    
    protected virtual void Attack()
    {
        if (target != null)
        {
            AudioManager.instance.PlayClipAt(attackSounds.ToList().GetRandom(), 1, transform.position);

            if (target.TryGetComponent<IDamage>(out IDamage component)) component.TakeDamage(characteristic.attackDamage);
            else Debug.LogWarning("hit miss");
        }
        else
        {
            currentState = StateEnnemy.Walk;
        }

        Wait();
    }

    protected virtual void Wait(bool isRefresh = false)
    {
        if (!this.enabled) return;
        Action action;
        float time;
        switch (currentState)
        {
            case StateEnnemy.Attack:
                Animate("Running", false);
                action = () => { Animate("Attack"); Attack(); };
                time = characteristic.cooldownAttack;
                break;
            case StateEnnemy.Walk:
                action = () => { Animate("Running", true); _rseRequestTargetLink?.Call(this);};
                time = characteristic.cooldownAttack;
                break;
            default: //Death case
                OnDeath();
                action = () => Destroy(gameObject);
                time = 0.5f;
                break;
        }
        
        StartCoroutine(Delay(action, isRefresh? 0f:time));
    }

    protected virtual void MoveToTarget()
    {
        transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
        float time = Vector3.Distance(targetPos, transform.position) / characteristic.walkSpeed;
        transform.DOMove(targetPos, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (Vector3.Distance(transform.position, Vector3.zero) <= 0.1f && target == null)
            {
                currentState = StateEnnemy.Death;
            }
            else
            {
                currentState = StateEnnemy.Attack;
            }

            Wait();
        });
    }

    protected void Animate(string name = "")
    {
        if (anim) anim.SetTrigger(name);
    }

    protected void Animate(string name = "", bool value = false)
    {
        if (anim) anim.SetBool(name, value);
    }

    protected void OnDeath()
    {
        StopAllCoroutines();
        Animate("Die");
    }

    #region FunctionTool

    public static IEnumerator Delay(Action ev, float delay)
    {
        yield return new WaitForSeconds(delay);
        ev?.Invoke();
    }

    public GameObject FindNearestObjectByTag(GameObject currentSelection, string tag)
    {
        List<GameObject> list = GameObject.FindGameObjectsWithTag(tag).ToList();
        if (list.Count != 0)
        {
            list.OrderBy(obj => (currentSelection.transform.position - obj.transform.position).sqrMagnitude);
            return list.First();
        }
        else { return null; }
    }
    #endregion
}

[System.Serializable]
public enum StateEnnemy
{
    Attack,
    Walk,
    Death,
}
