using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class IntroductionScript : MonoBehaviour
{
    private string text = @"<b>How many ways can you create a volume of __unit__ units with the given tools?</b>

<b><i>Completed:</i></b>        <b><i>Number of ways:</i></b> <color=__color__>__totalWays__</color>
__list__";
    public GameObject challengesTextGameObject;
    public GameObject hintTimerGameObject;
    private Text hintTimerText;
    private int numberOfWaysComplete;
    private Text challengesText;
    private bool shouldUpdate = false;
    private bool shouldGiveHint = true;
    [Tooltip("Time in seconds before hint is shown"), Range(0, 180)]
    public float durationBeforeHint = 60;
    private float currentDuration = 0;

    // Replacements
    private HashSet<HashSet<int>> finished = new HashSet<HashSet<int>>(HashSet<int>.CreateSetComparer());
    public int numberOfUnitsToMake = 12;

    void Awake()
    {
        if (challengesTextGameObject == null) {
            Debug.LogWarning($"Please populate challenges text game object in the {GetType()} on the {gameObject.name} game object. Defaulting to child object named 'Challenge'");
            challengesTextGameObject = GameObject.Find("Challenge").gameObject;
        }

        if (hintTimerGameObject == null) {
            Debug.LogWarning($"Please populate hint text game object in the {GetType()} on the {gameObject.name} game object. Defaulting to child object named 'HintTimer'");
            hintTimerGameObject = GameObject.Find("HintTimer").gameObject;
        }

        Initialize();
    }

    void Update()
    {
        if (!shouldGiveHint)
            currentDuration += Time.deltaTime;

        if (currentDuration >= durationBeforeHint) {
            currentDuration = 0;
            shouldGiveHint = true;
            challengesText.text = GetText();
        }

        hintTimerText.text = GiveHint();
    }

    void OnValidate()
    {
        Initialize();
    }

    void Initialize()
    {
        challengesText = challengesTextGameObject.GetComponent<Text>();
        hintTimerText = hintTimerGameObject.GetComponent<Text>();
        shouldUpdate = false;
        numberOfWaysComplete = HowManyWaysToGetAVolumeOf(numberOfUnitsToMake);
        challengesText.text = GetText();
    }

    private bool isFinished()
    {
        return finished.Count >= numberOfWaysComplete;
    }

    private int HowManyWaysToGetAVolumeOf(int num)
    {
        return GetAllWays(num).Count;
    }

    private HashSet<HashSet<int>> GetAllWays(int num)
    {
         HashSet<HashSet<int>> factors = new HashSet<HashSet<int>>(HashSet<int>.CreateSetComparer());
        int x = num;
        while (x > 0)
        {
            if (num % x == 0)
            {
                int y = num / x;
                foreach (HashSet<int> s in GetAllFactorsOf(y)) {
                    s.Add(x);
                    factors.Add(s);
                }
            }
            x--;
        }
        return factors;
    }

    private HashSet<HashSet<int>> GetAllFactorsOf(int num)
    {
        HashSet<HashSet<int>> factors = new HashSet<HashSet<int>>(HashSet<int>.CreateSetComparer());
        int x = num;
        while (x > 0) {
            if (num % x == 0)
            {
                factors.Add(new HashSet<int>{num/x, x});
            }
            x--;
        }
        return factors;
    }

    private string GetText()
    {
        string list = ListToText(SetToList(finished));
        return text
            .Replace("__unit__", numberOfUnitsToMake.ToString())
            .Replace("__color__", isFinished() ? "green" : "red")
            .Replace("__totalWays__", finished.Count.ToString())
            .Replace("__list__", list);
    }

    private string GiveHint()
    {
        if (!shouldGiveHint) return $"Hint coming in ~{Mathf.RoundToInt(durationBeforeHint - currentDuration)} seconds";
        List<int[]> hints = GetAllHints();
        return $"Try a shape with the volume {hints[0][0]} x {hints[0][1]} x {hints[0][2]}.";
    }

    private List<int[]> GetAllHints()
    {
        var factors = GetAllWays(numberOfUnitsToMake);

        foreach (HashSet<int> hs in finished)
        {
            factors.Remove(hs);
        }

        return SetToList(factors);
    }

    private List<int[]> SetToList(HashSet<HashSet<int>> set)
    {
        List<int[]> l = new List<int[]>();
        foreach (HashSet<int> hs in set)
        {
            if (hs.ElementAtOrDefault(1) == 0) {
                l.Add(new int[]{hs.ElementAt(0), hs.ElementAt(0), hs.ElementAt(0)});
            }
            else if (hs.ElementAtOrDefault(2) == 0) {
                l.Add(new int[]{hs.ElementAt(0), hs.ElementAt(1), numberOfUnitsToMake / hs.ElementAt(0) / hs.ElementAt(1)});
            }
            else {
                l.Add(hs.ToArray());
            }
        }

        return l;
    }

    private string ListToText(List<int[]> l)
    {
        List<string> strList = new List<string>();
        foreach (int[] item in l)
        {
            strList.Add($"{item[0]} x {item[1]} x {item[2]}");
        }
        return string.Join(", ", strList);
    }

    public void CheckVolume(Transform volume, float gridSize)
    {
        print($"volume: {Mathf.RoundToInt(volume.localScale.x * volume.localScale.y * volume.localScale.z * Mathf.Pow(gridSize, 3))}");
        if (Mathf.RoundToInt(volume.localScale.x * volume.localScale.y * volume.localScale.z * Mathf.Pow(gridSize, 3)) == numberOfUnitsToMake)
        {
            shouldGiveHint = false;
            finished.Add(new HashSet<int>{Mathf.RoundToInt(volume.localScale.x * gridSize), Mathf.RoundToInt(volume.localScale.y * gridSize), Mathf.RoundToInt(volume.localScale.z * gridSize)});
            challengesText.text = GetText();

        }
    }
}
