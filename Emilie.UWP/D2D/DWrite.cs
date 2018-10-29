using Emilie.Core.Extensions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Emilie.UWP.D2D
{

    public static class DWrite
    {

        /// <summary>
        /// Attempts to return a list of strings representing the text on each rendered line of a textblock
        /// </summary>
        /// <param name="textblock"></param>
        /// <returns></returns>
        public static List<LineInformation> GetLineInformation(TextBlock textblock, bool limitToRenderSize = false)
        {
            if (textblock.DesiredSize.IsEmpty || textblock.DesiredSize.Width == 0)
                throw new InvalidOperationException("TextBlock has no desired width. Cannot computed rendered lines");

            CanvasDevice device = CanvasDevice.GetSharedDevice(false);
            using (CanvasTextLayout layout = textblock.GetCanvasTextLayout(device, limitToRenderSize))
            {
                return GetLineInformation(layout, textblock.Text);
            }
        }

        public static List<LineInformation> GetLineInformation(CanvasTextLayout layout, string layoutText)
        {
            if (String.IsNullOrEmpty(layoutText))
                throw new ArgumentNullException(nameof(layoutText), "layoutText must contain valid data to calculate lines");

            // 1. Attempt to group characters by line
            var groupedChars = layout.GetCharacterRegions(0, layoutText.Length).GroupBy(region => region.LayoutBounds.Bottom).OrderBy(group => group.Key).ToList();

            // 2. Get each line of characters as a string
            StringBuilder sb = new StringBuilder();
            return groupedChars.Select((group) =>
            {
                sb.Clear();
                Rect r = group.First().LayoutBounds;
                group.DoImmediate(reg =>
                {
                    sb.Append(layoutText.Substring(reg.CharacterIndex, reg.CharacterCount));
                    r.Union(reg.LayoutBounds);
                });
                return new LineInformation { Text = sb.ToString(), Bounds = r };
            }).ToList();
        }

    



        //------------------------------------------------------
        //
        //  TextBlock to CanvasText* Converters
        //
        //------------------------------------------------------

        #region TextBlock to CanvasText* Converters

        /// <summary>
        /// Generates a CanvasTextFormat that closely represents the values set on a TextBlock
        /// Please read the source code to observe limitations of this method
        /// </summary>
        /// <param name="textblock"></param>
        /// <returns></returns>
        public static CanvasTextFormat GetCanvasTextFormat(this TextBlock textblock)
        {
            if (textblock.TextLineBounds != TextLineBounds.Full)
                throw new InvalidOperationException("Cannot create CanvasTextFormat when TextLineBounds is not set to TextLineBounds.Full");

            if (textblock.LineStackingStrategy == LineStackingStrategy.BaselineToBaseline)
                throw new InvalidOperationException("Cannot create CanvasTextFormat when LineStackingStrategy is LineStackingStrategy.BaselineToBaseline");

            return new CanvasTextFormat()
            {
                FontFamily          = textblock.FontFamily.Source,
                FontSize            = (Single)textblock.FontSize,
                FontStretch         = textblock.FontStretch,
                Direction           = textblock.FlowDirection.AsCanvasTextDirection(),
                FontStyle           = textblock.FontStyle,
                FontWeight          = textblock.FontWeight,
                LineSpacing         = textblock.LineStackingStrategy == LineStackingStrategy.MaxHeight ? -1 : (Single)textblock.LineHeight,
                WordWrapping        = textblock.TextWrapping.AsCanvasWordWrapping(),
                Options             = textblock.GetCanvasDrawTextOptions(),
                TrimmingGranularity = textblock.TextTrimming.AsCanvasTextTrimmingGranularity(),
                HorizontalAlignment = textblock.TextAlignment.AsCanvasHorizontalAlignment(),
            };
        }

        public static CanvasTextLayout GetCanvasTextLayout(this TextBlock textBlock, ICanvasResourceCreator resourceCreator, bool limitToRenderSize = false)
        {
            using (CanvasTextFormat format = textBlock.GetCanvasTextFormat())
            {
                // Create base layout from format
                CanvasTextLayout layout = new CanvasTextLayout(
                    resourceCreator, 
                    textBlock.Text, 
                    format, 
                    (float)textBlock.RenderSize.Width, 
                    limitToRenderSize ? (float)textBlock.RenderSize.Height : Single.MaxValue
                    );

                // Apply character spacing
                layout.SetCharacterSpacing(0, textBlock.Text.Length, 0, (float)textBlock.CharacterSpacing / (202f / 3f), 0);

                // Apply Optical Alignment
                layout.OpticalAlignment = textBlock.OpticalMarginAlignment.AsCanvasOpticalAlignment();

                return layout;
            }
        }

        #endregion





        //------------------------------------------------------
        //
        //  Xaml to DirectWrite Property Converters
        //
        //------------------------------------------------------

        #region Windows.UI.Xaml to Microsoft.Graphics.Canvas Converters

        public static CanvasOpticalAlignment AsCanvasOpticalAlignment(this OpticalMarginAlignment alignment)
        {
            if (alignment == OpticalMarginAlignment.None)
                return CanvasOpticalAlignment.Default;
            else
                return CanvasOpticalAlignment.NoSideBearings;
        }

        public static CanvasHorizontalAlignment AsCanvasHorizontalAlignment(this TextAlignment alignment)
        {
            if (alignment == TextAlignment.DetectFromContent)
                throw new InvalidOperationException("Cannot convert TextAlignment to CanvasHorizontalAlignment when TextAlignment is TextAlignment.DetectFromContent");

            switch (alignment)
            {
                case TextAlignment.Center:
                    return CanvasHorizontalAlignment.Center;
                case TextAlignment.Justify:
                    return CanvasHorizontalAlignment.Justified;
                case TextAlignment.Right:
                    return CanvasHorizontalAlignment.Right;
                default:
                    return CanvasHorizontalAlignment.Left;
            }
        }

        public static CanvasDrawTextOptions GetCanvasDrawTextOptions(this TextBlock textblock)
        {
            CanvasDrawTextOptions options = CanvasDrawTextOptions.Default;

            if (!textblock.UseLayoutRounding)
                options = CanvasDrawTextOptions.NoPixelSnap;

            if (textblock.IsColorFontEnabled)
                options = options & CanvasDrawTextOptions.EnableColorFont;

            if (textblock.TextTrimming == TextTrimming.Clip)
                options = options & CanvasDrawTextOptions.Clip;

            return options;
        }

        public static CanvasTextDirection AsCanvasTextDirection(this FlowDirection direction)
        {
            if (direction == FlowDirection.LeftToRight)
                return CanvasTextDirection.LeftToRightThenTopToBottom;
            else
                return CanvasTextDirection.RightToLeftThenTopToBottom;
        }

        public static CanvasWordWrapping AsCanvasWordWrapping(this TextWrapping wrapping)
        {
            switch (wrapping)
            {
                case TextWrapping.NoWrap:
                    return CanvasWordWrapping.NoWrap;
                case TextWrapping.Wrap:
                    return CanvasWordWrapping.Wrap;
                case TextWrapping.WrapWholeWords:
                    return CanvasWordWrapping.WholeWord;
                default:
                    return CanvasWordWrapping.NoWrap;
            }
        }

        public static CanvasTextTrimmingGranularity AsCanvasTextTrimmingGranularity(this TextTrimming trimming)
        {
            switch (trimming)
            {
                case TextTrimming.CharacterEllipsis:
                    return CanvasTextTrimmingGranularity.Character;
                case TextTrimming.WordEllipsis:
                    return CanvasTextTrimmingGranularity.Word;
                default:
                    return CanvasTextTrimmingGranularity.None;
            }
        }

        #endregion
    }
}
