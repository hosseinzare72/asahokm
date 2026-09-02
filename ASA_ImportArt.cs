using UnityEngine;
using UnityEditor;
using System.IO;

public static class ASA_ImportArt
{
    const string SRC = @"F:\download\اسا\game_assets_exact";
    const string DST = "Assets/_Game/Art/NewUI";

    [MenuItem("ASA/Import Art Pack")]
    static void Import()
    {
        if (!Directory.Exists(SRC))
        {
            Debug.LogError("SRC not found: " + SRC);
            return;
        }

        int count = 0;
        foreach (string dir in Directory.GetDirectories(SRC))
        {
            string group = Path.GetFileName(dir);
            foreach (string file in Directory.GetFiles(dir, "*.png"))
            {
                string dstDir = DST + "/" + group;
                Directory.CreateDirectory(dstDir);
                File.Copy(file, dstDir + "/" + Path.GetFileName(file), true);
                count++;
            }
        }

        AssetDatabase.Refresh();

        foreach (string dir in Directory.GetDirectories(DST))
        {
            foreach (string file in Directory.GetFiles(dir, "*.png"))
            {
                string p = file.Replace('\\', '/');
                TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(p);
                if (ti == null) continue;
                ti.textureType = TextureImporterType.Sprite;
                ti.spriteImportMode = SpriteImportMode.Single;
                ti.mipmapEnabled = false;
                ti.filterMode = FilterMode.Bilinear;
                ti.textureCompression = TextureImporterCompression.Uncompressed;
                ti.maxTextureSize = 2048;
                ti.SaveAndReimport();
            }
        }

        Debug.Log("Imported " + count + " art files OK");
    }
}