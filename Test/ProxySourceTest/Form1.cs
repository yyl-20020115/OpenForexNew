using System;
using System.Windows.Forms;

namespace ProxySourceTest
{
    public partial class Form1 : Form
    {
        TestAdapterImplementation _implementation = new TestAdapterImplementation();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SubscribeMessage message = new SubscribeMessage();

            //BinaryFormatter a;
            //a.UnsafeDeserialize(KeyDown, new System.Runtime.Remoting.Messaging.HeaderHandler(
            //a.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.

            //IFormatter formatter = new SoapFormatter();
            //Stream stream = new FileStream(@"d:\MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            //formatter.Serialize(stream, message);
            //stream.Close();

            _implementation.RegisterSource();
        }
    }
}
