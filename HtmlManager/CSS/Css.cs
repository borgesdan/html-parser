﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlManager.CSS
{
    public class Css
    {
        public static bool KnowCssProperty(string propertyName)
        {
            string property = Regex.Replace(propertyName,@"/ ^-.+? -/", "");

            return Array.IndexOf(CSSProperties, property) != -1;
        }

        public static readonly string[] CSSProperties = new string[]
        {
            "alignment-adjust","alignment-baseline","animation","animation-delay",
            "animation-direction","animation-duration","animation-iteration-count",
            "animation-name","animation-play-state","animation-timing-function",
            "appearance","azimuth","backface-visibility","background",
            "background-attachment","background-clip","background-color",
            "background-image","background-origin","background-position",
            "background-repeat","background-size","baseline-shift","binding",
            "bleed","bookmark-label","bookmark-level","bookmark-state",
            "bookmark-target","border","border-bottom","border-bottom-color",
            "border-bottom-left-radius","border-bottom-right-radius",
            "border-bottom-style","border-bottom-width","border-collapse",
            "border-color","border-image","border-image-outset",
            "border-image-repeat","border-image-slice","border-image-source",
            "border-image-width","border-left","border-left-color",
            "border-left-style","border-left-width","border-radius","border-right",
            "border-right-color","border-right-style","border-right-width",
            "border-spacing","border-style","border-top","border-top-color",
            "border-top-left-radius","border-top-right-radius","border-top-style",
            "border-top-width","border-width","bottom","box-decoration-break",
            "box-shadow","box-sizing","break-after","break-before","break-inside",
            "caption-side","clear","clip","color","color-profile","column-count",
            "column-fill","column-gap","column-rule","column-rule-color",
            "column-rule-style","column-rule-width","column-span","column-width",
            "columns","content","counter-increment","counter-reset","crop","cue",
            "cue-after","cue-before","cursor","direction","display",
            "dominant-baseline","drop-initial-after-adjust",
            "drop-initial-after-align","drop-initial-before-adjust",
            "drop-initial-before-align","drop-initial-size","drop-initial-value",
            "elevation","empty-cells","filter","fit","fit-position","flex-align",
            "flex-flow","flex-line-pack","flex-order","flex-pack","float","float-offset",
            "font","font-family","font-size","font-size-adjust","font-stretch",
            "font-style","font-variant","font-weight","grid-columns","grid-rows",
            "hanging-punctuation","height","hyphenate-after","hyphenate-before",
            "hyphenate-character","hyphenate-lines","hyphenate-resource","hyphens",
            "icon","image-orientation","image-rendering","image-resolution",
            "inline-box-align","left","letter-spacing","line-break","line-height",
            "line-stacking","line-stacking-ruby","line-stacking-shift",
            "line-stacking-strategy","list-style","list-style-image",
            "list-style-position","list-style-type","margin","margin-bottom",
            "margin-left","margin-right","margin-top","marker-offset","marks",
            "marquee-direction","marquee-loop","marquee-play-count","marquee-speed",
            "marquee-style","max-height","max-width","min-height","min-width",
            "move-to","nav-down","nav-index","nav-left","nav-right","nav-up",
            "opacity","orphans","outline","outline-color","outline-offset",
            "outline-style","outline-width","overflow","overflow-style",
            "overflow-wrap","overflow-x","overflow-y","padding","padding-bottom",
            "padding-left","padding-right","padding-top","page","page-break-after",
            "page-break-before","page-break-inside","page-policy","pause",
            "pause-after","pause-before","perspective","perspective-origin",
            "phonemes","pitch","pitch-range","play-during","pointer-events",
            "position",
            "presentation-level","punctuation-trim","quotes","rendering-intent",
            "resize","rest","rest-after","rest-before","richness","right",
            "rotation","rotation-point","ruby-align","ruby-overhang",
            "ruby-position","ruby-span","src","size","speak","speak-header",
            "speak-numeral","speak-punctuation","speech-rate","stress","string-set",
            "tab-size","table-layout","target","target-name","target-new",
            "target-position","text-align","text-align-last","text-decoration",
            "text-decoration-color","text-decoration-line","text-decoration-skip",
            "text-decoration-style","text-emphasis","text-emphasis-color",
            "text-emphasis-position","text-emphasis-style","text-height",
            "text-indent","text-justify","text-outline","text-shadow",
            "text-space-collapse","text-transform","text-underline-position",
            "text-wrap","top","transform","transform-origin","transform-style",
            "transition","transition-delay","transition-duration",
            "transition-property","transition-timing-function","unicode-bidi",
            "vertical-align","visibility","voice-balance","voice-duration",
            "voice-family","voice-pitch","voice-pitch-range","voice-rate",
            "voice-stress","voice-volume","volume","white-space","widows","width",
            "word-break","word-spacing","word-wrap","z-index",
            // flexbox:
            "align-content", "align-items", "align-self", "flex", "flex-basis",
            "flex-direction", "flex-flow", "flex-grow", "flex-shrink", "flex-wrap",
            "justify-content"
        };
    }
}
