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
    public class OffsetWithFilletData
    {
        [PropertyManagerPageSelectionBox(0, swSelectType_e.swSelSKETCHSEGS)]
        [PropertyManagerPageSelectionBoxStyle(height: 50)]
        [PropertyManagerPageControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectEdge)]
        public List<ISketchSegment> Segments { get; set; }

        [DisplayName("Offset Distance")]
        [Description("Offset Distance")]
        [PropertyManagerPageControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Depth)]
        [PropertyManagerPageNumberBoxStyle(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.001, true, 0.01, 0.0005)]
        public double Offset { get; set; } = 0.001;

        [DisplayName("Inner Tips Radius")]
        [Description("Inner Tips Radius")]
        [PropertyManagerPageControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Radius)]
        [PropertyManagerPageNumberBoxStyle(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.001, true, 0.01, 0.0005)]
        public double InnerTipsRadius { get; set; } = 0.00075;

        [DisplayName("Outer Tips Radius")]
        [Description("Outer Tips Radius")]
        [PropertyManagerPageControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Radius)]
        [PropertyManagerPageNumberBoxStyle(swNumberboxUnitType_e.swNumberBox_Length, 0, 1000, 0.001, true, 0.01, 0.0005)]
        public double OuterTipsRadius { get; set; } = 0.0015;
    }
}
