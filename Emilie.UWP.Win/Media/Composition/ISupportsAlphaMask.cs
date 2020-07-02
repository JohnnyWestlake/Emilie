using Windows.UI.Composition;

namespace Emilie.UWP.Media
{
    public interface ISupportsAlphaMask
    {
        CompositionBrush GetAlphaMask();
    }
}
