using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tera.Converters;

namespace TCTParser.Processors
{
    public class SectionProcessor
    {
        const int WORLD_ID_OFFSET = 5 * 2;
        const int GUARD_ID_OFFSET = WORLD_ID_OFFSET + 4 * 2;
        const int SECTION_ID_OFFSET = GUARD_ID_OFFSET + 4 * 2;
        

        public uint GetLocationNameId(string p)
        {
            uint id = GetSectionId(p); /*Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(SECTION_ID_OFFSET, 8)));*/
            XElement s = TCTData.TCTDatabase.NewWorldMapData.Descendants("Section").Where(x => (string)x.Attribute("id") == id.ToString()).FirstOrDefault();
            if (s != null)
            {
                id = Convert.ToUInt32(s.Attribute("nameId").Value);
            }
            return id;
        }
        public uint GetSectionId(string p)
        {
            return Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(SECTION_ID_OFFSET, 8)));
        }
        public uint GetGuardId(string p)
        {
            return Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(GUARD_ID_OFFSET, 8)));
        }
        public uint GetWorldId(string p)
        {
            return Convert.ToUInt32(StringUtils.Hex4BStringToInt(p.Substring(WORLD_ID_OFFSET, 8)));
        }

        public string GetLocationName(string p)
        {
            var locId = GetSectionId(p);
            var c = new LocationIdToName();
            return (string)c.Convert(locId, null, null, null);
        }
    }
}
