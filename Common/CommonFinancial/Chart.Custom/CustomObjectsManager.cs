using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CommonSupport;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Runtime.Serialization;

namespace CommonFinancial
{
    /// <summary>
    /// Handles creation of custom objects on the ChartPane.
    /// </summary>
    public class CustomObjectsManager : IDisposable
    {
        ListUnique<DynamicCustomObject> _dynamicCustomObjects = new ListUnique<DynamicCustomObject>();
        public ICollection<DynamicCustomObject> DynamicCustomObjects
        {
            get { return _dynamicCustomObjects; }
        }

        ListUnique<CustomObject> _staticCustomObjects = new ListUnique<CustomObject>();
        public ICollection<CustomObject> StaticCustomObjects
        {
            get { return _staticCustomObjects; }
        }

        ListUnique<DynamicCustomObject> _selectedDynamicCustomObjects = new ListUnique<DynamicCustomObject>();
        public ICollection<DynamicCustomObject> SelectedDynamicCustomObjects
        {
            get { return _selectedDynamicCustomObjects; }
        }

        [NonSerialized]
        ChartPane _pane = null;

        [NonSerialized]
        DynamicCustomObject _currentObjectBuilt = null;

        [NonSerialized]
        PointF? _dragLastDrawingSpaceMouseLocation = null;

        public bool IsBuildingObject
        {
            get { return _currentObjectBuilt != null; }
        }

        public delegate void DynamicObjectBuiltDelegate(CustomObjectsManager manager, DynamicCustomObject dynamicObject);
        /// <summary>
        /// If the object is null, this means creation was canceled.
        /// </summary>
        [field:NonSerialized]
        public event DynamicObjectBuiltDelegate DynamicObjectBuiltEvent;

        /// <summary>
        /// 
        /// </summary>
        public CustomObjectsManager()
        {
        }

        public void Initialize(ChartPane pane)
        {
            SystemMonitor.CheckThrow(_pane == null);
            _pane = pane;
        }

        public void UnInitialize()
        {
            _pane = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderInfo"></param>
        public void SaveState(SerializationInfoEx info)
        {
            info.AddValue("CustomObjectManager::_dynamicCustomObjects", _dynamicCustomObjects);
            info.AddValue("CustomObjectManager::_staticCustomObjects", _staticCustomObjects);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderInfo"></param>
        public void RestoreState(SerializationInfoEx info)
        {
            ListUnique<DynamicCustomObject> dynamicObjects = info.GetValue<ListUnique<DynamicCustomObject>>("CustomObjectManager::_dynamicCustomObjects");
            ListUnique<CustomObject> staticObjects = info.GetValue<ListUnique<CustomObject>>("CustomObjectManager::_staticCustomObjects");

            foreach (DynamicCustomObject dynamicObject in dynamicObjects)
            {
                this.Add(dynamicObject);
            }

            foreach (CustomObject customObject in staticObjects)
            {
                this.Add(customObject);
            }
        }

        ///// <summary>
        ///// Take over *persistent only* information from another instance.
        ///// </summary>
        ///// <param name="manager"></param>
        //public void ScapeObjectsFrom(CustomObjectsManager manager)
        //{
        //    _selectedDynamicCustomObjects.Clear();

        //    foreach(CustomObject customObject in _staticCustomObjects.ToArray())
        //    {
        //        this.Remove(customObject);
        //    }
            
        //    foreach(CustomObject customObject in _dynamicCustomObjects.ToArray())
        //    {
        //        this.Remove(customObject);
        //    }

        //    foreach (CustomObject customObject in manager._staticCustomObjects)
        //    {
        //        manager.Remove(customObject);
        //        this.Add(customObject);
        //    }

        //    foreach (CustomObject customObject in manager._dynamicCustomObjects)
        //    {
        //        manager.Remove(customObject);
        //        this.Add(customObject);
        //    }
        //}

        #region IDisposable Members

        public void Dispose()
        {
            _pane = null;
            _currentObjectBuilt = null;
            _dragLastDrawingSpaceMouseLocation = null;
            _dynamicCustomObjects.Clear();
            _selectedDynamicCustomObjects.Clear();
            _staticCustomObjects.Clear();
        }

        #endregion


        public bool BuildDynamicObject(DynamicCustomObject dynamicObject)
        {
            if (_currentObjectBuilt != null)
            {
                return false;
            }

            if (dynamicObject.Initialize(this) == false)
            {
                return false;
            }

            _currentObjectBuilt = dynamicObject;
            return true;
        }

        public void StopBuildingDynamicObject()
        {
            _currentObjectBuilt = null;
            DynamicObjectBuiltEvent(this, null);
            _pane.Invalidate();
        }

        public void HandleDynamicObjectUpdated(DynamicCustomObject dynamicObject)
        {
            _pane.Invalidate();
        }

        public bool Add(CustomObject customObject)
        {
            if (customObject.IsInitialized == false && customObject.Initialize(this) == false)
            {
                return false;
            }
            
            if (customObject is DynamicCustomObject)
            {
                _dynamicCustomObjects.Add((DynamicCustomObject)customObject);
            }
            else
            {
                _staticCustomObjects.Add(customObject);
            }

            _pane.Invalidate();
            return true;
        }

        public bool Remove(CustomObject customObject)
        {
            bool result;
            if (customObject is DynamicCustomObject)
            {
                result = _dynamicCustomObjects.Remove((DynamicCustomObject)customObject);
            }
            else
            {
                result = _staticCustomObjects.Remove(customObject);
            }

            customObject.UnInitialize();
            _pane.Invalidate();
            return result;
        }

        public void Clear()
        {
            for (int i = _dynamicCustomObjects.Count - 1; i > 0; i--)
            {
                Remove(_dynamicCustomObjects[i]);
            }
        }

        public void Draw(GraphicsWrapper g, RectangleF drawingSpaceClipping, CustomObject.DrawingOrderEnum drawingOrder)
        {
            foreach (CustomObject customObject in _dynamicCustomObjects)
            {
                if (customObject.DrawingOrder == drawingOrder)
                {
                    customObject.Draw(g, _pane.CurrentDrawingSpaceMousePosition, drawingSpaceClipping, _pane.DrawingSpace);
                }
            }

            if (_currentObjectBuilt != null)
            {
                _currentObjectBuilt.Draw(g, _pane.CurrentDrawingSpaceMousePosition, drawingSpaceClipping, _pane.DrawingSpace);
            }
        }

        void UpdateSelectedObjects()
        {
            _selectedDynamicCustomObjects.Clear();
            foreach (DynamicCustomObject dynamicObject in _dynamicCustomObjects)
            {
                if (dynamicObject.Selected)
                {
                    _selectedDynamicCustomObjects.Add(dynamicObject);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool OnMouseDoubleClick(MouseEventArgs e)
        {
            bool result = false;

            PointF drawingSpaceLocation = _pane.GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);

            if (_currentObjectBuilt != null)
            {
                _currentObjectBuilt.TrySelect(_pane.GraphicsWrapper.DrawingSpaceTransformClone, drawingSpaceLocation, _pane.DefaultAbsoluteSelectionMargin, !_pane.IsControlKeyDown);
            }
            else
            if (_currentObjectBuilt == null && _dragLastDrawingSpaceMouseLocation.HasValue == false)
            {// Only if dragging is not running try to perform typical selection.

                foreach (DynamicCustomObject dynamicObject in _dynamicCustomObjects)
                {
                    if (dynamicObject.TrySelect(_pane.GraphicsWrapper.DrawingSpaceTransformClone, drawingSpaceLocation, _pane.DefaultAbsoluteSelectionMargin, !_pane.IsControlKeyDown))
                    {
                        if (result == true)
                        {// Only one NEW selection per turn so deny this selection, 
                            // but keep cycling to perform needed deselections.
                            dynamicObject.Selected = false;
                        }

                        result = true;
                    }
                }

                UpdateSelectedObjects();
                _pane.Invalidate();
            }

            return result;
        }

        /// <summary>
        /// Returns true to specify the event has been handled and should not be processed any further.
        /// </summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            if (IsBuildingObject)
            {
                _pane.Focus();

                // If we refresh, the value of the incoming mouse position lags with 1 iteration.
                _pane.Invalidate();
                return false;
            }

            bool refreshRequired = false;

            PointF drawingSpaceLocation = _pane.GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);

            // Handle mouse hover requests.
            foreach (DynamicCustomObject dynamicObject in _dynamicCustomObjects)
            {
                if (dynamicObject.SetMouseHover(drawingSpaceLocation))
                {
                    refreshRequired = true;
                }
            }

            // Handle drag.
            if (_selectedDynamicCustomObjects.Count > 0 && _dragLastDrawingSpaceMouseLocation.HasValue)
            {
                PointF drawingSpaceDragVector = new PointF(drawingSpaceLocation.X - _dragLastDrawingSpaceMouseLocation.Value.X, drawingSpaceLocation.Y - _dragLastDrawingSpaceMouseLocation.Value.Y);
                foreach (DynamicCustomObject dynamicObject in _selectedDynamicCustomObjects)
                {
                    dynamicObject.Drag(drawingSpaceDragVector);
                }

                _dragLastDrawingSpaceMouseLocation = drawingSpaceLocation;
                refreshRequired = true;
            }


            if (refreshRequired)
            {
                _pane.Refresh();
            }

            return false;
        }

        /// <summary>
        /// Returns true to specify the event has been handled and should not be processed any further.
        /// </summary>
        public bool OnMouseLeave(EventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Returns true to specify the event has been handled and should not be processed any further.
        /// </summary>
        public bool OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF drawingSpaceLocation = _pane.GraphicsWrapper.ActualSpaceToDrawingSpace(e.Location, true);

                if (_currentObjectBuilt != null)
                {// Building new object.
                    if (_currentObjectBuilt.AddBuildingPoint(drawingSpaceLocation))
                    {
                        Add(_currentObjectBuilt);
                        DynamicObjectBuiltEvent(this, _currentObjectBuilt);
                        _currentObjectBuilt = null;
                    }
                    return true;
                }
                else
                {// Selection.

                    bool result = false;

                    // First check if we must start dragging.
                    foreach (DynamicCustomObject dynamicObject in _selectedDynamicCustomObjects)
                    {
                        if (dynamicObject.TrySelect(_pane.GraphicsWrapper.DrawingSpaceTransformClone, drawingSpaceLocation, _pane.DefaultAbsoluteSelectionMargin, false))
                        {// One of the selected objects was clicked on so start dragging.
                            _dragLastDrawingSpaceMouseLocation = drawingSpaceLocation;
                            result = true;
                            break;
                        }
                    }

                    //if (_dragLastDrawingSpaceMouseLocation.HasValue == false)
                    //{// Only if dragging is not running try to perform typical selection.
                        
                    //    foreach (DynamicCustomObject dynamicObject in _dynamicCustomObjects)
                    //    {
                    //        if (dynamicObject.TrySelect(_pane.DrawingSpaceTransform, drawingSpaceLocation, _defaultAbsoluteSelectionMargin, !_pane.IsControlKeyDown))
                    //        {
                    //            if (result == true)
                    //            {// Only one NEW selection per turn so deny this selection, 
                    //                // but keep cycling to perform needed deselections.
                    //                dynamicObject.Selected = false;
                    //            }

                    //            result = true;
                    //        }
                    //    }
                    //}

                    if (result)
                    {
                        _pane.Invalidate();
                    }
                    
                    UpdateSelectedObjects();
                    return result;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true to specify the event has been handled and should not be processed any further.
        /// </summary>
        public bool OnMouseUp(MouseEventArgs e)
        {
            if (_dragLastDrawingSpaceMouseLocation.HasValue)
            {// Stop dragging.
                _dragLastDrawingSpaceMouseLocation = null;
                return true;
            }

            if (_currentObjectBuilt != null)
            {// Building object.
                if (e.Button == MouseButtons.Left)
                {
                    UpdateSelectedObjects();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true to specify the event has been handled and should not be processed any further.
        /// </summary>
        public bool OnKeyPress(KeyPressEventArgs e)
        {
            if (_currentObjectBuilt != null)
            {// Building an object - send the key to it.
                if (_currentObjectBuilt.AddBuildingKey(e))
                {// Handled by object.

                    Add(_currentObjectBuilt);
                    DynamicObjectBuiltEvent(this, _currentObjectBuilt);
                    _currentObjectBuilt = null;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true to specify the event has been handled and should not be processed any further.
        /// </summary>
        public bool OnKeyDown(KeyEventArgs e)
        {
            if (IsBuildingObject && e.KeyCode == Keys.Escape)
            {
                StopBuildingDynamicObject();
                return true;
            }

            if (_currentObjectBuilt != null)
            {// Building an object - send the key to it.
                if (_currentObjectBuilt.AddBuildingKey(e))
                {// Handled by object.

                    Add(_currentObjectBuilt);
                    DynamicObjectBuiltEvent(this, _currentObjectBuilt);
                    _currentObjectBuilt = null;

                    return true;
                }
            }

            if (e.KeyCode == Keys.Delete && _selectedDynamicCustomObjects.Count > 0)
            {// Delete.
                foreach (DynamicCustomObject dynamicObject in _selectedDynamicCustomObjects)
                {
                    _dynamicCustomObjects.Remove(dynamicObject);
                }

                _selectedDynamicCustomObjects.Clear();
                _pane.Invalidate();
                return false;
            }

            return false;
        }

        /// <summary>
        /// Returns true to specify the event has been handled and should not be processed any further.
        /// </summary>
        public bool OnKeyUp(KeyEventArgs e)
        {
            return false;
        }


    }
}
