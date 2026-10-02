using System.Collections.Generic;
using UnityEngine;

[System.Serializable]


public class GamerTypeResult
{
    public float KillerScore;
    public float SocializerScore;
    public float AchieverScore;
    public float ExplorerScore;

    public int LawyerScore;
    public int SurgeonScore;
    public int CookScore;

    // Keep the old single-type field for compatibility / fallback
    public GamerType FinalGamerType;

    // New: store all highest-scoring gamer types
    public List<GamerType> FinalGamerTypes = new List<GamerType>();

    public JobType FinalJobType;
    public GamerTypeResult Clone()
    {
        return new GamerTypeResult
        {
            KillerScore     = this.KillerScore,
            SocializerScore = this.SocializerScore,
            AchieverScore   = this.AchieverScore,
            ExplorerScore   = this.ExplorerScore,
            LawyerScore     = this.LawyerScore,
            SurgeonScore    = this.SurgeonScore,
            CookScore       = this.CookScore,
            FinalGamerType  = this.FinalGamerType,
            FinalJobType    = this.FinalJobType
        };
    }
}


public enum GamerType
{
    Killer,
    Socializer,
    Achiever,
    Explorer
}

public enum JobType
{
    Lawyer,
    Surgeon,
    Cook
}

