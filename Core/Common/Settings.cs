using System.Xml;

namespace Core.Common
{

    /// <summary>
    /// A class for handling the read/write of settings to and from an XML file
    /// </summary>
    public class Settings
    {
        protected string _width;
        protected string _height;
        protected string _smoothingcycles;
        protected GridType _gridtype;
        protected Tiling _tiling;

        /// <summary>
        /// Farfield width
        /// </summary>
        public float Width
        {
            get => float.Parse(_width);
            set => _width = Convert.ToString(value);
        }

        /// <summary>
        /// Farfield height
        /// </summary>
        public float Height
        {
            get => float.Parse(_height);
            set => _height = Convert.ToString(value);
        }

        /// <summary>
        /// Number of times smoothing method is run per call
        /// </summary>
        public short Smoothingcycles
        {
            get => short.Parse(_smoothingcycles);

            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                _smoothingcycles = Convert.ToString(value);
            }
        }

        /// <summary>
        /// The basic type of grid element that is used to construct the mesh
        /// </summary>
        public GridType Gridtype
        {
            get => _gridtype;
            set => _gridtype = value;
        }

        /// <summary>
        /// The type of tiling that is applied to the grid
        /// </summary>
        public Tiling Tiling
        {
            get => _tiling;
            set => _tiling = value;
        }

        /// <summary>
        /// Assembles and formats the settings data
        /// </summary>
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
                    withBlock.WriteElementString("width", "2");
                    withBlock.WriteElementString("height", "2");
                    withBlock.WriteElementString("smoothingcycles", "8");
                    withBlock.WriteElementString("gridtype", "1");
                    withBlock.WriteElementString("tiling", "0");
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
                _smoothingcycles = childnode.SelectSingleNode("smoothingcycles").InnerText;
                _gridtype = (GridType)Enum.Parse(typeof(GridType), childnode.SelectSingleNode("gridtype").InnerText);
                _tiling = (Tiling)Enum.Parse(typeof(Tiling), childnode.SelectSingleNode("tiling").InnerText);
            }
        }

        /// <summary>
        /// Writes settings to the settings.xml file
        /// </summary>
        /// <param name="farfield"></param>
        public static void WriteSettings(Farfield farfield)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Settings.xml");

            XmlNodeList nodelist = xmlDoc.SelectNodes("settings");

            foreach (XmlNode childnode in nodelist)
            {
                childnode.SelectSingleNode("width").InnerText = Convert.ToString(farfield.Width);
                childnode.SelectSingleNode("height").InnerText = Convert.ToString(farfield.Height);
                childnode.SelectSingleNode("smoothingcycles").InnerText = Convert.ToString(farfield.Smoothingcycles);
                childnode.SelectSingleNode("gridtype").InnerText = Convert.ToString(farfield.Gridtype);
                childnode.SelectSingleNode("tiling").InnerText = Convert.ToString(farfield.Tiling);
            }

            xmlDoc.Save("Settings.xml");
        }
    }
}
