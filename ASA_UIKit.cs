using UnityEngine;
using UnityEditor;
using System.IO;

public static class ASA_UIKit
{
    static readonly Color Lapis     = Hex(0x0B1F3A);
    static readonly Color LapisDark = Hex(0x061224);
    static readonly Color Turq      = Hex(0x17C3B2);
    static readonly Color Gold      = Hex(0xE9B94E);
    static readonly Color Green1    = Hex(0x2AA14A);
    static readonly Color Green2    = Hex(0x0B5A22);
    static readonly Color Red1      = Hex(0xC0392B);
    static readonly Color Red2      = Hex(0x6E120C);
    static readonly Color GoldBtn1  = Hex(0xE1A83C);
    static readonly Color GoldBtn2  = Hex(0x8A5A16);

    static Color Hex(int v) => new Color(((v >> 16) & 255) / 255f, ((v >> 8) & 255) / 255f, (v & 255) / 255f, 1f);
    static Color32 To32(Color c) => c;

    static float RBox(Vector2 p, float hx, float hy, float r)
    {
        float qx = Mathf.Abs(p.x) - hx;
        float qy = Mathf.Abs(p.y) - hy;
        float ox = Mathf.Max(qx, 0f);
        float oy = Mathf.Max(qy, 0f);
        return Mathf.Min(Mathf.Max(qx, qy), 0f) + Mathf.Sqrt(ox * ox + oy * oy) - r;
    }

    static float SqOutline(Vector2 p, float a, float t, float rotDeg)
    {
        float r = rotDeg * Mathf.Deg2Rad;
        float c = Mathf.Cos(r);
        float s = Mathf.Sin(r);
        Vector2 q = new Vector2(p.x * c + p.y * s, -p.x * s + p.y * c);
        float d = Mathf.Max(Mathf.Abs(q.x) - a, Mathf.Abs(q.y) - a);
        return Mathf.Abs(d) - t;
    }

    static float A(float d) => Mathf.Clamp01(0.5f - d);
    static Color Mix(Color b, Color o, float a) => Color.Lerp(b, o, Mathf.Clamp01(a));

    [MenuItem("ASA/Generate UI Kit")]
    static void Generate()
    {
        string dir = "Assets/_Game/Art/UI";
        Directory.CreateDirectory(dir);
        Save(MakeBackground(), dir + "/ASA_BG_Main.png");
        Save(MakeButton(Green1, Green2), dir + "/ASA_Btn_Green.png");
        Save(MakeButton(Red1, Red2), dir + "/ASA_Btn_Red.png");
        Save(MakeButton(GoldBtn1, GoldBtn2), dir + "/ASA_Btn_Gold.png");
        Save(MakePanel(), dir + "/ASA_Panel.png");
        AssetDatabase.Refresh();
        foreach (var f in Directory.GetFiles(dir, "*.png"))
        {
            string p = f.Replace('\\', '/');
            TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(p);
            ti.textureType = TextureImporterType.Sprite;
            ti.mipmapEnabled = false;
            ti.filterMode = FilterMode.Bilinear;
            ti.SaveAndReimport();
        }
        Debug.Log("ASA UI Kit generated ✔");
    }

    static Texture2D MakeBackground()
    {
        int w = 1080, h = 2340, tile = 160;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var px = new Color32[w * h];
        Vector2 center = new Vector2(w / 2f, h / 2f);
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            Color col = Color.Lerp(LapisDark, Lapis, Mathf.Clamp01(1f - Mathf.Abs(y - h * 0.45f) / (h * 0.75f)));
            float lx = ((x % tile) + tile) % tile - tile / 2f;
            float ly = ((y % tile) + tile) % tile - tile / 2f;
            Vector2 lp = new Vector2(lx, ly);
            float dPat = Mathf.Min(SqOutline(lp, 46f, 2f, 0f), SqOutline(lp, 46f, 2f, 45f));
            col = Mix(col, Turq, A(dPat) * 0.10f);
            float vig = Mathf.Clamp01(Vector2.Distance(new Vector2(x, y), center) / (h * 0.62f));
            col = Mix(col, Color.black, vig * vig * 0.45f);
            Vector2 pc = new Vector2(x - center.x, y - center.y);
            float f1 = Mathf.Abs(RBox(pc, w / 2f - 34f, h / 2f - 34f, 26f)) - 2.5f;
            float f2 = Mathf.Abs(RBox(pc, w / 2f - 56f, h / 2f - 56f, 18f)) - 1.2f;
            col = Mix(col, Gold, A(f1) * 0.85f);
            col = Mix(col, Turq, A(f2) * 0.35f);
            col.a = 1f;
            px[y * w + x] = To32(col);
        }
        tex.SetPixels32(px);
        tex.Apply();
        return tex;
    }

    static Texture2D MakeButton(Color top, Color bottom)
    {
        int w = 600, h = 180;
        float r = 44f, border = 7f;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var px = new Color32[w * h];
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            Vector2 p = new Vector2(x - w / 2f, y - h / 2f);
            float dOut = RBox(p, w / 2f - border, h / 2f - border, r);
            float dIn = RBox(p, w / 2f - border - 3f, h / 2f - border - 3f, r - 3f);
            float aOut = A(dOut);
            float aIn = A(dIn);
            Color grad = Color.Lerp(bottom, top, Mathf.Clamp01(y / (float)h + 0.15f));
            if (y > h - 34 && y < h - 14) grad = Mix(grad, Color.white, 0.18f);
            Color col = Mix(Gold, grad, aIn);
            col.a = aOut;
            px[y * w + x] = To32(col);
        }
        tex.SetPixels32(px);
        tex.Apply();
        return tex;
    }

    static Texture2D MakePanel()
    {
        int w = 900, h = 1200;
        float r = 36f, border = 5f;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var px = new Color32[w * h];
        Color fill = LapisDark; fill.a = 0.94f;
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            Vector2 p = new Vector2(x - w / 2f, y - h / 2f);
            float dOut = RBox(p, w / 2f - border, h / 2f - border, r);
            float dIn = RBox(p, w / 2f - border - 2f, h / 2f - border - 2f, r - 2f);
            float aOut = A(dOut);
            float aIn = A(dIn);
            float hair = Mathf.Abs(RBox(p, w / 2f - 26f, h / 2f - 26f, 22f)) - 1f;
            Color col = Mix(Gold, fill, aIn);
            col = Mix(col, Turq, A(hair) * 0.4f * aIn);
            col.a = Mathf.Max(aOut, aIn * fill.a);
            px[y * w + x] = To32(col);
        }
        tex.SetPixels32(px);
        tex.Apply();
        return tex;
    }

    static void Save(Texture2D tex, string path)
    {
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }
}