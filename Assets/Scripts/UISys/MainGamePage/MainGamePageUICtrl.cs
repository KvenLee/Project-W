using UnityEngine;
using UnityEngine.UI;

/// Author:Lee
/// Date:2017-3-6
/// Func:主城界面View层

public class MainGamePageUICtrl : BaseUICtrl
{
    [Header("人物头像框变量")]
    public Image PlayerHeadImg;
    public Text PlayerNameTxt;
    public Text PlayerVipTxt;
    public Button ImproveVipBtn;
    public Slider PlayerExpSlider;
    public Text PlayerExpTxt;
    public Text PlayerLvTxt;

    [Header("模块入口变量")]
    public Button PlayerModuleBtn;
    public Button TeamModuleBtn;
    public Button BagModuleBtn;
    public Button TaskModuleBtn;
    public Button RecruitModuleBtn;
    public Button WorldModuleBtn;
    public Button CalendarModuleBtn;
    public Button ChestModuleBtn;
    public Button MapModuleBtn;

}
