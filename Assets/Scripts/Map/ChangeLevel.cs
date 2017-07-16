using UnityEngine;

public class ChangeLevel : MonoBehaviour {

    public string nextLevelName;
    [HideInInspector] public bool changingInProgress;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!changingInProgress)
        {
            Game.instance.StartChangingLevel(nextLevelName);
        }
    }
}
