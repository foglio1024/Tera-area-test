using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCTMain
{
    public static class UserData
    {
        static XDocument UserDataDoc;

        public static List<UserTemplate> UserTemplates {get;set;}

        public static void PopulateUserData()
        {
            UserTemplates = new List<UserTemplate>();
            UserDataDoc = XDocument.Load(@"C:\Users\Vincenzo1\Desktop\Progetti VS\TeraDataTools-master\TeraDataTools-master\release\xml\UserData.xml");

            foreach (XElement templateElement in UserDataDoc.Descendants().Where(x => x.Name == "Template"))
            {
                var id = Convert.ToUInt32(templateElement.Attribute("id").Value);
                var teraClass = templateElement.Attribute("class").Value;
                var gender = templateElement.Attribute("gender").Value;
                var race = templateElement.Attribute("race").Value;

                var userTemplate = new UserTemplate(id, teraClass, gender, race);

                UserTemplates.Add(userTemplate);
            }
        }
    }
    public class UserTemplate
    {
        public uint Id { get; set; }
        public string TeraClass { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }

        public UserTemplate(uint id, string tc, string g, string r)
        {
            Id = id;
            TeraClass = tc;
            Gender = g;
            Race = r;
        }
    }
}