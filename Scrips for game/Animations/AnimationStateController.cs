using UnityEngine;

public class AnimationStats : MonoBehaviour
{
    private Animator animator;
    private SpectatorState currentState;
    private float stateTimer;
    
    private enum SpectatorState 
    {
        Cheering,
        Clapping,
        Idle
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        PickRandomState();
    }
    void Update()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            PickRandomState();
        }
    }

    void PickRandomState()
    {
        int randomIndex = Random.Range(0, 3);
        currentState = (SpectatorState)randomIndex; // picks a state
        stateTimer = Random.Range(5f, 10f);
        ApplyState();
        //Debug.Log($"This is the state of {currentState}");
    }

    void ApplyState()
    {
        float randomOffset = Random.Range(0f, 4f) * 0.25f; // 0% to 100% of the animation | the 0.2 is to off set it in % of the animation, its so the animation starts at a random spot, asked for it to be per 20% because it will look a bit better

        if (currentState == SpectatorState.Cheering)
        {

            animator.Play("Rally", 0, randomOffset); // state name must match Animator
        }
        else if (currentState == SpectatorState.Clapping)
        {
 
            animator.Play("Excited", 0, randomOffset);
        }
        else if (currentState == SpectatorState.Idle)
        {

            animator.Play("HappyIdle", 0, randomOffset);
        }
    }
}