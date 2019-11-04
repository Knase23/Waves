using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Holds the score for a spesific gameobject
/// </summary>
public class ScoreHandler : MonoBehaviour
{

    /// <summary>
    /// Discord Id of the score for paried member
    /// </summary>
    public long Id = 0;

    /// <summary>
    /// The amount of score/points gatherd
    /// </summary>
    public int score = 0;

    public delegate void ScoreChanged(int score);
    public ScoreChanged OnScoreChanged;

    // Start is called before the first frame update
    void Start()
    {
        InputHandler inputHandler = GetComponent<InputHandler>();

        if (inputHandler != null)
            SetMemberId(inputHandler.UserId);

    }
    /// <summary>
    /// Set the MemberID witch this is paired with
    /// </summary>
    /// <param name="id"></param>
    public void SetMemberId(long id)
    {
        if (Id == id)
            return;

        Id = id;

        ScoreSystem.Instance.AddPair(id, this);

        //We need to clear the OnScoreChanged, and find new elements to subscirbe.
        OnScoreChanged = null;

        if (Id == DiscordManager.CurrentUser.Id)
        {
            // Find the UI for the currentUsers score display. 
            // Add then a function from that element that updateds the displayed score

        }
        // Find Leaderboard UI Element for the ID;
        // Add then a function from that element that updateds the displayed score
    }

    /// <summary>
    /// Add a amount of points to the score
    /// </summary>
    /// <param name="amount"></param>
    public void AddScore(int amount)
    {
        if (amount < 0)
        {
            amount = -amount;
            //Converts the amount to a positive value.
        }
        SetScore(score + amount);
    }

    /// <summary>
    /// Sets the score
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score)
    {
        this.score = score;
        OnScoreChanged?.Invoke(this.score);
    }

}
