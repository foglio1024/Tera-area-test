using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCTMain
{
    public static class ItemToolTip
    {
        static XDocument ItemToolTipDoc;

        public static List<Grade> Grades { get; set; }
        public static List<Class> Classes { get; set; }
        public static List<Gender> Genders { get; set; }
        public static List<Race> Races { get; set; }
        public static List<ItemCategory> ItemCategories { get; set; }

        public static void PopulateItemToolTip()
        {
            Grades = new List<Grade>();
            Classes = new List<Class>();
            Genders = new List<Gender>();
            Races = new List<Race>();
            ItemCategories = new List<ItemCategory>();

            ItemToolTipDoc = XDocument.Load(@"C:\Users\Vincenzo1\Desktop\Progetti VS\TeraDataTools-master\TeraDataTools-master\release\xml\ItemToolTip.xml");

            foreach (XElement grades in ItemToolTipDoc.Descendants().Where(x => x.Name == "Grade").Descendants())
            {
                var name = grades.Attribute("eng").Value;
                var text = grades.Attribute("displayText").Value;
                Grades.Add(new Grade(name, text));
            }
            foreach (XElement classes in ItemToolTipDoc.Descendants().Where(x => x.Name == "Class").Descendants())
            {
                var name = classes.Attribute("eng").Value;
                var text = classes.Attribute("displayText").Value;
                Classes.Add(new Class(name, text));
            }
            foreach (XElement genders in ItemToolTipDoc.Descendants().Where(x => x.Name == "Gender").Descendants())
            {
                var name = genders.Attribute("eng").Value;
                var text = genders.Attribute("displayText").Value;
                Genders.Add(new Gender(name, text));
            }
            foreach (XElement races in ItemToolTipDoc.Descendants().Where(x => x.Name == "Race").Descendants())
            {
                var name = races.Attribute("eng").Value;
                var text = races.Attribute("displayText").Value;
                Races.Add(new Race(name, text));
            }
            foreach (XElement itemCat in ItemToolTipDoc.Descendants().Where(x => x.Name == "ItemCategory").Descendants())
            {
                var name = itemCat.Attribute("eng").Value;
                var text = itemCat.Attribute("displayText").Value;
                ItemCategories.Add(new ItemCategory(name, text));
            }
        }
    }

    public class Grade
    {
        public string Name { get; set; }
        public string DisplayedText { get; set; }
        public Grade(string n, string t)
        {
            Name = n;
            DisplayedText = t;
        }
    }
    public class Class
    {
        public string Name { get; set; }
        public string DisplayedText { get; set; }
        public Class(string n, string t)
        {
            Name = n;
            DisplayedText = t;
        }

    }
    public class Gender
    {
        public string Name { get; set; }
        public string DisplayedText { get; set; }
        public Gender(string n, string t)
        {
            Name = n;
            DisplayedText = t;
        }

    }
    public class Race
    {
        public string Name { get; set; }
        public string DisplayedText { get; set; }
        public Race(string n, string t)
        {
            Name = n;
            DisplayedText = t;
        }

    }
    public class ItemCategory
    {
        public string Name { get; set; }
        public string DisplayedText { get; set; }
        public ItemCategory(string n, string t)
        {
            Name = n;
            DisplayedText = t;
        }

    }
}
