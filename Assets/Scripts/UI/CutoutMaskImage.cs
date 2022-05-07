using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI
{
    public class CutoutMaskImage : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material newMaterial = new Material(base.materialForRendering);
                newMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return newMaterial;
            }
        }
    }
}
