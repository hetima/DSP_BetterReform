using System;
using System.Collections.Generic;
using System.Text;

using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;


namespace BetterReformMod
{
    public static class Util
    {

        public static Color DSPBlue => new Color(0.3821f, 0.8455f, 1f, 0.7059f);
        public static Color DSPOrange => new Color(0.9906f, 0.5897f, 0.3691f, 0.7059f);

        public static void RemovePersistentCalls(GameObject go)
        {
            Button oldbutton = go.GetComponent<Button>();
            UIButton btn = go.GetComponent<UIButton>();
            if (btn != null && oldbutton != null)
            {
                GameObject.DestroyImmediate(oldbutton);
                btn.button = go.AddComponent<Button>();
            }
        }

        //俺たちは雰囲気でUnity座標系をやっている
        public static RectTransform NormalizeRect(GameObject go, float width = 0, float height = 0)
        {
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMax = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.pivot = Vector2.zero;
            if (width > 0 && height > 0)
            {
                rect.sizeDelta = new Vector2(width, height);
            }
            return rect;
        }

        //offsetでサイズを決める感じ
        public static RectTransform NormalizeRectB(GameObject go)
        {
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMax = Vector2.one;
            rect.anchorMin = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            return rect;
        }

        //中央揃え
        public static RectTransform NormalizeRectC(GameObject go, float width = 0, float height = 0)
        {
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            if (width > 0 && height > 0)
            {
                rect.sizeDelta = new Vector2(width, height);
            }
            return rect;
        }

        //左上
        public static RectTransform NormalizeRectD(GameObject go, float width = 0, float height = 0)
        {
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMax = Vector2.up;
            rect.anchorMin = Vector2.up;
            rect.pivot = Vector2.up;
            if (width > 0 && height > 0)
            {
                rect.sizeDelta = new Vector2(width, height);
            }
            return rect;
        }

        public static Sprite LoadSpriteResource(string path)
        {
            Sprite s = Resources.Load<Sprite>(path);
            if (s != null)
            {
                return s;
            }
            else
            {
                Texture2D t = Resources.Load<Texture2D>(path);
                if (t != null)
                {
                    s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
                    return s;
                }
            }
            return null;
        }
        


        public static UIButton MakeSmallTextButton(string label = "", float width = 0, float height = 0, int fontSize = 12)
        {
            UIAssemblerWindow assemblerWindow = UIRoot.instance.uiGame.assemblerWindow;
            GameObject go = GameObject.Instantiate(assemblerWindow.copyButton.gameObject);
            UIButton btn = go.GetComponent<UIButton>();
            Transform child = go.transform.Find("Text");
            GameObject.DestroyImmediate(child.GetComponent<Localizer>());
            Text txt = child.GetComponent<Text>();
            txt.text = label;
            txt.fontSize = fontSize;
            btn.tips.tipText = "";
            btn.tips.tipTitle = "";
            btn.tips.delay = 0.6f;

            if (width > 0 || height > 0)
            {
                RectTransform rect = (RectTransform)go.transform;
                if (width == 0)
                {
                    width = rect.sizeDelta.x;
                }
                if (height == 0)
                {
                    height = rect.sizeDelta.y;
                }
                rect.sizeDelta = new Vector2(width, height);
            }

            go.transform.localScale = Vector3.one;

            return btn;
        }
        public static UIButton MakeIconButtonB(Sprite sprite, float size = 60)
        {
            GameObject go = GameObject.Instantiate(UIRoot.instance.uiGame.researchQueue.pauseButton.gameObject);
            UIButton btn = go.GetComponent<UIButton>();
            RectTransform rect = (RectTransform)go.transform;
            //rect.sizeDelta = new Vector2(size, size);
            float scale = size / 60;
            rect.localScale = new Vector3(scale, scale, scale);
            Image img = go.transform.Find("icon")?.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = sprite;
            }
            btn.tips.tipText = "";
            btn.tips.tipTitle = "";
            return btn;
        }





        public static UIButton MakeIconButton(Transform parent, Sprite sprite, float posX = 0, float posY = 0, bool right = false, bool bottom = false)
        {

            //GameObject go = GameObject.Find("UI Root/Overlay Canvas/In Game/Research Queue/pause");
            GameObject go = UIRoot.instance.uiGame.researchQueue.pauseButton.gameObject;
            if (go == null) return null;
            UIButton btn = MakeGameObject<UIButton>(parent, go, posX, posY, 0, 0, right, bottom);
            if (btn == null) return null;


            var bg = btn.gameObject.transform.Find("bg");
            if (bg != null) bg.gameObject.SetActive(false);
            var sd = btn.gameObject.transform.Find("sd");
            if (sd != null) sd.gameObject.SetActive(false);

            var icon = btn.gameObject.transform.Find("icon");
            if (sprite != null && icon != null)
            {
                Image img = icon.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = sprite;
                    img.color = new Color(0.94f, 0.74f, 0.24f, 0.6f);
                }
                icon.localScale = new Vector3(1.6f, 1.6f, 1.6f);
            }

            btn.gameObject.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);

            btn.tips.offset = new Vector2(0, -10);
            btn.tips.corner = 0;
            btn.tips.delay = 0.5f;
            btn.tips.tipText = "";
            btn.tips.tipTitle = "";

            return btn;
        }

        public static T MakeGameObject<T>(Transform parent, GameObject src, float posX = 0, float posY = 0, float width = 0, float height = 0, bool right = false, bool bottom = false)
        {
            if (src == null) return default;
            var go = UnityEngine.Object.Instantiate(src);
            if (go == null)
            {
                return default;
            }

            var rect = (RectTransform)go.transform;
            if (rect != null)
            {
                float yAnchor = bottom ? 0 : 1;
                float xAnchor = right ? 1 : 0;
                rect.anchorMax = new Vector2(xAnchor, yAnchor);
                rect.anchorMin = new Vector2(xAnchor, yAnchor);
                rect.pivot = new Vector2(0, 0);
                if (width == -1) width = rect.sizeDelta.x;
                if (height == -1) height = rect.sizeDelta.y;
                if (width > 0 && height > 0)
                {
                    rect.sizeDelta = new Vector2(width, height);
                }
                rect.SetParent(parent, false);
                rect.anchoredPosition = new Vector2(posX, posY);
            }
            return go.GetComponent<T>();
        }

        public static string KMGFormat(long num)
        {
            if (num >= 100_000_000_000_000)
                return (num / 1_000_000_000_000).ToString("#,0T");
            if (num >= 10_000_000_000_000)
                return (num / 1_000_000_000_000).ToString("0.#") + "T";
            if (num >= 100_000_000_000)
                return (num / 1_000_000_000).ToString("#,0G");
            if (num >= 10_000_000_000)
                return (num / 1_000_000_000).ToString("0.#") + "G";
            if (num >= 100_000_000)
                return (num / 1_000_000).ToString("#,0M");
            if (num >= 10_000_000)
                return (num / 1_000_000).ToString("0.#") + "M";
            if (num >= 100_000)
                return (num / 1_000).ToString("#,0K");
            if (num >= 10_000)
                return (num / 1_000).ToString("0.#") + "K";

            return num.ToString("#,0");
        }


        //LoadImage() 使うには UnityEngine.ImageConversionModule.dll を参照に追加する
        public static Sprite StampSprite()
        {
            Texture2D tex = new Texture2D(2, 2);
            byte[] pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x18, 0x08, 0x04, 0x00, 0x00, 0x00, 0x4A, 0x7E, 0xF5, 0x73, 0x00, 0x00, 0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0B, 0x13, 0x00, 0x00, 0x0B, 0x13, 0x01, 0x00, 0x9A, 0x9C, 0x18, 0x00, 0x00, 0x01, 0x40, 0x49, 0x44, 0x41, 0x54, 0x78, 0xDA, 0xA5, 0xD2, 0x03, 0xA8, 0x58, 0x51, 0x1C, 0x80, 0xF1, 0xFB, 0xAC, 0x38, 0xA4, 0xD9, 0x96, 0xC2, 0xAC, 0x30, 0x5B, 0xCA, 0x43, 0x5A, 0xB6, 0x85, 0x65, 0x2E, 0x4B, 0xB3, 0x94, 0x3D, 0x86, 0xD9, 0x52, 0x98, 0x6D, 0xDE, 0xF3, 0x7B, 0xA7, 0xCE, 0xB3, 0xF1, 0x7D, 0x57, 0x38, 0x7F, 0xD5, 0xC9, 0xBA, 0x8D, 0x82, 0x78, 0x56, 0x1A, 0x61, 0xA4, 0xAA, 0x8E, 0x86, 0x2C, 0xF6, 0xD8, 0x67, 0x9F, 0xBC, 0xB1, 0xB0, 0x23, 0xD9, 0x07, 0xA8, 0xE7, 0xBB, 0x39, 0x59, 0xA6, 0xB0, 0xED, 0x80, 0x13, 0x08, 0x92, 0x1C, 0x54, 0x96, 0xBE, 0xB7, 0x1E, 0xF0, 0x0A, 0x01, 0xE4, 0xB8, 0xA2, 0x77, 0xAA, 0xD1, 0x7A, 0xC0, 0xF9, 0xBA, 0x80, 0x80, 0x33, 0xAA, 0xDA, 0xAB, 0xB0, 0x58, 0x3D, 0x7F, 0x6D, 0x4B, 0xF9, 0xDB, 0x40, 0x95, 0x4F, 0x20, 0xE0, 0x65, 0x1B, 0xE3, 0x2A, 0x54, 0x14, 0x2D, 0x50, 0x6A, 0x3F, 0x72, 0x01, 0xC7, 0xB2, 0x4C, 0xB1, 0xF8, 0x5D, 0x61, 0x93, 0xB6, 0x1A, 0xBE, 0xDA, 0x83, 0x1C, 0xEC, 0x6D, 0xB8, 0x42, 0x41, 0xA3, 0x17, 0x7D, 0x8D, 0x36, 0xD6, 0x50, 0xC3, 0x9C, 0x43, 0x88, 0x72, 0xDD, 0x28, 0xC3, 0x8D, 0x35, 0xCE, 0x80, 0xA6, 0x15, 0x76, 0xB9, 0xE4, 0xB1, 0x87, 0x6E, 0xBB, 0xE2, 0x9F, 0x5A, 0x82, 0xFB, 0x6E, 0x79, 0xE4, 0x89, 0x9B, 0x76, 0x28, 0x68, 0xB8, 0xFC, 0x8F, 0xF6, 0xF8, 0x6E, 0x77, 0xED, 0xF2, 0x9E, 0xDE, 0xE2, 0xBF, 0x5C, 0xB2, 0x21, 0xB5, 0xDF, 0xFE, 0xE1, 0xB9, 0x71, 0x29, 0x60, 0xAC, 0x2F, 0xC8, 0x05, 0xD1, 0xDA, 0x7B, 0x13, 0xFF, 0xE3, 0x8D, 0x65, 0x29, 0x60, 0x92, 0x6F, 0x3A, 0xC2, 0x47, 0x1B, 0x52, 0x40, 0x7F, 0x2F, 0xF1, 0xA5, 0x1D, 0xB9, 0x63, 0x46, 0x0A, 0x28, 0x34, 0xCF, 0xC6, 0x78, 0xCE, 0x8A, 0xCE, 0x6C, 0xC5, 0x79, 0xD6, 0x9B, 0xA6, 0x24, 0xAB, 0xC5, 0x14, 0x13, 0x8D, 0x68, 0xC5, 0xE1, 0xD1, 0xA9, 0xA6, 0x66, 0xF5, 0xA8, 0xD0, 0x3E, 0x1F, 0x8C, 0xAE, 0xDB, 0x11, 0x4A, 0xBD, 0xF2, 0xC5, 0x7B, 0xEF, 0xE2, 0xD9, 0x92, 0xEF, 0xFC, 0xF0, 0x50, 0xFF, 0xFA, 0x0A, 0x85, 0x56, 0xD9, 0x64, 0x85, 0xE5, 0xAD, 0xBA, 0xC5, 0x6A, 0x85, 0x59, 0x57, 0xA8, 0x06, 0x6A, 0x16, 0x37, 0xED, 0xD1, 0x44, 0x44, 0x96, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };

            tex.LoadImage(pngBytes);
            return Sprite.Create(tex, new Rect(0f, 0f, 24f, 24f), new Vector2(0f, 0f));
        }

    }
}
