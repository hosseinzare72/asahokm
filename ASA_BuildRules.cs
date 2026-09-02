using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class ASA_BuildRules
{
    [MenuItem("ASA/Build Rules Screen")]
    static void Build()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("ASA: aval Play ra khamoosh kon, bad in menu ra bezan.");
            return;
        }
        Debug.Log("ASA: dataPath=" + Application.dataPath);

        if (System.IO.Directory.Exists("Assets/_Game/Art/NewUI/backgrounds"))
        {
            foreach (string f in System.IO.Directory.GetFiles("Assets/_Game/Art/NewUI/backgrounds"))
                Debug.Log("ASA: bg file: " + System.IO.Path.GetFileName(f));
        }

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;
        Debug.Log("ASA: scene ok");

        GameObject canvas = new GameObject("Canvas");
        Canvas c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler sc = canvas.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1080, 2340);
        sc.matchWidthOrHeight = 0.5f;
        canvas.AddComponent<GraphicRaycaster>();

        GameObject es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        Debug.Log("ASA: canvas ok");

        GameObject bg = Img(canvas.transform, "BG", "Assets/_Game/Art/NewUI/backgrounds/background.png", "Assets/_Game/Art/UI/ASA_BG_Main.png");
        SetRect(bg, 0, 0, 1080, 2340);
        Debug.Log("ASA: bg ok, sprite=" + (bg.GetComponent<Image>().sprite != null));

        GameObject title = Txt(canvas.transform, "TxtTitle", "Vazirmatn-Bold", "قوانین بازی", 80, TextAlignmentOptions.Center, "#E9B94E");
        SetRect(title, 0, 980, 900, 160);

        GameObject panel = Img(canvas.transform, "PanelRules", "Assets/_Game/Art/UI/ASA_Panel.png", null);
        SetRect(panel, 0, 60, 960, 1620);
        Debug.Log("ASA: panel ok");

        GameObject scroll = new GameObject("ScrollView", typeof(RectTransform));
        scroll.transform.SetParent(panel.transform, false);
        SetRect(scroll, 0, 0, 880, 1540);
        ScrollRect sr = scroll.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;

        GameObject vp = new GameObject("Viewport", typeof(RectTransform));
        vp.transform.SetParent(scroll.transform, false);
        RectTransform vpr = vp.GetComponent<RectTransform>();
        vpr.anchorMin = Vector2.zero;
        vpr.anchorMax = Vector2.one;
        vpr.offsetMin = Vector2.zero;
        vpr.offsetMax = Vector2.zero;
        vp.AddComponent<Image>();
        Mask mk = vp.AddComponent<Mask>();
        mk.showMaskGraphic = false;
        sr.viewport = vpr;

        GameObject content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(vp.transform, false);
        RectTransform cr = content.GetComponent<RectTransform>();
        cr.anchorMin = new Vector2(0.5f, 1f);
        cr.anchorMax = new Vector2(0.5f, 1f);
        cr.pivot = new Vector2(0.5f, 1f);
        cr.anchoredPosition = Vector2.zero;
        cr.sizeDelta = new Vector2(800, 100);
        sr.content = cr;

        GameObject rules = Txt(content.transform, "TxtRules", "Vazirmatn-Regular", RULES, 44, TextAlignmentOptions.Right, "#F5EBD8");
        RectTransform rr = rules.GetComponent<RectTransform>();
        rr.anchorMin = new Vector2(0.5f, 1f);
        rr.anchorMax = new Vector2(0.5f, 1f);
        rr.pivot = new Vector2(0.5f, 1f);
        rr.anchoredPosition = Vector2.zero;
        rr.sizeDelta = new Vector2(800, 100);
        TextMeshProUGUI tmp = rules.GetComponent<TextMeshProUGUI>();
        tmp.enableWordWrapping = true;
        tmp.ForceMeshUpdate();
        Vector2 pref = tmp.GetPreferredValues(800f, Mathf.Infinity);
        rr.sizeDelta = new Vector2(800, pref.y + 50);
        cr.sizeDelta = new Vector2(800, pref.y + 50);
        Debug.Log("ASA: rules ok, height=" + pref.y);

        GameObject accept = Btn(canvas.transform, "BtnAccept", "Assets/_Game/Art/UI/ASA_Btn_Green.png", "قبول می‌کنم");
        SetRect(accept, 240, -1050, 430, 150);
        GameObject exit = Btn(canvas.transform, "BtnExit", "Assets/_Game/Art/UI/ASA_Btn_Red.png", "خروج از بازی");
        SetRect(exit, -240, -1050, 430, 150);
        Debug.Log("ASA: buttons ok");

        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<RulesScreen>();
        Debug.Log("ASA: canvas children=" + canvas.transform.childCount);

        System.IO.Directory.CreateDirectory("Assets/_Game/Scenes");
        bool saved = EditorSceneManager.SaveScene(scene, "Assets/_Game/Scenes/01_Rules.unity");
        if (saved)
            Debug.Log("Rules screen built OK, scene saved");
        else
            Debug.LogError("ASA: SAVE FAILED -> " + System.IO.Path.GetFullPath("Assets/_Game/Scenes/01_Rules.unity"));
    }

    static void SetRect(GameObject go, float x, float y, float w, float h)
    {
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = new Vector2(x, y);
        r.sizeDelta = new Vector2(w, h);
    }

    static Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(path);
        if (ti != null && ti.textureType != TextureImporterType.Sprite)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.mipmapEnabled = false;
            ti.filterMode = FilterMode.Bilinear;
            ti.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static GameObject Img(Transform parent, string name, string path, string fallback)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        Sprite s = LoadSprite(path);
        if (s == null && fallback != null) s = LoadSprite(fallback);
        img.sprite = s;
        return go;
    }

    static TMP_FontAsset Font(string key)
    {
        TMP_FontAsset f = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Game/Fonts/" + key + " SDF.asset");
        if (f == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            string k = key.ToLower().Replace(" ", "").Replace("-", "");
            foreach (string g in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(g);
                string n = System.IO.Path.GetFileNameWithoutExtension(p).ToLower().Replace(" ", "").Replace("-", "");
                if (n.Contains(k))
                {
                    f = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(p);
                    break;
                }
            }
        }
        if (f == null) f = TMP_Settings.defaultFontAsset;
        Debug.Log("ASA: font[" + key + "] => " + (f != null ? f.name : "NULL"));
        return f;
    }

    static GameObject Txt(Transform parent, string name, string fontKey, string text, int size, TextAlignmentOptions align, string hex)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.font = Font(fontKey);
        tmp.text = RTLTextMeshPro.Fix(text);
        tmp.fontSize = size;
        tmp.alignment = align;
        ColorUtility.TryParseHtmlString(hex, out Color col);
        tmp.color = col;
        return go;
    }

    static GameObject Btn(Transform parent, string name, string path, string label)
    {
        GameObject go = Img(parent, name, path, null);
        go.AddComponent<Button>();
        GameObject t = Txt(go.transform, "Txt" + name, "Vazirmatn-Bold", label, 48, TextAlignmentOptions.Center, "#FFFFFF");
        SetRect(t, 0, 0, 400, 120);
        return go;
    }

    const string RULES = "مطالعه و موافقت با قوانین زیر برای ورود به بازی الزامی است. در صورت نقض هر کدام از قوانین، حساب شما مسدود خواهد شد.\n\n* این بازی فاقد هرگونه شرط‌بندی و قمار است و صرفاً با هدف سرگرمی خانوادگی برای فارسی‌زبانان طراحی شده است. هرگونه سوءاستفاده از بازی ممنوع بوده و عواقب قانونی خواهد داشت.\n\n* استفاده از الفاظ و کلماتی که خلاف ادب، خلاف قوانین شرع مقدس اسلام یا خلاف قوانین کشور باشد، در هر جایی از بازی ممنوع است و با خاطیان برخورد خواهد شد.\n\n* از آنجا که این بازی صرفاً جهت سرگرمی طراحی شده، هیچ محتوایی با ارزش مالی وجود ندارد؛ بنابراین خرید و فروش اکانت ممنوع است.\n\n* هرگونه تقلب در بازی به هر نحوی از جمله استفاده از نرم‌افزارهای هک یا دستکاری در امتیازات ممنوع بوده و با خاطیان برخورد خواهد شد.\n\n* این بازی به هیچ عنوان به اطلاعات شخصی و مالی بازیکن دسترسی نخواهد داشت.";
}