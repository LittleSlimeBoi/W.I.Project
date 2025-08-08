using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mainSprite;
    //[SerializeField] private SpriteRenderer destroyedSprite;
    [SerializeField] private SpriteRenderer shadow;
    [SerializeField] private Animator animator;
    private BoxCollider2D bc;

    private void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }

    public void SetShadow(Sprite sprite)
    {
        shadow.sprite = sprite;
    }
    public void SetSprite(Sprite sprite)
    {
        mainSprite.sprite = sprite;

        animator.enabled = false;
    }
    public void SetSprite(AnimationClip clip)
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);

        if (clip != null)
        {
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(aoc.overridesCount);
            aoc.GetOverrides(overrides);
            if (overrides.Count > 0)
            {
                overrides[0] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[0].Key, clip);
                aoc.ApplyOverrides(overrides);

                animator.runtimeAnimatorController = aoc;
            }
        }
    }
}
