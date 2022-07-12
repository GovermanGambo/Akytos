#if AKYTOS_EDITOR

using System.Numerics;
using Akytos.Assets;
using Akytos.Editor.Renderers;
using LightInject;
using Veldrid;

namespace Akytos.Editor;

internal class EditorCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<IGuiControlRenderer<int>, IntControlRenderer>();
        serviceRegistry.Register<IGuiControlRenderer<string>, TextControlRenderer>();
        serviceRegistry.Register<IGuiControlRenderer<float>, FloatControlRenderer>();
        serviceRegistry.Register<IGuiControlRenderer<Vector2>, Vector2ControlRenderer>();
        serviceRegistry.Register<IGuiControlRenderer<Asset<Texture>?>, TextureAssetRenderer>();
        serviceRegistry.Register<IGuiControlRenderer<bool>, BoolControlRenderer>();
        serviceRegistry.Register<IGuiControlRenderer<Color>, ColorControlRenderer>();
    }
}

#endif