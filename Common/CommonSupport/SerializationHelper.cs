using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using CommonSupport;


namespace CommonSupport
{
    /// <summary>
    /// Helps in persisting objects. Provides surrogate selectors for serializing
    /// non serializables like Pen, Brush, etc.
    /// </summary>
    public class SerializationHelper
    {
        /// <summary>
        /// Any serialization above this limit will produce a warning.
        /// </summary>
        public static int SerializationWarningLimit = 1024 * 1024;

        ///// <summary>
        ///// This is an extremely partial state persistence, does not include contained control.
        ///// </summary>
        //public class DragControlSurrogate : ISerializationSurrogate
        //{
        //    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        //    {
        //        DragControl control = (DragControl)obj;
        //        info.AddValue("location", control.Location);
        //        info.AddValue("text", control.Text);
        //        info.AddValue("name", control.Name);
        //        info.AddValue("dock", control.Dock);
        //    }

        //    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        //    {
        //        DragControl control = new DragControl();
        //        control.Location = (Point)info.GetValue("location", typeof(Point));
        //        control.Text = info.GetString("text");
        //        control.Name = info.GetString("name");
        //        control.Dock = (DockStyle)info.GetValue("dock", typeof(DockStyle));
                
        //        return control;
        //    }

        //}

        public class MatrixSurrogate : ISerializationSurrogate
        {
            #region ISerializationSurrogate Members

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Matrix matrix = (Matrix)obj;
                info.AddValue("elements", matrix.Elements);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                float[] elements = (float[])info.GetValue("elements", typeof(float[]));
                return new Matrix(elements[0], elements[1], elements[2],elements[3], elements[4], elements[5]);
            }

            #endregion
        }

        public class PenSurrogate : ISerializationSurrogate
        {
            #region ISerializationSurrogate Members

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Pen pen = (Pen)obj;

                ManualSerialize("color", pen.Color, info);
                info.AddValue("dashStyle", (int)pen.DashStyle);
                // Can not serialize - throws "OutOfMemoryException"
                //info.AddValue("dashPattern", pen.DashPattern);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Color color;
                ManualDeSerialize(info, "color", out color);

                Pen pen = new Pen(color);
                pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)info.GetInt32("dashStyle");
                //pen.DashPattern = (float[])info.GetValue("dashPattern", typeof(float[]));
                return pen;
            }

            #endregion
        }

        public class FontSurrogate : ISerializationSurrogate
        {
            #region ISerializationSurrogate Members

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Font font = (Font)obj;
                info.AddValue("fontFamily", font.FontFamily.Name);
                info.AddValue("fontSize", font.Size);
                info.AddValue("fontStyle", (int)font.Style);

            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                string fontFamily = info.GetString("fontFamily");
                float fontSize = info.GetSingle("fontSize");
                FontStyle fontStyle = (FontStyle)info.GetInt32("fontStyle");
                return new Font(fontFamily, fontSize, fontStyle);
            }

            #endregion
        }

        public class SolidBrushSurrogate : ISerializationSurrogate
        {
            #region ISerializationSurrogate Members

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                SolidBrush brush = (SolidBrush)obj;
                ManualSerialize("color", brush.Color, info);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Color color;
                ManualDeSerialize(info, "color", out color);
                SolidBrush brush = new SolidBrush(color);
                return brush;
            }

            #endregion
        }

        public class BlendSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Blend blend = (Blend)obj;
                info.AddValue("factors", blend.Factors);
                info.AddValue("positions", blend.Positions);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Blend blend = new Blend();
                blend.Factors = (float[])info.GetValue("factors", typeof(float[]));
                blend.Positions = (float[])info.GetValue("positions", typeof(float[]));

                return blend;
            }
        }

        public class ColorBlendSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                ColorBlend blend = (ColorBlend)obj;
                info.AddValue("colors", blend.Colors);
                info.AddValue("positions", blend.Positions);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                ColorBlend blend = new ColorBlend();
                blend.Colors = (Color[])info.GetValue("colors", typeof(Color[]));
                blend.Positions = (float[])info.GetValue("positions", typeof(float[]));

                return blend;
            }
        }

        public class LinearGradientBrushSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                LinearGradientBrush brush = (LinearGradientBrush)obj;
                ManualSerialize("linearColor0", brush.LinearColors[0], info);
                ManualSerialize("linearColor1", brush.LinearColors[1], info);

                info.AddValue("blend", brush.Blend);

                //info.AddValue("interpolationColors", brush.InterpolationColors);
                info.AddValue("transform", brush.Transform);
                info.AddValue("rectangle", brush.Rectangle);
                info.AddValue("wrapMode", (int)brush.WrapMode);
                info.AddValue("gammaCorrection", brush.GammaCorrection);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                RectangleF rectangle = (RectangleF)info.GetValue("rectangle", typeof(RectangleF));

                Color color1, color2;
                ManualDeSerialize(info, "linearColor0", out color1);
                ManualDeSerialize(info, "linearColor1", out color2);

                LinearGradientBrush brush = new LinearGradientBrush(rectangle, color1, color2, LinearGradientMode.ForwardDiagonal);
                brush.WrapMode = (WrapMode)info.GetInt32("wrapMode");
                brush.GammaCorrection = info.GetBoolean("gammaCorrection");

                //brush.InterpolationColors = (ColorBlend)info.GetValue("interpolationColors", typeof(ColorBlend));
                brush.Blend = (Blend)info.GetValue("blend", typeof(Blend));
                brush.Transform = (Matrix)info.GetValue("transform", typeof(Matrix));
                return brush;
            }
        }

        /// <summary>
        /// TODO: Reusing the formatter will probably speed up the serializations allot, since it is 
        /// complex creation procedure.
        /// </summary>
        /// <returns></returns>
        public static IFormatter GenerateFormatter()
        {
            // 1. Construct the desired formatter
            IFormatter formatter = new BinaryFormatter();

            // 2. Construct a SurrogateSelector object
            SurrogateSelector surrogateSelector = new SurrogateSelector();

            // 3. Tell the surrogate selector to use our object when a 
            // object is serialized/deserialized
            //surrogateSelector.AddSurrogate(typeof(Color),
            //   new StreamingContext(StreamingContextStates.All),
            //   new ColorSurrogate());

            //surrogateSelector.AddSurrogate(typeof(DragControl),
            //   new StreamingContext(StreamingContextStates.All),
            //   new MatrixSurrogate());

            surrogateSelector.AddSurrogate(typeof(Matrix),
               new StreamingContext(StreamingContextStates.All),
               new MatrixSurrogate());

            surrogateSelector.AddSurrogate(typeof(Pen),
               new StreamingContext(StreamingContextStates.All),
               new PenSurrogate());

            surrogateSelector.AddSurrogate(typeof(SolidBrush),
               new StreamingContext(StreamingContextStates.All),
               new SolidBrushSurrogate());

            surrogateSelector.AddSurrogate(typeof(LinearGradientBrush),
               new StreamingContext(StreamingContextStates.All),
               new LinearGradientBrushSurrogate());

            surrogateSelector.AddSurrogate(typeof(Blend),
               new StreamingContext(StreamingContextStates.All),
               new BlendSurrogate());

            surrogateSelector.AddSurrogate(typeof(ColorBlend),
               new StreamingContext(StreamingContextStates.All),
               new ColorBlendSurrogate());

            formatter.SurrogateSelector = surrogateSelector;

            return formatter;
        }

        /// <summary>
        /// Will clone the object using binary serialization.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object BinaryClone(object input)
        {
            byte[] bytes = Serialize(input);
            object result = null;
            if (DeSerialize(bytes, out result) == false)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Perform object serialization to a array of bytes.
        /// </summary>
        public static byte[] Serialize(object p)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(ms, p);
                ms.Flush();
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// Helper, overrides.
        /// </summary>
        public static bool DeSerialize(byte[] data, out object value)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return DeSerialize(ms, out value);
            }
        }

        /// <summary>
        /// Perform deserialization of an object from a stream.
        /// </summary>
        public static bool DeSerialize(MemoryStream stream, out object value)
        {
            try
            {
                IFormatter formatter = GenerateFormatter();
                value = formatter.Deserialize(stream);
            }
            catch (Exception ex)
            {
                SystemMonitor.Error("Failed to deserialize object.", ex);
                value = null;
                return false;
            }

            return true;
        }

        public static bool Serialize(MemoryStream stream, object p)
        {
            try
            {
                IFormatter formatter = GenerateFormatter();
                formatter.Serialize(stream, p);
                if (stream.Position > SerializationWarningLimit)
                {
                    SystemMonitor.Warning("Serialialization of object [" + p.GetType().Name + "] has grown above the default serialization limit to [" + stream.Position.ToString() + "] bytes.");
                }

                return true;
            }
            catch (Exception ex)
            {
                SystemMonitor.Error("Failed to serialize object [" + p.GetType().Name + "," + ex.Message + "].");
                return false;
            }
        }

        ///// <summary>
        ///// Special helper for doing data from a SerializationInfo.
        ///// </summary>
        //public static void SerializeInfo(Stream stream, SerializationInfo info)
        //{
        //    try
        //    {
        //        IFormatter formatter = GenerateFormatter();

        //        SerializationInfoEnumerator enumerator = info.GetEnumerator();
        //        while (enumerator.MoveNext())
        //        {
        //            formatter.SaveState(stream, enumerator.Current.Name);
        //            formatter.SaveState(stream, enumerator.Current.Value);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SystemMonitor.Error("Failed to serialize info [" + ex.Message + "].");
        //    }
        //}

        ///// <summary>
        ///// Special helper for doing data from a SerializationInfo.
        ///// </summary>
        //public static void DeSerializeInfo(SerializationInfo info, Stream stream)
        //{
        //    try
        //    {
        //        IFormatter formatter = GenerateFormatter();

        //        string name;
        //        do
        //        {
        //            name = (string)formatter.Deserialize(stream);
        //            object value = formatter.Deserialize(stream);
        //            info.AddValue(name, value);
        //        }
        //        while (string.IsNullOrEmpty(name) == false);
        //    }
        //    catch (Exception ex)
        //    {
        //        SystemMonitor.Error("Failed to serialize info [" + ex.Message + "].");
        //    }
        //}

        public static void ManualSerialize(Dictionary<string, Pen> pens, SerializationInfo info)
        {
            // There are problems with serializing arrays of pens (they require a single pen to also be serialized to reference the Pen surrogate or something).
            // so this model of serialization is used.
            info.AddValue("outResultPenCount", pens.Count);
            int index = 0;
            foreach (string name in pens.Keys)
            {
                info.AddValue("outResultPenName." + index.ToString(), name);
                info.AddValue("outResultPen." + index.ToString(), pens[name]);
                index++;
            }
        }

        public static void ManualDeSerialize(SerializationInfo info, Dictionary<string, Pen> pens)
        {
            // There are problems with serializing arrays of pens (they require a single pen to also be serialized to reference the Pen surrogate or something).
            // so this model of serialization is used.
            int penCount = info.GetInt32("outResultPenCount");
            for (int i = 0; i < penCount; i++)
            {
                string name = info.GetString("outResultPenName." + i.ToString());
                Pen pen = (Pen)info.GetValue("outResultPen." + i.ToString(), typeof(Pen));
                pens.Add(name, pen);
            }
        }

        /// <summary>
        /// Color surrogate does not work (it is a reference type, so this might be the problem).
        /// </summary>
        public static void ManualSerialize(string name, Color color, SerializationInfo info)
        {
            info.AddValue(name, color.ToArgb());
        }

        public static void ManualDeSerialize(SerializationInfo info, string name, out Color color)
        {
            color = Color.FromArgb(info.GetInt32(name));
        }
    }
}
