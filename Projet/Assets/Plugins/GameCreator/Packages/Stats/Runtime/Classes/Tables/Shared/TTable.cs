using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Stats
{
    [Title("Table")]
    
    [Serializable]
    public abstract class TTable : ITable
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public virtual int MinLevel => 1;
        public virtual int MaxLevel => 99;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public int GetLevelForCumulativeExperience(int cumulative)
        {
            return this.LevelFromCumulative(cumulative);
        }
        
        public int GetCumulativeExperienceForLevel(int level)
        {
            level = Mathf.Clamp(level, this.MinLevel, this.MaxLevel + 1);
            return this.CumulativeFromLevel(level);
        }
        
        public int GetLevelExperienceForLevel(int level)
        {
            int currLevel = Mathf.Clamp(level + 0, this.MinLevel, this.MaxLevel + 1);
            int nextLevel = Mathf.Clamp(level + 1, this.MinLevel, this.MaxLevel + 1);
            
            int cumulativeCurrent = this.CumulativeFromLevel(currLevel);
            int cumulativeNext = this.CumulativeFromLevel(nextLevel);

            return cumulativeNext - cumulativeCurrent;
        }
        
        public int GetLevelExperienceAtCurrentLevel(int cumulative)
        {
            int currentLevel = this.GetLevelForCumulativeExperience(cumulative);
            return cumulative - this.CumulativeFromLevel(currentLevel);
        }
        
        public int GetLevelExperienceToNextLevel(int cumulative)
        {
            int nextLevel = this.GetNextLevel(cumulative);
            return this.CumulativeFromLevel(nextLevel) - cumulative;
        }

        public float GetRatioAtCurrentLevel(int cumulative)
        {
            int experienceFrom = this.GetLevelExperienceAtCurrentLevel(cumulative);
            int experienceTo = this.GetLevelExperienceToNextLevel(cumulative);
            return (float) experienceFrom / (experienceFrom + experienceTo);
        }
        
        public float GetRatioForNextLevel(int cumulative)
        {
            return 1f - this.GetRatioAtCurrentLevel(cumulative);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected int GetPreviousLevel(int cumulative)
        {
            int currentLevel = this.LevelFromCumulative(cumulative);
            return Math.Max(this.MinLevel, currentLevel - 1);
        }
        
        protected int GetNextLevel(int cumulative)
        {
            int currentLevel = this.LevelFromCumulative(cumulative);
            return Math.Min(this.MaxLevel, currentLevel + 1);
        }

        // ABSTRACT METHODS: ----------------------------------------------------------------------
        
        protected abstract int LevelFromCumulative(int cumulative);
        protected abstract int CumulativeFromLevel(int level);
    }
}