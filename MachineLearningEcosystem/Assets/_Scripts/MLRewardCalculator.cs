using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;

public class MLRewardCalculator
{
    // Tunable weights — tweak these to shape behavior
    [SerializeField] private const float SURVIVAL_REWARD = 0.05f;
    [SerializeField] private const float EAT_REWARD = 8.0f;
    [SerializeField] private const float DRINK_REWARD = 8.0f;
    [SerializeField] private const float HUNGER_WARNING = -1.5f;
    [SerializeField] private const float THIRST_WARNING = -1.5f;
    [SerializeField] private const float CRITICAL_PENALTY = -4.0f;
    [SerializeField] private const float DEATH_PENALTY = -30.0f;
    [SerializeField] private const float IDLE_PENALTY = -0.2f;
    [SerializeField] private const float APPROACH_FOOD_REWARD = 0.3f;
    [SerializeField] private const float APPROACH_WATER_REWARD = 0.3f;


    //creature action to disincentivive doing nothing
    //CreatureAction action,
    public float CalculateReward(CreatureMovement creature, 
                                  float previousDistToFood, float previousDistToWater)
    {
        float reward = 0f;

        // --- Survival: small constant reward for staying alive ---
        reward += SURVIVAL_REWARD;

        // --- Eating & Drinking events ---
        if (creature.JustAte) reward += EAT_REWARD;
        if (creature.JustDrank) reward += DRINK_REWARD;

        // --- Hunger urgency scaling ---
        // Linear penalty as hunger drops — creature learns to ACT before crisis
        if (creature.stats.CurrHunger/creature.stats.Hunger < 0.5f)
            reward += HUNGER_WARNING * (0.5f - creature.stats.CurrHunger/creature.stats.Hunger);  // max -0.75 at hunger=0

        if (creature.stats.CurrHunger/creature.stats.Hunger < 0.2f)
            reward += CRITICAL_PENALTY;  // extra kick when near death

        // --- Thirst urgency scaling ---
        if (creature.stats.CurrThirst/creature.stats.Thirst < 0.5f)
            reward += THIRST_WARNING * (0.5f - creature.stats.CurrThirst / creature.stats.Thirst);

        if (creature.stats.CurrThirst / creature.stats.Thirst < 0.2f)
            reward += CRITICAL_PENALTY;

        // --- Death penalty ---
        if (creature.JustDied) reward += DEATH_PENALTY;

        // --- Shaping reward: reward moving TOWARD food when hungry ---
        // This helps the creature learn faster (optional but powerful)
        /*
        if (creature.stats.CurrHunger / creature.stats.Hunger < 0.6f)
        {
            float currentDistToFood = creature.DistanceToNearestFood;
            float improvement = previousDistToFood - currentDistToFood;
            if (improvement > 0) reward += APPROACH_FOOD_REWARD * improvement;
        }

        // --- Shaping reward: reward moving TOWARD water when thirsty ---
        if (creature.Thirst < 0.6f)
        {
            float currentDistToWater = creature.DistanceToNearestWater;
            float improvement = previousDistToWater - currentDistToWater;
            if (improvement > 0) reward += APPROACH_WATER_REWARD * improvement;
        }
        */

        // --- Idle penalty: discourage doing nothing ---
        //if (action == CreatureAction.StayStill)
        //    reward += IDLE_PENALTY;

        return reward;
    }
}
