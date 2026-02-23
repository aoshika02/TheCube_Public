using Cysharp.Threading.Tasks;
using UnityEngine;

public static class AnimatorExtensionsWithTask
{
    private static async UniTask AnimAsync(
        this Animator animator,
        int layer = 0)
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        float animLength = stateInfo.length / animator.speed;
        await UniTask.Delay((int)(animLength * 1000f));
    }
    public static async UniTask TriggerAsync(
       this Animator animator,
       string triggerName,
       int layer = 0)
    {
        animator.SetTrigger(triggerName);

        await animator.AnimAsync(layer);

        animator.ResetTrigger(triggerName);
    }
    public static async UniTask FloatAsync(
        this Animator animator,
        string floatName,
        float value,
        int layer = 0)
    {
        animator.SetFloat(floatName, value);
        await animator.AnimAsync(layer);
    }
    public static async UniTask IntAsync(
        this Animator animator,
        string intName,
        int value,
        int layer = 0)
    {
        animator.SetInteger(intName, value);
        await animator.AnimAsync(layer);
    }

    public static async UniTask BoolAsync(
        this Animator animator,
        string boolName,
        bool value,
        int layer = 0)
    {
        animator.SetBool(boolName, value);

        await animator.AnimAsync(layer);
    }
}
