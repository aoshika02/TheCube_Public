using UnityEngine;

namespace TileAnimUtil
{
    public static partial class TileMaterialAnimUtil
    {
        private static readonly int _frameTexId = Shader.PropertyToID(ParamConsts.FRAME_TEX);
        private static readonly int _iconTexId = Shader.PropertyToID(ParamConsts.ICON_TEX);
        private static readonly int _baseColorId = Shader.PropertyToID(ParamConsts.BASE_COLOR);
        private static readonly int _frameColorId = Shader.PropertyToID(ParamConsts.FRAME_COLOR);
        private static readonly int _iconColorId = Shader.PropertyToID(ParamConsts.ICON_COLOR);
        private static readonly int _iconAlphaId = Shader.PropertyToID(ParamConsts.ICON_ALPHA);
        private static readonly int _rotateId = Shader.PropertyToID(ParamConsts.ROTATE);
        private static readonly int _scaleId = Shader.PropertyToID(ParamConsts.SCALE);
        private static readonly int _stepRotateId = Shader.PropertyToID(ParamConsts.STEP_ROTATE);
        private static readonly int _borderId = Shader.PropertyToID(ParamConsts.BORDER);
    }
}
