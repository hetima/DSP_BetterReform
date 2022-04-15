using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace BetterReformMod
{
    public class UIBetterReformPanel : ManualBehaviour
    {

        //public int reformSize;
        public RectTransform rectTrans;

        //public Image reformColorImage;
        //public Image reformColorFloorImage;

        public UIButton type1Button;
        public UIButton type2Button;
        //public UIButton type7Button;

        public UIButton iconTagButton;
        public Image iconTagImage;


        //public static Color highlightColor = new Color(0.188f, 0.8f, 1f, 0.1f);

        public static UIBetterReformPanel CreateInstance()
        {
            UIBetterReformPanel win = null;
            UIBuildMenu buildMenu = UIRoot.instance.uiGame.buildMenu;
            GameObject reformGroup = buildMenu.reformGroup; //base
            UIButton veinBuriedButton = buildMenu.veinBuriedButton;
            UIReformSelect reformSelect = buildMenu.reformSelect;
            GameObject reformTypeGroup = reformSelect.reformTypeGroup; //bg

            GameObject panelGo = GameObject.Instantiate(reformTypeGroup.gameObject);
            panelGo.name = "betterReformPanel";
            win = panelGo.AddComponent<UIBetterReformPanel>();
            win.rectTrans = panelGo.transform as RectTransform;
            win.rectTrans.sizeDelta = new Vector2(600, 60);

            void SetPosition_(RectTransform rect_, float x_, float y_ = 10f)
            {
                if (rect_ != null)
                {
                    rect_.SetParent(reformGroup.transform);
                    rect_.anchorMin = new Vector2(0.5f, 0.5f);
                    rect_.anchorMax = new Vector2(0.5f, 0.5f);
                    rect_.pivot = new Vector2(0.5f, 0.5f);
                    rect_.anchoredPosition3D = new Vector3(x_, y_, 0f);
                }
            }

            //original ui
            RectTransform sandRect = buildMenu.sandPanel.transform as RectTransform;
            SetPosition_(sandRect, 240f);
            sandRect.Find("icon")?.gameObject?.SetActive(false);
            sandRect.SetSiblingIndex(0);
            SetPosition_(buildMenu.reformTypeButton1.transform as RectTransform, 1500f, -500f); //type1,2
            SetPosition_(buildMenu.reformTypeButton0.transform as RectTransform, 190f); //type7
            buildMenu.reformTypeButton0.tips.delay = 0.6f;
            buildMenu.veinBuriedButton.tips.delay = 0.6f;
            Util.RemovePersistentCalls(buildMenu.reformTypeButton0.gameObject);
            buildMenu.reformTypeButton0.onClick += win.OnTypeSelectClick;
            buildMenu.reformTypeButton0.data = 7;
            SetPosition_(reformGroup.transform.Find("sep-line-left-0") as RectTransform, -118f);
            SetPosition_(reformGroup.transform.Find("sep-line-right-0") as RectTransform, -117f);

            SetPosition_(veinBuriedButton.transform as RectTransform, -84f);
            SetPosition_(reformGroup.transform.Find("sep-line-left-1") as RectTransform, -51f);
            SetPosition_(reformGroup.transform.Find("sep-line-right-1") as RectTransform, -50f);

            //SetPosition_(reformGroup.transform.Find("sep-line-left-2") as RectTransform, -40f);
            //SetPosition_(reformGroup.transform.Find("sep-line-right-2") as RectTransform, -39f);
            reformGroup.transform.Find("sep-line-left-2").gameObject.SetActive(false);
            reformGroup.transform.Find("sep-line-right-2").gameObject.SetActive(false);


            //SetPosition_(reformGroup.transform.Find("sep-line-left-3") as RectTransform, 220f);
            //SetPosition_(reformGroup.transform.Find("sep-line-right-3") as RectTransform, 221f);
            reformGroup.transform.Find("sep-line-left-3").gameObject.SetActive(false);
            reformGroup.transform.Find("sep-line-right-3").gameObject.SetActive(false);

            //type select buttons
            UIButton SetupTypeSelectButton_(Sprite s_, int data_, float pos_)
            {
                GameObject go = GameObject.Instantiate(buildMenu.reformTypeButton0.gameObject);
                Util.RemovePersistentCalls(go);
                UIButton btn = go.GetComponent<UIButton>();
                Image image = go.transform.Find("icon")?.gameObject.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = s_;
                }

                btn.onClick += win.OnTypeSelectClick;
                btn.data = data_;
                RectTransform rect = btn.transform as RectTransform;
                SetPosition_(rect, pos_);

                btn.tips.tipTitle = buildMenu.reformTypeButton1.tips.tipTitle;
                btn.tips.tipText = buildMenu.reformTypeButton1.tips.tipText;
                btn.tips.delay = 0.6f;

                go.SetActive(true);
                return btn;
            }

            for (int i = panelGo.transform.childCount - 1; i >= 0 ; i--)
            {
                GameObject child = panelGo.transform.GetChild(i).gameObject;
                if (child.name != "bg")
                {
                    //bt-type-0 bt-type-1
                    if (child.name == "bt-type-0")
                    {
                        UIButton btn = child.GetComponent<UIButton>();
                        Sprite s = null;
                        if (btn != null)
                        {
                            s = btn.transform.Find("icon-0")?.gameObject.GetComponent<Image>()?.sprite;
                        }
                        win.type1Button = SetupTypeSelectButton_(s, 1, 90f);
                        win.type1Button.gameObject.name = "type1Button";
                    }
                    else if (child.name == "bt-type-1")
                    {
                        UIButton btn = child.GetComponent<UIButton>();
                        Sprite s = null;
                        if (btn != null)
                        {
                            s = btn.transform.Find("icon-1")?.gameObject.GetComponent<Image>()?.sprite;
                        }
                        win.type2Button = SetupTypeSelectButton_(s, 2, 140f);
                        win.type2Button.gameObject.name = "type2Button";
                    }
                    //else if (child.name == "bt-type-2")
                    //{
                    //    UIButton btn = child.GetComponent<UIButton>();
                    //    if (btn != null)
                    //    {
                    //        child.name = "type7Button";
                    //        SetupTypeSelectButton_(btn, 7, 200f);
                    //        Image img = child.transform.Find("icon-2")?.GetComponent<Image>();
                    //        if (img != null)
                    //        {
                    //            img.sprite = buildMenu.reformTypeButton0?.transform.Find("icon")?.GetComponent<Image>()?.sprite;
                    //            //img.sprite = GameObject.Find("UI Root/Overlay Canvas/In Game/Function Panel/Build Menu/reform-group/button-reform-0/icon")?.GetComponent<Image>()?.sprite;
                    //        }
                    //        child.SetActive(true);
                    //        win.type7Button = btn;
                    //    }
                    //}
                    else
                    {
                        child.SetActive(false);
                    }
                }
            }

            //color select button
            UIButton reformColorSelectBtn = buildMenu.reformColorSelectBtn;
            if (reformColorSelectBtn != null)
            {
                //GameObject go = GameObject.Instantiate(reformColorSelectBtn.gameObject, reformGroup.transform);
                //go.name = "colorSelectBtn";
                //RectTransform rect = go.transform as RectTransform;
                //rect.sizeDelta = new Vector2(36f, 36f);
                //SetPosition_(rect, 50f);

                //win.reformColorFloorImage = go.GetComponent<Image>();
                //win.reformColorFloorImage.color = new Color(1f, 1f, 1f, 0.1f);
                //win.reformColorImage = go.transform.Find("col-select").GetComponent<Image>();
                //go.SetActive(true);

                RectTransform rect = buildMenu.reformColorSelectBtn.transform as RectTransform;
                rect.sizeDelta = new Vector2(36f, 36f);
                SetPosition_(rect as RectTransform, 40f);
                reformColorSelectBtn.tips.delay = 0.6f;
                RectTransform colorPanelRect = reformSelect.reformColorGroup.transform as RectTransform;
                colorPanelRect.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                colorPanelRect.anchoredPosition = new Vector2(40f, 4f);
            }

            //1x1 10x10 button
            {
                UIButton btn = Util.MakeSmallTextButton("1x", 42, 22, 14);
                SetPosition_(btn.transform as RectTransform, -22, -2);
                btn.transform.transform.localScale = Vector3.one;
                btn.data = 1;
                btn.onClick += win.OnBrushSizeBtnClick;
                btn.onRightClick += win.OnBrushSizeBtnRightClick;
                btn.gameObject.name = "1x1-btn";
                btn.tips.tipTitle = "Brush Size";
                btn.tips.tipText = "Click to set brush size to 1x1.";
                btn.tips.corner = 2;
                btn.tips.offset = new Vector2(0f, -6f);


                btn = Util.MakeSmallTextButton("10x", 42, 22, 14);
                SetPosition_(btn.transform as RectTransform, -22, 22);
                btn.transform.transform.localScale = Vector3.one;
                btn.data = 10;
                btn.onClick += win.OnBrushSizeBtnClick;
                btn.onRightClick += win.OnBrushSizeBtnRightClick;
                btn.gameObject.name = "10x10-btn";
                btn.tips.tipTitle = "Brush Size";
                btn.tips.tipText = "Click to set brush size to 10x10.\nRight-Click to set 20x20.";
                btn.tips.corner = 8;
                btn.tips.offset = new Vector2(0f, 6f);
            }

            panelGo.SetActive(false);

            //win.reformSize = 20;

            return win;
        }

        public void RefreshState()
        {
            BuildTool_Reform reformTool = GameMain.mainPlayer?.controller.actionBuild.reformTool;
            if (reformTool == null)
            {
                return;
            }
           
            //if (reformTool.brushSize == 1)
            //{
            //}
            //else if (reformTool.brushSize == 10)
            //{
            //}
            type1Button.highlighted = (reformTool.brushType == 1);
            type2Button.highlighted = (reformTool.brushType == 2);
            //type7Button.highlighted = (reformTool.brushType == 7);

            //int brushColor = reformTool.brushColor;
            //Color color = Color.clear;
            //if (brushColor < 16)
            //{
            //    color = Configs.builtin.reformColors[brushColor];
            //}
            //else
            //{
            //    PlatformSystem platformSystem = reformTool.factory.platformSystem;
            //    if (platformSystem != null)
            //    {
            //        color = platformSystem.reformCustomColors[brushColor - 16];
            //    }
            //}
            //if (reformColorImage != null)
            //{
            //    if (color.a == 0f)
            //    {
            //        color.a = 1f;

            //    }
            //    else if (color.a<0.5f)
            //    {
            //        color.a = 0.5f;
            //    }
            //    else
            //    {
            //        color.a += (1f - color.a) / 2;
            //    }
            //    reformColorImage.color = color;
            //}
            //if (reformColorFloorImage != null)
            //{
            //    color.a = 1f;
            //    //reformColorFloorImage.color = color;
            //}

        }


        public bool IsStampReady()
        {
            return false;
        }

        public void OnBrushSizeBtnClick(int obj)
        {
            BuildTool_Reform reformTool = GameMain.mainPlayer?.controller.actionBuild.reformTool;
            if (reformTool != null && obj > 0)
            {
                reformTool.brushSize = obj;
                BetterReform.HoldFoundationInHands();
            }
        }

        public void OnBrushSizeBtnRightClick(int obj)
        {
            BuildTool_Reform reformTool = GameMain.mainPlayer?.controller.actionBuild.reformTool;
            if (reformTool != null && obj > 0)
            {
                reformTool.brushSize = obj * 2;
                BetterReform.HoldFoundationInHands();
            }
        }

        public void OnTypeSelectClick(int data)
        {
            UIBuildMenu buildMenu = UIRoot.instance.uiGame.buildMenu;
            if (data == 7)
            {
                buildMenu.OnReformTypeRemoveClick(7);
            }
            else
            {
                BuildTool_Reform reformTool = GameMain.mainPlayer?.controller.actionBuild.reformTool;
                if (reformTool != null)
                {
                    reformTool.brushType = data;
                }
                //OnReformTypeSelectClick で reformTypeGroup の開け閉めをしてるので開いておく
                //buildMenu.OnReformTypeSelectClick();
                //if (buildMenu.reformSelect.reformTypeGroup.activeSelf)
                //{
                //    buildMenu.reformSelect.CloseReformType();
                //}
                BetterReform.HoldFoundationInHands();
            }
            RefreshState();

        }

    }
}
