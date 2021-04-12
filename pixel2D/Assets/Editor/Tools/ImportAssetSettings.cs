using System;
using Framework;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public class ImportAssetSettings : AssetPostprocessor
    {
        private const string ISIMPORTED = "isImported";
        private bool IsImported()
        {
            if (Application.isBatchMode) return true;
            return !assetPath.StartsWith(Constants.ArtDirPath) || assetImporter.userData.EndsWith(ISIMPORTED);
        }
        private void OnPostprocessTexture(Texture2D texture)
        {
            assetImporter.userData = ISIMPORTED;
        }

        private void OnPreprocessTexture()
        {
            if(IsImported()) return;
            TextureImporter textureImporter = (TextureImporter) assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = false;
        }
    }
}