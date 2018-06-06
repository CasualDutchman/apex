using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponents : MonoBehaviour {

    [Header("Screens")]
    public GameObject screenHUD;
    public GameObject screenAlpha, screenSettings, screenBegin;

    [Header("Begin")]
    public Transform textBeginTitle;
    public Transform textBeginExplain;
    public Transform buttonBackBegin;

    [Header("HUD")]
    [Header("Icons")]
    public GameObject iconParent;
    public Transform imageHealthFillIcon;
    public Transform imageFoodBarFillIcon;
    public Transform imageExperienceBarFillIcon;
    public Transform textLevelIcon;
    [Header("Bars")]
    public GameObject barParent;
    public Transform imageHealthFillBar;
    public Transform imageFoodBarFillBar;
    public Transform imageExperienceBarFillBar;
    public Transform textExperience;
    public Transform textLevelBar;
    public Transform buttonSettings;
    public Transform buttonAlpha;

    //[Header("Wolf Pack Screen")]

    [Header("Alpha Screen")]
    public Transform textAlphaTitle;
    public Transform skillA1;
    public Transform skillA2;
    public Transform skillB1;
    public Transform skillB2;
    public Transform skillB3;
    public Transform skillDescriptionBox;
    public Transform skillDescTitle;
    public Transform skillDescDesc;
    public Transform skillDescReq;
    public Transform skillDescBack;
    public Transform buttonBackAlpha;

    [Header("Settings Screen")]
    public Transform textSettingsTitle;
    public Transform textLanguage;
    public Transform textGraphical;
    public Transform textGraphicalLow;
    public Transform textGraphicalHigh;
    public Transform textAudio;
    public Transform textAudioOn;
    public Transform textAudioOff;
    public Transform buttonGraphicsLow;
    public Transform buttonGraphicsHigh;
    public Transform buttonAudio;
    public Transform buttonBackSettings;
    public Transform buttonEnglish;
    public Transform buttonDutch;
    public Transform buttonQuitDelete;
}
