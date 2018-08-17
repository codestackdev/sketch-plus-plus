using CodeStack.VPages.Sw.Attributes;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStack.Tools.Sw.SketchPlusPlus.Commands
{
    [PropertyManagerPageOptions(swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton
        | swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
        | swPropertyManagerPageOptions_e.swPropertyManagerOptions_WhatsNew)]
    [PropertyManagerPageHelp("https://www.codestack.net/labs/solidworks/sketch-plus-plus/", "https://www.codestack.net/labs/solidworks/sketch-plus-plus/whats-new/")]
    [DisplayName("Offset Sketch")]
    public class OffsetWithFilletData
    {
        [PropertyManagerPageSelectionBox(0, swSelectType_e.swSelEXTSKETCHSEGS, swSelectType_e.swSelSKETCHSEGS)]
        [PropertyManagerPageSelectionBoxStyle(height: 50)]
        [PropertyManagerPageControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectEdge)]
        public List<ISketchSegment> Segments { get; set; }

        [DisplayName("Offset Distance")]
        [Description("Offset Distance")]
        [PropertyManagerPageControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Depth)]
        [PropertyManagerPageNumberBoxStyle(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.0005, true, 0.001, 0.00025)]
        public double Offset { get; set; } = 0.001;

        [DisplayName("Inner Tips Radius")]
        [Description("Inner Tips Radius")]
        [PropertyManagerPageControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Radius)]
        [PropertyManagerPageNumberBoxStyle(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.0005, true, 0.001, 0.00025)]
        public double InnerTipsRadius { get; set; } = 0.00075;

        [DisplayName("Outer Tips Radius")]
        [Description("Outer Tips Radius")]
        [PropertyManagerPageControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Radius)]
        [PropertyManagerPageNumberBoxStyle(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.0005, true, 0.001, 0.00025)]
        public double OuterTipsRadius { get; set; } = 0.0015;

        [DisplayName("Reverse")]
        [Description("Indicates if the offset should be reversed")]
        public bool Reverse { get; set; }
    }
}
