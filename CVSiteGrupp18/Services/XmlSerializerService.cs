using System.Xml.Serialization;


namespace CVSiteGrupp18.Services
{
    public class XmlSerializerService
    {

        public void Serialize<T>(T obj, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(filestream, obj);
            }
        }
    }
}
