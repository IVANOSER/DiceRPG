using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BattleDiceWeightedRoll : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private DiceRollAnimator animator;

    [Header("Debug")]
    [SerializeField] private bool logTable = true;

    public void Roll()
    {
        if (TurnManager.Instance == null)
            return;

        if (!TurnManager.Instance.CanRollDice())
            return;

        
        TurnManager.Instance.MarkRolled();


        SkillSO skill = PickSkillWeighted(out string dbg);
        if (logTable) Debug.Log(dbg);

        if (animator != null)
        {
            animator.Play(skill, () =>
            {
                TurnManager.Instance.SetPendingSkill(skill);
            });
        }
        else
        {
            TurnManager.Instance.SetPendingSkill(skill);
        }
    }


    private SkillSO PickSkillWeighted(out string debugText)
    {
        var rt = DiceLoadoutRuntime.Instance;
        if (rt == null || rt.SkillFaces == null || rt.SkillFaces.Length == 0)
        {
            debugText = "[BattleDice] DiceLoadoutRuntime missing or empty.";
            return null;
        }

        var entries = rt.SkillFaces
            .Where(s => s != null)
            .GroupBy(s => s)
            .Select(g =>
            {
                int count = g.Count();
                int w = Mathf.Max(0, g.Key.dropWeight);
                int total = count * w;
                return new Entry(g.Key, count, w, total);
            })
            .Where(e => e.totalWeight > 0)
            .OrderByDescending(e => e.totalWeight)
            .ToList();

        if (entries.Count == 0)
        {
            debugText = "[BattleDice] No eligible skills (all null or dropWeight=0).";
            return null;
        }

        int totalWeight = entries.Sum(e => e.totalWeight);
        int roll = Random.Range(1, totalWeight + 1);

        int acc = 0;
        SkillSO chosen = null;

        foreach (var e in entries)
        {
            acc += e.totalWeight;
            if (roll <= acc)
            {
                chosen = e.skill;
                break;
            }
        }

        debugText = BuildDebug(entries, totalWeight, roll, chosen);
        return chosen;
    }

    private string BuildDebug(List<Entry> entries, int totalWeight, int roll, SkillSO chosen)
    {
        string chosenName = chosen ? chosen.name : "NULL";
        var s = $"[BattleDice] total={totalWeight}, roll={roll} => CHOSEN={chosenName}\n";

        int acc = 0;
        foreach (var e in entries)
        {
            int from = acc + 1;
            acc += e.totalWeight;
            int to = acc;

            float pct = (totalWeight > 0) ? (e.totalWeight * 100f / totalWeight) : 0f;
            s += $" - {e.skill.name}: count={e.count}, dropWeight={e.dropWeight}, totalWeight={e.totalWeight}, pct={pct:0.##}%, range=[{from}-{to}]\n";
        }

        return s;
    }

    private readonly struct Entry
    {
        public readonly SkillSO skill;
        public readonly int count;
        public readonly int dropWeight;
        public readonly int totalWeight;

        public Entry(SkillSO skill, int count, int dropWeight, int totalWeight)
        {
            this.skill = skill;
            this.count = count;
            this.dropWeight = dropWeight;
            this.totalWeight = totalWeight;
        }
    }

    public void Reroll()
    {
        if (TurnManager.Instance == null) return;
        if (animator != null && animator.IsPlaying) return;

        
        if (!TurnManager.Instance.CanReroll())
            return;

        TurnManager.Instance.ConsumeReroll();

        SkillSO skill = PickSkillWeighted(out string dbg);
        if (logTable) Debug.Log("[REROLL]\n" + dbg);

        if (animator != null)
        {
            animator.Play(skill, () =>
            {
                TurnManager.Instance.SetPendingSkill(skill);
            });
        }
        else
        {
            TurnManager.Instance.SetPendingSkill(skill);
        }
    }

}
