using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using CommonSupport;
using System.Windows.Forms;

namespace CommonFinancial
{
    /// <summary>
    /// Base class for dynamic custom chart objects. Dynamic objects are considered objects
    /// that can change state dynamically, whether code wise or trough user interaction.
    /// </summary>
    [Serializable]
    public abstract class DynamicCustomObject : CustomObject, IDynamicPropertyContainer
    {
        protected static readonly Font DefaultDynamicObjectFont = new Font("Tahoma", 10);
        
        public bool Enabled
        {
            get
            {
                return this.Visible;
            }
            set
            {
                this.Visible = value;
            }
        }


        Pen _defaultControlPointPen = Pens.LightGray;
        public Pen DefaultControlPointPen
        {
            get { return _defaultControlPointPen; }
            set { _defaultControlPointPen = value; }
        }

        Pen _defaultSelectedControlPointPen = Pens.Red;
        public Pen DefaultSelectedControlPointPen
        {
            get { return _defaultSelectedControlPointPen; }
            set { _defaultSelectedControlPointPen = value; }
        }

        int _defaultAbsoluteControlPointSelectionRectanglesSize = 5;
        public int DefaultAbsoluteControlPointSelectionRectanglesSize
        {
            get { return _defaultAbsoluteControlPointSelectionRectanglesSize; }
            set { _defaultAbsoluteControlPointSelectionRectanglesSize = value; }
        }

        protected List<int> _selectedControlPoints = new List<int>();
        public ICollection<int> SelectedControlPoints
        {
            get { return _selectedControlPoints; }
        }

        protected List<PointF> _controlPoints = new List<PointF>();
        public ICollection<PointF> ControlPoints
        {
            get { return _controlPoints; }
        }

        public abstract bool IsBuilding
        {
            get;
        }

        bool _selected = false;
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public DynamicCustomObject(string name)
        {
            base.Name = name;
        }

        void RaiseUpdatedEvent()
        {
            Manager.HandleDynamicObjectUpdated(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformationMatrix"></param>
        /// <param name="point"></param>
        /// <param name="absoluteSelectionMargin"></param>
        /// <param name="canLoseSelection">If the control is selected and current point unselects it, can it set state to unselected.</param>
        /// <returns>Return true to mark - selected.</returns>
        public virtual bool TrySelect(Matrix transformationMatrix, PointF drawingSpaceSelectionPoint, float absoluteSelectionMargin, bool canLoseSelection)
        {
            if (IsBuilding)
            {
                return false;
            }

            if (TrySelectControlPoints(transformationMatrix, drawingSpaceSelectionPoint, absoluteSelectionMargin) > 0)
            {
                Selected = true;
                return true;
            }

            if (canLoseSelection)
            {
                Selected = false;
            }

            return false;
        }

        /// <summary>
        /// Return true, when building is done.
        /// </summary>
        /// <param name="point"></param>
        public abstract bool AddBuildingPoint(PointF point);

        /// <summary>
        /// Return true, when building is done.
        /// Used by those objects that need key input.
        /// </summary>
        /// <param name="point"></param>
        public virtual bool AddBuildingKey(KeyPressEventArgs key)
        {
            return false;
        }
        
        /// <summary>
        /// Return true, when building is done.
        /// Used by those objects that need key input.
        /// </summary>
        public virtual bool AddBuildingKey(KeyEventArgs key)
        {
            return false;
        }

        /// <summary>
        /// Return false to specify the entire object need to be removed, true - the point was deleted.
        /// </summary>
        public virtual bool DeleteControlPoint(int pointIndex)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mouseLocation"></param>
        /// <returns>Returns true to indicate hover is active and a redraw is needed.</returns>
        public virtual bool SetMouseHover(PointF mouseLocation)
        {
            return false;
        }


        /// <summary>
        /// Helper - extends the given line segment to the ends of the drawing space.
        /// </summary>
        protected void StretchSegmentToDrawingSpace(ref PointF point1, ref PointF point2, RectangleF drawingSpace)
        {
            //if (_controlPoints.Count < 2)
            //{
            //    return;
            //}

            SimpleLine thisLine = new SimpleLine(point1, point2);
            PointF?[] intersectionPoints = new PointF?[4];
            // The order is important, so preserve it.
            intersectionPoints[0] = thisLine.Intersection(new SimpleLine(new PointF(drawingSpace.X, drawingSpace.Y), new PointF(drawingSpace.X, drawingSpace.Y + drawingSpace.Width)));
            intersectionPoints[1] = thisLine.Intersection(new SimpleLine(new PointF(drawingSpace.X + drawingSpace.Width, drawingSpace.Y), new PointF(drawingSpace.X + drawingSpace.Width, drawingSpace.Y + drawingSpace.Width)));
            intersectionPoints[2] = thisLine.Intersection(new SimpleLine(new PointF(drawingSpace.X, drawingSpace.Y), new PointF(drawingSpace.X + drawingSpace.Height, drawingSpace.Y)));
            intersectionPoints[3] = thisLine.Intersection(new SimpleLine(new PointF(drawingSpace.X, drawingSpace.Y + drawingSpace.Height), new PointF(drawingSpace.X + drawingSpace.Height, drawingSpace.Y + drawingSpace.Height)));

            if (intersectionPoints[0].HasValue == false)
            {// Parallel to the 0,0 / 0, 10 line.
                point1 = new PointF(point1.X, drawingSpace.Y);
                point2 = new PointF(point1.X, drawingSpace.Y + drawingSpace.Height);
                return;
            }
            else if (intersectionPoints[1].HasValue == false)
            {// Parallel to the 0,0 / 10, 0 line.
                point1 = new PointF(drawingSpace.X, point1.Y);
                point2 = new PointF(drawingSpace.X + drawingSpace.Width, point1.Y);
                return;
            }

            // Establish the best 2 points (shortest line = best performance).
            int pointIndex1 = -1, pointIndex2 = -1;
            for (int i = 0; i < intersectionPoints.Length; i++)
            {
                if (intersectionPoints[i].HasValue == false)
                {
                    continue;
                }

                // Is the point within drawing space (Contains baseMethod does not deliver needed results).
                float xCalculationErrorMargin = (drawingSpace.X + drawingSpace.Width) / 100;
                float yCalculationErrorMargin = (drawingSpace.Y + drawingSpace.Height) / 100;

                if (intersectionPoints[i].Value.X < drawingSpace.X - xCalculationErrorMargin
                    || intersectionPoints[i].Value.X > drawingSpace.X + drawingSpace.Width + xCalculationErrorMargin
                    || intersectionPoints[i].Value.Y < drawingSpace.Y - yCalculationErrorMargin
                    || intersectionPoints[i].Value.Y > drawingSpace.Y + drawingSpace.Height + yCalculationErrorMargin)
                {// Point outside.
                    continue;
                }

                // Point approved.    
                if (pointIndex1 < 0)
                {
                    pointIndex1 = i;
                }
                else
                {
                    pointIndex2 = i;
                    break;
                }
            }

            if (pointIndex1 < 0 || pointIndex2 < 0)
            {
                //SystemMonitor.Error("This scenario should not happen.");
                pointIndex1 = 0;
                pointIndex2 = 1;
            }

            point1 = intersectionPoints[pointIndex1].Value;
            point2 = intersectionPoints[pointIndex2].Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Drag(PointF drawingSpaceMoveVector)
        {
            if (IsBuilding)
            {
                return;
            }

            if (_selectedControlPoints.Count > 0)
            {// Drag selected point only.
                foreach (int pointIndex in _selectedControlPoints)
                {
                    _controlPoints[pointIndex] = new PointF(_controlPoints[pointIndex].X + drawingSpaceMoveVector.X, _controlPoints[pointIndex].Y + drawingSpaceMoveVector.Y);
                }
            }
            else
            {// Drag entire object.
                for (int i = 0; i < _controlPoints.Count; i++)
                {
                    _controlPoints[i] = new PointF(_controlPoints[i].X + drawingSpaceMoveVector.X, _controlPoints[i].Y + drawingSpaceMoveVector.Y);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingSpace">This parameter is used in "total space" objects like an endless line.</param>
        /// <returns></returns>
        public virtual RectangleF GetContainingRectangle(RectangleF drawingSpace)
        {
            if (ControlPoints.Count == 0)
            {
                return new RectangleF();
            }

            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            foreach (PointF point in ControlPoints)
            {
                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);

                minY = Math.Min(minY, point.Y);
                maxY = Math.Max(maxY, point.Y);
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }


        /// <summary>
        /// Helper.
        /// </summary>
        protected int TrySelectControlPoints(Matrix transformationMatrix, PointF point, float absoluteSelectionMargin)
        {
            _selectedControlPoints.Clear();

            for (int i = 0; i < _controlPoints.Count; i++)
            {
                if (MathHelper.GetAbsoluteDistance(transformationMatrix, _controlPoints[i], point) <= absoluteSelectionMargin)
                {
                    _selectedControlPoints.Add(i);
                }
            }

            return _selectedControlPoints.Count;
        }

        /// <summary>
        /// Helper.
        /// </summary>
        protected bool TrySelectLine(Matrix transformationMatrix, bool allowLineSegmentOnly, PointF linePoint1, PointF linePoint2, PointF location, float absoluteSelectionMargin)
        {
            float lineSegmentLocation;
            PointF intersectionPoint;
            
            SimpleLine line = new SimpleLine(linePoint1, linePoint2);
            float? distance = line.DistanceToPoint(location, out lineSegmentLocation, out intersectionPoint);
                        
            if (MathHelper.GetAbsoluteDistance(transformationMatrix, location, intersectionPoint) <= absoluteSelectionMargin)
            {// Selection.
                return allowLineSegmentOnly == false || (lineSegmentLocation >=0 && lineSegmentLocation <= 1);
            }

            return false;
        }

        /// <summary>
        /// Control points selection rectangle is rendered in absolute size, ignoring scaling.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen">Pass null to use default control point pen.</param>
        protected void DrawControlPoints(GraphicsWrapper g)
        {
            float xMargin = 0.5f * Math.Abs(_defaultAbsoluteControlPointSelectionRectanglesSize / g.DrawingSpaceTransformClone.Elements[0]);
            float yMargin = 0.5f * Math.Abs(_defaultAbsoluteControlPointSelectionRectanglesSize / g.DrawingSpaceTransformClone.Elements[3]);

            for (int i = 0; i < _controlPoints.Count; i++)
            {
                PointF point = _controlPoints[i];
                if (_selectedControlPoints.Contains(i))
                {
                    g.DrawRectangle(_defaultSelectedControlPointPen, point.X - xMargin, point.Y - yMargin, xMargin * 2, yMargin * 2);
                }
                else
                {
                    g.DrawRectangle(_defaultControlPointPen, point.X - xMargin, point.Y - yMargin, xMargin * 2, yMargin * 2);
                }
            }
        }



        public string[] GetGenericDynamicPropertiesNames()
        {
            return new string[] { };
        }

        public object GetGenericDynamicPropertyValue(string name)
        {
            return null;
        }

        public Type GetGenericDynamicPropertyType(string name)
        {
            return null;
        }

        public bool SetGenericDynamicPropertyValue(string name, object value)
        {
            return false;
        }

        public void PropertyChanged()
        {
            RaiseUpdatedEvent();
        }

        
    }
}
