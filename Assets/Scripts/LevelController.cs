using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {

    public Button stage2;
    public Button stage3;
    public Text txt_stage2;
    public Text txt_stage3;

    private int levelStats;

    // Update is called once per frame
    void Start() {
        levelStats = PlayerPrefs.GetInt("levelStage", 1);

        if (levelStats >= 2) { stage2.interactable = true; txt_stage2.color = Color.white; }
        if (levelStats >= 3) { stage3.interactable = true; txt_stage3.color = Color.white; }
    }
}
