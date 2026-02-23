using UnityEngine;

namespace TileAnimUtil
{

    public static partial class TileMaterialAnimUtil
    {
        private static bool MaterialCheck(this Material material)
        {
            if (material == null)
            {
                Debug.LogWarning("Material is null.");
                return false;
            }
            return true;
        }

        public static void MaterialInit(
            this Material material,
            Texture frameTexture,
            Texture iconTexture,
            Color baseColor,
            Color frameColor,
            Color iconColor,
            bool isIconShow)
        {
            if (!material.MaterialCheck()) return;
            material.SetTexture(_frameTexId, frameTexture);
            material.SetTexture(_iconTexId, iconTexture);
            material.SetColor(_baseColorId, baseColor);
            material.SetColor(_frameColorId, frameColor);
            material.SetColor(_iconColorId, iconColor);
            material.SetFloat(_scaleId, 1f);
            material.SetFloat(_borderId, 1f);

            if (isIconShow)
            {
                material.SetFloat(_iconAlphaId, 1f);
            }
            else
            {
                material.SetFloat(_iconAlphaId, 0f);
            }
        }
    }
}
