using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace CommonSupport
{
	/// <summary>
	/// Summary description for ObjectSpace.
	/// </summary>
	public class Space
	{
		public Space()
		{
		}

        SpaceCamera _camera = new SpaceCamera();
        public SpaceCamera Camera
        {
            get
            {
                return _camera;
            }
        }

        SpaceSnap _snap = new SpaceSnap();
        public SpaceSnap Snap
        {
            get
            {
                return _snap;
            }
        }

		System.Drawing.Color _backgroundColor = System.Drawing.Color.Black;
		public System.Drawing.Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				_backgroundColor = value;
			}
		}

        List<Stencil> _stencils = new List<Stencil>();
        public System.Collections.ObjectModel.ReadOnlyCollection<Stencil> Stencils 
		{
			get
			{
                return _stencils.AsReadOnly();
			}
		}

		PointF _mousePosition = new PointF(float.MaxValue, float.MaxValue);

        public bool AllObjectsAtMousePositionAreSelected
        {
            get
            {
                foreach (Stencil objectItem in ObjectsAtMousePosition)
                {
                    if (objectItem.IsSelected == false)
                    {
                        return false;
                    }
                }
                if (ObjectsAtMousePosition.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        List<Stencil> _objectsAtMousePosition = new List<Stencil>();
        public List<Stencil> ObjectsAtMousePosition
		{
			get
			{
				return _objectsAtMousePosition;
			}
		}

        public Stencil[] SelectedStencils
        {
            get
            {
                List<Stencil> result = new List<Stencil>();
                foreach (Stencil stencil in Stencils)
                {
                    if ( stencil.IsSelected )
                    {
                        result.Add(stencil);
                    }
                }

                Stencil[] stencils = new Stencil[result.Count];
                result.CopyTo(stencils);
                return stencils;
            }

            set
            {
                foreach (Stencil stencil in Stencils)
                {
                    stencil.IsSelected = false;
                }

                foreach (Stencil stencil in value)
                {
                    stencil.IsSelected = true;
                }
            }
        }


        public void SetSelectedStencilsFromList(List<Stencil> stencils)
        {
            Stencil[] array = new Stencil[stencils.Count];
            stencils.CopyTo(array);
            SelectedStencils = array;
        }


		/// <summary>
		/// This will trigger all the events related to setting up the proper properties on the objects, related to mouse pos (like ObjectsAtMousePosition)
		/// </summary>
		/// <param name="mousePosition"></param>
		public void SetMousePosition(PointF mousePosition)
		{
			_mousePosition = mousePosition;

            _objectsAtMousePosition.Clear();

            foreach (Stencil stencil in Stencils)
			{
                stencil.SetMousePosition(mousePosition);
                if (stencil.IsMouseOver)
                {
                    _objectsAtMousePosition.Add(stencil);
                }
			}
		}
		
		List<Stencil> GetObjectsAt(PointF inputObjectSpacePoint)
		{
            List<Stencil> resultingObjects = new List<Stencil>();
			foreach(Stencil stencil in Stencils)
			{
				if(stencil.IsPointInside(inputObjectSpacePoint))
				{
					resultingObjects.Add(stencil);
				}
			}
			return resultingObjects;
		}

        //public PointF ObjectSpaceToDrawSpace(PointF inputPoint)
        //{
        //    inputPoint.X += Camera.Position.X;
        //    inputPoint.X *= Camera.Zoom;
        //    inputPoint.Y += Camera.Position.Y;
        //    inputPoint.Y *= Camera.Zoom;
        //    return inputPoint;
        //}

        public PointF DrawSpaceToObjectSpace(PointF inputPoint)
        {
            PointF[] points = new PointF[] { inputPoint };
            Matrix transformationInverted = Camera.TransformationMatrix.Clone();
            transformationInverted.Invert();
            transformationInverted.TransformPoints(points);
            //inputPoint.X -= Camera.Position.X;
            //inputPoint.X *= 1 / Camera.Zoom;
            //inputPoint.Y -= Camera.Position.Y;
            //inputPoint.Y *= 1 / Camera.Zoom;
            return points[0];
        }

		public void AddStencil(Stencil stencil)
		{
            System.Diagnostics.Debug.Assert(_stencils.Contains(stencil) == false, "Adding objectItem more than 1 time.");
            _stencils.Add(stencil);
		}

		public void RemoveStencil(Stencil stencil)
        {
            System.Diagnostics.Debug.Assert(_stencils.Contains(stencil) == true, "Removing not added object.");
            _stencils.Remove(stencil);
		}
	}
}
