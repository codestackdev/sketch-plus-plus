using CodeStack.Tools.Sw.SketchPlusPlus.Commands.Base;
using CodeStack.Tools.Sw.SketchPlusPlus.Exceptions;
using CodeStack.VPages.Sw;
using CodeStack.VPages.Sw.Controls;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeStack.Tools.Sw.SketchPlusPlus.Commands
{
    public class OffsetWithFillet : IOffsetWithFillet
    {
        private ISldWorks m_App;
        private OffsetWithFilletData m_Data;

        private PropertyManagerPageBuilder<OffsetWithFilletPageEventHandler> m_PmpBuilder;
        private PropertyManagerPage<OffsetWithFilletPageEventHandler> m_ActivePage;
        private OffsetBuilder m_OffsetBuilder;

        public OffsetWithFillet(ISldWorks app)
        {
            m_App = app;
            m_PmpBuilder = new PropertyManagerPageBuilder<OffsetWithFilletPageEventHandler>(m_App);
            m_Data = new OffsetWithFilletData();
        }

        public bool CanExecute()
        {
            if (m_App.IActiveDoc2 != null)
            {
                return m_App.IActiveDoc2.SketchManager.ActiveSketch != null;
            }

            return false;
        }

        public void Execute()
        {
            m_App.IActiveDoc2.ISelectionManager.GetSelectedObject6(1, -1);

            m_Data.Segments = new List<ISketchSegment>();

            var selMgr = m_App.IActiveDoc2.ISelectionManager;

            if (selMgr != null)
            {
                for (int i = 1; i < selMgr.GetSelectedObjectCount2(-1); i++)
                {
                    if (selMgr.GetSelectedObjectType3(i, -1) == (int)swSelectType_e.swSelSKETCHSEGS)
                    {
                        m_Data.Segments.Add(selMgr.GetSelectedObject6(i, -1) as ISketchSegment);
                    }
                }
            }

            m_ActivePage = m_PmpBuilder.CreatePage(m_Data);
            m_OffsetBuilder = new OffsetBuilder(m_App, m_App.IActiveDoc2);
            m_ActivePage.Handler.DataChanged += OnDataChanged;
            m_ActivePage.Handler.Closing += OnClosing;
            m_ActivePage.Handler.Closed += OnClosed;
            m_ActivePage.Page.Show2(0);
            m_OffsetBuilder.PreviewOffset(m_Data.Segments, m_Data.Offset,
                    m_Data.InnerTipsRadius, m_Data.OuterTipsRadius, m_Data.Reverse);
        }

        private void OnClosing(swPropertyManagerPageCloseReasons_e reason, ClosingArg arg)
        {
            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                try
                {
                    m_OffsetBuilder.CreateOffset(m_Data.Segments, m_Data.Offset,
                        m_Data.InnerTipsRadius, m_Data.OuterTipsRadius, m_Data.Reverse);
                }
                catch (UserException ex)
                {
                    arg.Cancel = true;
                    arg.ErrorMessage = ex.Message;
                    arg.ErrorTitle = "Offset Failed";
                }
                catch (Exception ex)
                {
                    arg.Cancel = true;
                    arg.ErrorMessage = "Failed to generate the offset";
                    arg.ErrorTitle = "Offset Failed";
                }
            }
        }

        private void OnClosed(swPropertyManagerPageCloseReasons_e reason)
        {
            m_OffsetBuilder.HidePreview();

            if (reason == swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay)
            {
                var offsetBody = m_OffsetBuilder.CreateOffset(m_Data.Segments, m_Data.Offset,
                        m_Data.InnerTipsRadius, m_Data.OuterTipsRadius, m_Data.Reverse);
                
                m_OffsetBuilder.ConvertToSketchSegments(m_App.IActiveDoc2, offsetBody);
            }

            m_OffsetBuilder.Dispose();
        }

        private void OnDataChanged()
        {
            try
            {
                m_OffsetBuilder.PreviewOffset(m_Data.Segments, m_Data.Offset,
                    m_Data.InnerTipsRadius, m_Data.OuterTipsRadius, m_Data.Reverse);
            }
            catch
            {
                //TODO: show error
            }
        }
    }
}
