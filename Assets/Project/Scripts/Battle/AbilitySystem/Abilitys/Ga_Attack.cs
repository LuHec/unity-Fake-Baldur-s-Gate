using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Ga_Attack : AbilityBase
{
    private bool attackEnd;
    public Ga_Attack(GameActor owner) : base(owner)
    {
        name = "Ga_Attack";
    }

    public override void OnActive()
    {
        
    }

    public override void OnFinished()
    {
        
    }

    public override void ActiveAbility(Action onAbilityActive, Action onAbilityEnd)
    {
        onActive?.Invoke();
        onAbilityActive?.Invoke();
        owner.StartCoroutine(AttackCoroutine(onAbilityEnd));
    }
    
    private IEnumerator AttackCoroutine(Action onAbilityEnd = null)
    {
        // Attack Start
        owner.Attack(owner, () => { attackEnd = true;});
        
        // wait Attack Finished
        while(attackEnd == false)
            yield return null;
        
        // Create Mo
        ((Character)owner).abilitySystem.TryApplyModifier(ModifierPool.Instance.CreateModifier("Mo_Decrease_Hp", owner, owner));
        
        onAbilityEnd?.Invoke();
    }
}