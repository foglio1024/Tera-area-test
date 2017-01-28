using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTMain;

namespace TCTParser
{
    public static class LocationParser
    {
        public static Location PlayerLocation(string v)
        {
            return new Location(
                1024,
                StringUtils.Hex4BStringToFloat(v.Substring(8)),
                StringUtils.Hex4BStringToFloat(v.Substring(16)),
                StringUtils.Hex4BStringToFloat(v.Substring(24)),
                StringUtils.Hex2BStringToInt(v.Substring(32)),
                false
                );


        }
        public static Location PlayerFlyingLocation(string v)
        {
            return new Location(
                1024,
                StringUtils.Hex4BStringToFloat(v.Substring(16)),
                StringUtils.Hex4BStringToFloat(v.Substring(24)),
                StringUtils.Hex4BStringToFloat(v.Substring(32)),
                0,
                true
                );


        }
        public static Location UserLocation(string v)
        {
            return new Location(
                StringUtils.Hex8BStringToInt(v.Substring(8)),
                StringUtils.Hex4BStringToFloat(v.Substring(24)),
                StringUtils.Hex4BStringToFloat(v.Substring(32)),
                StringUtils.Hex4BStringToFloat(v.Substring(40)),
                StringUtils.Hex2BStringToInt(v.Substring(48)),
                false
                );


        }
        public static Location UserFlyingLocation(string v)
        {
            return new Location(
             StringUtils.Hex8BStringToInt(v.Substring(8)),
             StringUtils.Hex4BStringToFloat(v.Substring(16 + 16)),
             StringUtils.Hex4BStringToFloat(v.Substring(24 + 16)),
             StringUtils.Hex4BStringToFloat(v.Substring(32 + 16)),
             0,
             true
             );


        }
        public static Location UserSpawnedLocation(string v)
        {
            return new Location(
                StringUtils.Hex8BStringToInt(v.Substring(38 * 2)),
                StringUtils.Hex4BStringToFloat(v.Substring(46 * 2)),
                StringUtils.Hex4BStringToFloat(v.Substring(46 * 2 + 8)),
                StringUtils.Hex4BStringToFloat(v.Substring(46 * 2 + 16)),
                StringUtils.Hex2BStringToInt(v.Substring(46 * 2 + 24)),
                false
                );

        }
        public static Location NpcLocation(string v)
        {
            return new Location(
                StringUtils.Hex8BStringToInt(v.Substring(8)),
                StringUtils.Hex4BStringToFloat(v.Substring(24)),
                StringUtils.Hex4BStringToFloat(v.Substring(32)),
                StringUtils.Hex4BStringToFloat(v.Substring(40)),
                StringUtils.Hex2BStringToInt(v.Substring(48)),
                false
                );


        }
        public static Location NpcSpawnedLocation(string v)
        {
            return new Location(
                StringUtils.Hex8BStringToInt(v.Substring(14 * 2)),
                StringUtils.Hex4BStringToFloat(v.Substring(30 * 2)),
                StringUtils.Hex4BStringToFloat(v.Substring(30 * 2 + 8)),
                StringUtils.Hex4BStringToFloat(v.Substring(30 * 2 + 16)),
                StringUtils.Hex2BStringToInt(v.Substring(30 * 2 + 24)),
                false
                );

        }
    }

}
