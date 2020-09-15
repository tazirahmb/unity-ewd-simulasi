using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // === START OF PUBLIC === //

    //============================================= Integer
    public int level;

    //============================================= Audio
    public AudioSource au;
    public AudioClip au_True;
    public AudioClip au_False;
    
    //============================================= Asset Text
    public Text txt_Score;
    public Text txt_ttlScore;
    public Text txt_ttlWrong;
    public Text txt_feedback;
    public Text txt_hiScore;
    public Text txt_linesRemaining;

    //============================================= GameObject
    public GameObject winPanel;
    public GameObject gamePanel;
    public GameObject hi_Wrong;
    public GameObject hi_Right;
    public GameObject hi_Selected;

    //============================================= GameObject yang ditampung
    public GameObject[] lineAsset; //semua selain easy
    public GameObject[] tabelSwitch; //khusus hard
    public GameObject[] tabel; //khusus hard

    // === END OF PUBLIC === //

    // === START OF PRIVATE === //

    //============================================= GameObject dan Komponen
    private DiagramContainer diagramContainer;
    private GameObject go;

    //============================================= Boolean
    private bool isFound = false;
    private bool rightCable;
    static bool feedbackIsWrited;

    //============================================= Integer
    private int score;
    private int hiScore;
    private int hiWrong;
    private int nLine;
    private int lineTotal;
    private int wrong;
    private int sortInt;

    //============================================= String
    private string tag1, tag2;
    private string komtag1, komtag2;
    private string lineName;
    private string check1, check2;
    private string sortString;
    private string conditionCable;

    //============================================= Arrays
    private int[] nTag;
    private string[] nCheck;

    // === END OF PRIVATE === //

    void Start () {
        //getting content from database
        diagramContainer = GetComponent<DiagramContainer>();

        //setting cable default color (only in medium and hard)
        if (level > 1) { conditionCable = "positive"; }

        //feedback component
        nTag = new int[5];
        for (int i = 0; i < nTag.Length; i++) { nTag[i] = 0; }
        
        //adding component name for feedback
        nCheck = new string[5];
        nCheck[0] = "Baterai";
        nCheck[1] = "Pengaman";
        nCheck[2] = "Switch";
        nCheck[3] = "Relay";
        nCheck[4] = "Beban";
        feedbackIsWrited = false;

        //set total score, wrong, and high score
        score = wrong = 0;
        hiScore = PlayerPrefs.GetInt("hiScore_0" + level.ToString(), 0);
        hiWrong = PlayerPrefs.GetInt("hiWrong_0" + level.ToString(), 0);
        Debug.Log(hiScore);
        Debug.Log(hiWrong);

        //finding array of gameObject with specific tag, then hide it
        lineAsset = GameObject.FindGameObjectsWithTag("Line");
        lineTotal = lineAsset.Length;
        nLine = lineTotal;
        txt_linesRemaining.text = "Sisa kabel : " + nLine;
        foreach (GameObject line in lineAsset) line.SetActive(false);

        if(level == 3) //finding table for switch
        {
            tabel = GameObject.FindGameObjectsWithTag("Tabel");
            tabelSwitch = GameObject.FindGameObjectsWithTag("TabelSwitch");
            foreach (GameObject tabel in tabelSwitch) tabel.SetActive(false);
        }

        //hiding gamepanel
        gamePanel.SetActive(false);
	} //done
	
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
        if (nLine == 0) {
            winPanel.SetActive(true);
            gamePanel.SetActive(false);
            txt_ttlScore.text = "Skor : " + score;
            txt_ttlWrong.text = "Total Salah : " + wrong;
            if(!feedbackIsWrited)
            {
                txt_hiScore.text = WriteHiScore(score, wrong, hiScore, hiWrong);
                txt_feedback.text = WriteFeedback();
                feedbackIsWrited = true;
            }
            if (PlayerPrefs.GetInt("levelStage") == 3) { return; }
            else if (PlayerPrefs.GetInt("levelStage") < level + 1) { PlayerPrefs.SetInt("levelStage", level + 1); }
        }
        
    } //done
    
    private void CastRay()
    {
        //geting ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit)
        {
            //=================================================== karena tag tabel dan tabelswitch cuma ada di stage 3, maka otomatis bakalan keskip kalo di stage 1 dan 2
            if (hit.collider.tag == "Tabel")
            {
                foreach (GameObject tabel in tabelSwitch)
                {
                    if ("tbl_" + hit.collider.name == tabel.name)
                    {
                        hit.collider.gameObject.SetActive(false);
                        tabel.SetActive(true);
                        break;
                    }
                }
            }
            else if (hit.collider.tag == "TabelSwitch")
            {
                for (int i = 0; i < tabel.Length; i++)
                {
                    if (hit.collider.name.Substring(4) == tabel[i].name)
                    {
                        hit.collider.gameObject.SetActive(false);
                        tabel[i].SetActive(true);
                        break;
                    }
                }
            }
            //=================================================== karena tag tabel dan tabelswitch cuma ada di stage 3, maka otomatis bakalan keskip kalo di stage 1 dan 2

            else if (tag1 == null) //deteksi apakah objek pertama udah disentuh
            {
                hi_Selected.transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z);
                hi_Selected.SetActive(true);
                tag1 = hit.collider.name; //ambil nama objek yang disentuh
                komtag1 = hit.collider.tag; //ambil tag objek yang disentuh
                Debug.Log("tag 1 : " + tag1);
            }
            else
            {
                if (hit.collider.name != tag1)
                {
                    tag2 = hit.collider.name; ; //ambil objek yang disentuh kedua
                    komtag2 = hit.collider.tag;
                    Debug.Log("tag 2 : " + tag2);
                    CheckDatabase(tag1, tag2, komtag1, komtag2);
                                       
                 }
                else
                {
                    Debug.Log("tagnya sama tuh...");
                }

                hi_Selected.SetActive(false);
                tag1 = tag2 = komtag1 = komtag2 = null; //kalo udah dibandingin, kedua variabel tag dibikin null
                
            }

        }
    } //done

    private void CheckDatabase(string tag1, string tag2, string komtag1, string komtag2)
    {
        isFound = false;
        for(int i = 0; i < lineTotal; i++)
        {
            //checking if this level has cable
            if (diagramContainer.CableHere)
            {
                if (conditionCable == diagramContainer.cond[i])
                {
                    rightCable = true;
                }
                else
                {
                    rightCable = false;
                }
            }
            else
            {
                rightCable = true;
            }

            if (tag1 == diagramContainer.tag1[i] && tag2 == diagramContainer.tag2[i] && rightCable) //detect tag from database
            {
                isFound = true;
                //Debug.Log("yey, kamu benar");
                lineName = "line_" + tag1 + "_" + tag2;
                foreach (GameObject gos in lineAsset)
                {
                    if (gos.name == lineName)
                    {
                        go = gos;
                        break;
                    }
                }
                if (!go.activeSelf)
                {

                    go.SetActive(true);
                    playAudio(au_True);
                    nLine--;
                    txt_linesRemaining.text = "sisa kabel : " + nLine;
                    //Debug.Log("Sisa garis :" + nLine);
                    score += 2;
                    UpdateScore();
                    StartCoroutine(highlight(isFound));
                    break;
                }
            }
        }
        if (!isFound)
        {
            playAudio(au_False);
            wrong++;
            score--;
            UpdateScore();
            FeedbackGenerator(komtag1, komtag2);
            StartCoroutine(highlight(isFound));
            Debug.Log("yaah, kamu salah");
        }
    } //done

    private void playAudio(AudioClip audio)
    {
        au.clip = audio;
        au.Play();
    } //done

    IEnumerator highlight(bool isTrue)
    {
        if (isTrue)
        {
            hi_Right.SetActive(true);
        }
        else
        {
            hi_Wrong.SetActive(true);
        }
        yield return new WaitForSeconds(0.5f);
        if (isTrue)
        {
            hi_Right.SetActive(false);
        }
        else
        {
            hi_Wrong.SetActive(false);
        }
    } //done

    private void FeedbackGenerator(string komtag1, string komtag2)
    {
        while(komtag1 != null)
        {
            if (komtag1 == "Battery") { nTag[0]++; break; }
            else if (komtag1 == "Pengaman") { nTag[1]++; break; }
            else if (komtag1 == "Switch") { nTag[2]++; break; }
            else if (komtag1 == "Relay") { nTag[3]++; break; }
            else if (komtag1 == "Beban") { nTag[4]++; break; }
        }
        while (komtag2 != null)
        {
            if (komtag2 == "Battery") { nTag[0]++; break; }
            else if (komtag2 == "Pengaman") { nTag[1]++; break; }
            else if (komtag2 == "Switch") { nTag[2]++; break; }
            else if (komtag2 == "Relay") { nTag[3]++; break; }
            else if (komtag2 == "Beban") { nTag[4]++; break; }
        }

    } //done

    private void UpdateScore()
    {
        if (score < 0) score = 0;
        txt_Score.text = "Score : " + score;
    } //done

    private string WriteFeedback()
    {
        //=====================================sorting Wrong

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (nTag[i] < nTag[j])
                {
                    //========================sorting int
                    sortInt = nTag[i];
                    nTag[i] = nTag[j];
                    nTag[j] = sortInt;

                    //========================sorting string
                    sortString = nCheck[i];
                    nCheck[i] = nCheck[j];
                    nCheck[j] = sortString;
                }
            }
        }

        //=====================================sorting Wrong
        Debug.Log(nTag[0] + " " + nTag[1] + " " + nTag[2] + " " + nTag[3] + " " + nTag[4]);
        Debug.Log(nCheck[0] + " " + nCheck[1] + " " + nCheck[2] + " " + nCheck[3] + " " + nCheck[4]);
        if (nTag[0] + nTag[1] + nTag[2] + nTag[3] + nTag[4] == 0) return "Skor anda sempurna!";
        else return "Perhatikan hubungan pada komponen " + nCheck[nCheck.Length - 1] + " dan " + nCheck[nCheck.Length - 2];
    } //done

    private string WriteHiScore(int score, int wrong, int hiScore, int hiWrong)
    {
        if (hiScore < score || (hiScore == score && hiWrong < wrong))
        {
            PlayerPrefs.SetInt("hiScore_0" + level.ToString(), score);
            PlayerPrefs.SetInt("hiWrong_0" + level.ToString(), wrong);
            return "Selamat! Anda meraih skor tertinggi.";
        }
        else
        {
            return "Skor tertinggi pada stage ini : " + hiScore + "\ndengan jumlah salah : " + hiWrong;
        }
    } //done

    public void setCable(string cable)
    {
        conditionCable = cable;
    } //done

}