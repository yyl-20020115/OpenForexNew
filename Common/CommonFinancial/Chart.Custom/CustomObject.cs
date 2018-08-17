using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CommonSupport;

namespace CommonFinancial
{
    [Serializable]
    public abstract class CustomObject
    {
        public enum DrawingOrderEnum
        {
            PreSeries,
            PostSeries
        }

        DrawingOrderEnum _drawingOrder = DrawingOrderEnum.PreSeries;
        public DrawingOrderEnum DrawingOrder
        {
            get { return _drawingOrder; }
            set { _drawingOrder = value; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public bool IsInitialized
        {
            get { return _manager != null; }
        }

        [NonSerialized]
        CustomObjectsManager _manager;
        protected CustomObjectsManager Manager
        {
            get { return _manager; }
        }

        /// <summary>
        /// 
        /// </summary>
        public CustomObject()
        {
        }

        public abstract void Draw(GraphicsWrapper g, PointF? mousePosition, RectangleF clippingRectangle, RectangleF drawingSpace);

        public virtual bool Initialize(CustomObjectsManager manager)
        {
            SystemMonitor.CheckThrow(_manager == null);
            _manager = manager;
            return true;
        }

        public void UnInitialize()
        {
            _manager = null;
        }

    }
}
