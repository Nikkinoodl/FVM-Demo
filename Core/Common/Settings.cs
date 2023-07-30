using System.Xml;

namespace Core.Common
{
    public class Settings
    {
        protected string _width;
        protected string _height;
        protected string _scale;
        protected string _layers;
        protected string _cellheight;
        protected string _cellfactor;
        protected string _nodetrade;
        protected string _expansionpower;
        protected string _offset;
        protected string _smoothingcycles;
        protected string? _airfoilnodes;
        protected string _filename;
        protected GridType _gridtype;


        // on read takes the string and converts to specific type
        // on write takes a specific type and converts to a string
        // 
        public int Width
        {
            get => int.Parse(_width);
            set => _width = Convert.ToString(value);
        }
        public int Height
        {
            get => int.Parse(_height);
            set => _height = Convert.ToString(value);
        }
        public float Scale
        {
            get => float.Parse(_scale);
            set => _scale = Convert.ToString(value);
        }
        public short Layers
        {
            get => short.Parse(_layers);
            set => _layers = Convert.ToString(value);
        }
        public float Cellheight
        {
            get => float.Parse(_cellheight);
            set => _cellheight = Convert.ToString(value);
        }
        public double Cellfactor
        {
            get => double.Parse(_cellfactor);
            set => _cellfactor = Convert.ToString(value);
        }
        public short Nodetrade
        {
            get => short.Parse(_nodetrade);
            set => _nodetrade = Convert.ToString(value);
        }
        public double Expansionpower
        {
            get => double.Parse(_expansionpower);
            set => _expansionpower = Convert.ToString(value);
        }
        public short Offset
        {
            get => short.Parse(_offset);

            set
            {
                if (value < 2)
                {
                    //MessageBox.Show(MeshConstants.MSGMINOFFSET);
                    value = 2;
                }
                _offset = Convert.ToString(value);
            }
        }
        public short Smoothingcycles
        {
            get => short.Parse(_smoothingcycles);

            set
            {
                if (value < 1)
                {
                    //MessageBox.Show(MeshConstants.MSGSMOOTHINGCYCLES);
                    value = 1;
                }
                _smoothingcycles = Convert.ToString(value);
            }
        }
        public short? Airfoilnodes
        {
            get => short.Parse(_airfoilnodes);
            set
            {
                if (value == null) value = 0;
                
                _airfoilnodes = Convert.ToString(value);
            }
        }
        public string Filename
        {
            get => _filename;
            set => _filename = value;
        }
        public GridType Gridtype
        {
            get => _gridtype;
            set => _gridtype = value;
        }

        public void CreateSettings()
        {

            // creates a settings.xml file if one doesn't already exist
            if (File.Exists("Settings.xml") == false)
            {
                XmlDocument doc = new();
                XmlWriter writer;

                 writer = doc.CreateNavigator().AppendChild();

                {
                    var withBlock = writer;
                    withBlock.WriteStartDocument();
                    withBlock.WriteStartElement("settings");
                    withBlock.WriteElementString("width", "1000");
                    withBlock.WriteElementString("height", "600");
                    withBlock.WriteElementString("scale", "500");
                    withBlock.WriteElementString("layers", "4");
                    withBlock.WriteElementString("cellheight", "40");
                    withBlock.WriteElementString("cellfactor", "0.95");
                    withBlock.WriteElementString("nodetrade", "4");
                    withBlock.WriteElementString("expansionpower", "0.3");
                    withBlock.WriteElementString("offset", "2");
                    withBlock.WriteElementString("smoothingcycles", "8");
                    withBlock.WriteElementString("filename", @"C:\users\simon\OneDrive\documents\Apps\Airfoil.csv");
                    //withBlock.WriteElementString("gridtype", "1");
                    withBlock.WriteEndElement();
                    withBlock.WriteEndDocument();
                    withBlock.Close();
                }
                doc.Save("Settings.xml");
            }

            // read settings from settings.xml
            XmlDocument xmlDoc = new();
            xmlDoc.Load("Settings.xml");

            XmlNodeList nodelist = xmlDoc.SelectNodes("settings");

            foreach (XmlNode childnode in nodelist)
            {
                _width = childnode.SelectSingleNode("width").InnerText;
                _height = childnode.SelectSingleNode("height").InnerText;
                _scale = childnode.SelectSingleNode("scale").InnerText;
                _layers = childnode.SelectSingleNode("layers").InnerText;
                _cellheight = childnode.SelectSingleNode("cellheight").InnerText;
                _cellfactor = childnode.SelectSingleNode("cellfactor").InnerText;
                _nodetrade = childnode.SelectSingleNode("nodetrade").InnerText;
                _expansionpower = childnode.SelectSingleNode("expansionpower").InnerText;
                _offset = childnode.SelectSingleNode("offset").InnerText;
                _smoothingcycles = childnode.SelectSingleNode("smoothingcycles").InnerText;
                _filename = childnode.SelectSingleNode("filename").InnerText;
               // _gridtype = childnode.SelectSingleNode("gridtype").InnerText;
            }
        }

        // Write settings to the settings.xml file
        public static void WriteSettings(Farfield farfield)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Settings.xml");

            XmlNodeList nodelist = xmlDoc.SelectNodes("settings");

            foreach (XmlNode childnode in nodelist)
            {
                childnode.SelectSingleNode("width").InnerText = Convert.ToString(farfield.Width);
                childnode.SelectSingleNode("height").InnerText = Convert.ToString(farfield.Height);
                childnode.SelectSingleNode("scale").InnerText = Convert.ToString(farfield.Scale);
                childnode.SelectSingleNode("layers").InnerText = Convert.ToString(farfield.Layers);
                childnode.SelectSingleNode("cellheight").InnerText = Convert.ToString(farfield.Cellheight);
                childnode.SelectSingleNode("cellfactor").InnerText = Convert.ToString(farfield.Cellfactor);
                childnode.SelectSingleNode("nodetrade").InnerText = Convert.ToString(farfield.Nodetrade);
                childnode.SelectSingleNode("expansionpower").InnerText = Convert.ToString(farfield.Expansionpower);
                childnode.SelectSingleNode("offset").InnerText = Convert.ToString(farfield.Offset);
                childnode.SelectSingleNode("smoothingcycles").InnerText = Convert.ToString(farfield.Smoothingcycles);
                childnode.SelectSingleNode("filename").InnerText = Convert.ToString(farfield.Filename);
                childnode.SelectSingleNode("gridtype").InnerText = Convert.ToString(farfield.Gridtype);
            }

            xmlDoc.Save("Settings.xml");
        }
    }
}
