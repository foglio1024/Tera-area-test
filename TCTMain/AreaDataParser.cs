using Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCTParser;
using Tera;
using Tera.Game;
using TCTParser.Processors;

namespace TCTMain
{
    public class AreaDataParser
    {
        string ConformPacket(Tera.Message message)
        {
            byte[] data = new byte[message.Data.Count];
            Array.Copy(message.Data.Array, 0, data, 2, message.Data.Count - 2);
            data[0] = (byte)(((short)message.Data.Count) & 255);
            data[1] = (byte)(((short)message.Data.Count) >> 8);

            if (message.Direction == MessageDirection.ClientToServer && message.OpCode == 19900)
            {
                var msg = new C_CHECK_VERSION_CUSTOM(new CustomReader(message));
                DamageMeter.Sniffing.TeraSniffer.Instance.opn = new OpCodeNamer(System.IO.Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/{msg.Versions[0]}.txt"));
            }

            return StringUtils.ByteArrayToString(data).ToUpper();
        }

        public void ProcessLastPacket(Tera.Message message)
        {
            var packet = ConformPacket(message);

            switch (DamageMeter.Sniffing.TeraSniffer.Instance.opn.GetName(message.OpCode))
            {
                case "C_PLAYER_LOCATION":
                    MovePlayer.Invoke(LocationParser.PlayerLocation(packet));
                    break;
                case "C_PLAYER_FLYING_LOCATION":
                    MovePlayer.Invoke(LocationParser.PlayerFlyingLocation(packet));
                    break;
                case "S_USER_LOCATION":
                    MoveUser.Invoke(LocationParser.UserLocation(packet));
                    break;
                case "S_USER_FLYING_LOCATION":
                    MoveUser.Invoke(LocationParser.UserFlyingLocation(packet));
                    break;
                case "S_SPAWN_USER":
                    AnalyzeParsed(new S_SPAWN_USER(new CustomReader(message)));
                    SpawnUser.Invoke(LocationParser.UserSpawnedLocation(packet));
                    break;
                case "S_DESPAWN_USER":
                    DespawnUser.Invoke(StringUtils.Hex8BStringToInt(packet.Substring(8)));
                    break;
                case "S_SPAWN_NPC":
                    SpawnNpc.Invoke(LocationParser.NpcSpawnedLocation(packet));
                    break;
                case "S_DESPAWN_NPC":
                    DespawnNpc.Invoke(StringUtils.Hex8BStringToInt(packet.Substring(8)));
                    break;
                case "S_NPC_LOCATION":
                    MoveNpc.Invoke(LocationParser.NpcLocation(packet));
                    break;
                case "S_VISIT_NEW_SECTION":
                    uint[] d = { new SectionProcessor().GetWorldId(packet), new SectionProcessor().GetGuardId(packet), new SectionProcessor().GetSectionId(packet) };
                    ChangeSection.Invoke(d);
                    break;
                default:
                    break;
            }
        }
        void AnalyzeParsed(S_SPAWN_USER m)
        {

            var template = UserData.UserTemplates.Find(x => x.Id == m.templateId);
            
            Console.WriteLine("Lv.{4} - {0}:\t {3} {5} {6} -- {2} of {1}",
                m.name,             //0
                m.guildName,        //1 
                m.guildRank,        //2
                template.Race,      //3
                m.level,            //4
                template.Gender,    //5 
                template.TeraClass  //6
                );

            UI.AreaWindow.AddUser(new User(m.userId,m.cId, m.name, m.guildName, m.guildRank, template.Race, template.Gender, template.TeraClass, m.level));

        }
        void Analyze(string p)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < p.Length; i++)
            {
                if(i != 0 && i%8 == 0)
                {
                    sb.Append(" ");
                }
                sb.Append(p[i]);
            }

            Console.WriteLine(sb.ToString());
        }

        class S_LOAD_TOPO 
        {
            public S_LOAD_TOPO(CustomReader r)
            {

                ContinentId = r.ReadUInt32();

                Y = r.ReadSingle();
                X = r.ReadSingle();
                Z = r.ReadSingle();

                Unk = r.ReadByte();
            }  

            public uint ContinentId { get; private set; }
            public float Y { get; private set; }
            public float X { get; private set; }
            public float Z { get; private set; }

            public byte Unk { get; private set; }
        }
        class S_SPAWN_ME 
        {
            public S_SPAWN_ME(CustomReader r)
            {

                Id = r.ReadInt64();

                Y = r.ReadSingle();
                X = r.ReadSingle();
                Z = r.ReadSingle();

                Heading = r.ReadUInt32();

                Alive = r.ReadByte();
                Unk = r.ReadByte();
            }

            public long Id {get;private set; }

            public float Y { get; private set; }
            public float X { get; private set; }
            public float Z { get; private set; }

            public uint Heading { get; private set; }

            public byte Alive { get; private set; }
            public byte Unk { get; private set; }


        }
        class S_SPAWN_NPC 
        {
            public S_SPAWN_NPC(CustomReader r)
            {

                unk1 = r.ReadInt32();
                unk2 = r.ReadInt32();

                pointer1 = r.ReadUInt16();
                id = r.ReadInt64();
                targetId = r.ReadInt64();
                y = r.ReadSingle();
                x = r.ReadSingle();
                z = r.ReadSingle();
                heading = r.ReadUInt16();
                unk3 = r.ReadInt32();
                templateId = r.ReadUInt32();
                zoneId = r.ReadUInt16();
                unk4 = r.ReadUInt16();
                unk5 = r.ReadUInt32();
                unk6 = r.ReadUInt16();
                unk7 = r.ReadUInt16();
                unk8 = r.ReadUInt16();
                flag1 = r.ReadByte();
                flag2 = r.ReadByte();
                flag3 = r.ReadByte();
                unk12_1  = r.ReadUInt32();
                unk12_2  = r.ReadUInt32();
                unk12_3  = r.ReadUInt32();
                unk12_4  = r.ReadUInt32();
                unk12_5  = r.ReadUInt32();
                unk12_6  = r.ReadUInt32();
                unk12_7  = r.ReadUInt32();
                unk12_8  = r.ReadUInt32();
                unk12_9  = r.ReadUInt32();
                unk12_10 = r.ReadUInt32();
                unk12_11 = r.ReadUInt32();
                unk12_12 = r.ReadUInt32();
                unk13 = r.ReadByte();
                unk14 = r.ReadUInt16();
                unk15 = r.ReadUInt16();
                unk16 = r.ReadUInt16();
                unk17 = r.ReadUInt32();
            }

            public int unk1{get; private set;} // 0x0
            public int unk2{get; private set;} // 0x0
            public ushort pointer1{get; private set;} // 0x7A
            public long id{get; private set;}
            public long targetId{get; private set;} //aggro target?
            public float y{get; private set;}
            public float x{get; private set;}
            public float z{get; private set;}
            public ushort heading{get; private set;}
            public int unk3{get; private set;} //0x0C most npc -- 0x0A for unknown
            public uint templateId{get; private set;}
            public ushort zoneId{get; private set;}
            public ushort unk4{get; private set;} //0x64 -- 0x3C -- 0x0
            public uint unk5{get; private set;} //0xA0 -- 0x0 for unknown -- 0x6E -- 0x75 -- 0x10089 -- 0x50 -- 0x64
            public ushort unk6{get; private set;} // 0x0 
            public ushort unk7{get; private set;} // 0x5
            public ushort unk8{get; private set;} // 0x0
            public byte flag1{get; private set;}  // true
            public byte flag2{get; private set;} // false for unknown and enemies (invincible?)
            public byte flag3{get; private set;} // almost always true
            public uint unk12_1{get; private set;} // 0x0
            public uint unk12_2{get; private set;} // 0x0
            public uint unk12_3{get; private set;} // 0x0
            public uint unk12_4{get; private set;} // 0x0
            public uint unk12_5{get; private set;} // 0x0 or 0x1
            public uint unk12_6{get; private set;} // 0x0
            public uint unk12_7{get; private set;} // 0x0
            public uint unk12_8{get; private set;} // 0x0
            public uint unk12_9{get; private set;} // 0x0
            public uint unk12_10{get; private set;}// 0x0
            public uint unk12_11{get; private set;}// 0x0
            public uint unk12_12{get; private set;}// 0x0
            public byte unk13{get; private set;}
            public ushort unk14{get; private set;} //0x0
            public ushort unk15{get; private set;} //0x1 (bool x2?)
            public ushort unk16{get; private set;}
            public uint unk17{get; private set;}

            //more stuff for longer packets




        }
        class S_SPAWN_USER
        {
            public uint Size { get; set; }
            public uint OpCode { get; set; }
            public ushort iconsCount{get; private set;}
            ushort iconsOffset;
            public ushort count2{get; private set;}
            ushort offset2;
            ushort nameOffset;
            ushort guildNameOffset;
            ushort guildRankOffset;
            ushort detailsOffset;
            public ushort detailsCount{get; private set;}
            ushort guildTitleOffset;
            ushort guildEmblemOffset;
            ushort details2Offset;
            public ushort details2Count{get; private set;}
            public long cId{get; private set;}  //?
            public long userId{get; private set;}
            public float y{get; private set;}
            public float x{get; private set;}
            public float z{get; private set;}
            public ushort heading{get; private set;}
            public int relation{get; private set;} //?
            public uint templateId{get; private set;} //UserData.xml
            public ushort unk1{get; private set;} //0x0
            public ushort unk2{get; private set;}
            public ushort unk3{get; private set;}
            public ushort unk4{get; private set;} //0x0
            public ushort unk5{get; private set;} //0x0
            public byte unk6{get; private set;} //1
            public byte unk7{get; private set;} //1
            public long appearance{get; private set;}
            public uint weaponId{get; private set;} //ItemData-i.xml -- StrSheet_Item-i.xml
            public uint chestId{get; private set;} //ItemData-i.xml -- StrSheet_Item-i.xml
            public uint glovesId{get; private set;} //ItemData-i.xml -- StrSheet_Item-i.xml
            public uint bootsId{get; private set;} //ItemData-i.xml -- StrSheet_Item-i.xml
            public uint innerWearId{get; private set;} //ItemData-i.xml -- StrSheet_Item-i.xml
            public uint headItemId{get; private set;} //ItemData-i.xml -- StrSheet_Item-i.xml
            public uint faceItemId{get; private set;} //ItemData-i.xml -- StrSheet_Item-i.xml
            public int unk8{get; private set;} //0x1 or 0x0
            public int unk9{get; private set;}
            public int unk10{get; private set;} //0x07
            public uint unk11{get; private set;}
            public int unk12{get; private set;} //0x0
            public int unk13{get; private set;} // 0x0
            public uint guildLaurelId{get; private set;} //GuildEmblem.xml
            public byte unk14{get; private set;}
            public int unk15{get; private set;} //0x0
            public int unk16{get; private set;} //0x0
            public int unk17{get; private set;} //0x0
            public int unk18{get; private set;} //0x0
            public int unk19{get; private set;} //0x0
            public ushort unk20{get; private set;}
            public byte rchest{get; private set;}
            public byte gchest{get; private set;}
            public byte bchest{get; private set;}
            public byte achest{get; private set;}
            public byte rgloves{get; private set;}
            public byte ggloves{get; private set;}
            public byte bgloves{get; private set;}
            public byte agloves{get; private set;}
            public byte rboots{get; private set;}
            public byte gboots{get; private set;}
            public byte bboots{get; private set;}
            public byte aboots{get; private set;}
            public uint unk21{get; private set;} //0x0
            public uint unk22{get; private set;} //0x0
            public uint unk23{get; private set;} //0x0
            public uint unk24{get; private set;} //0x0
            public uint weaponEnchant{get; private set;}
            public ushort unk25{get; private set;}
            public uint level{get; private set;}
            public int unk26{get; private set;} //0x0
            public int unk27{get; private set;} //0x0
            public byte unk28{get; private set;}
            public uint headCostumeId{get; private set;}
            public uint faceCostumeId{get; private set;}
            public uint backCostumeId{get; private set;}
            public uint weaponCostumeId{get; private set;}
            public uint bodyCostumeId{get; private set;}
            public byte rBodyCostume{get; private set;}
            public byte gBodyCostume{get; private set;}
            public byte bBodyCostume{get; private set;}
            public byte aBodyCostume{get; private set;}
            public int unk29{get; private set;}
            public int unk30{get; private set;}
            public ushort unk31{get; private set;} //0x0001
            public int unk32{get; private set;}
            public int unk33{get; private set;} //0x0
            public int unk34{get; private set;} //0x0, 0x64
            public float unk35{get; private set;} //1.0
            public string name{get; private set;}
            public string guildName{get; private set;}
            public string guildRank{get; private set;}
            public byte[] details{get; private set;}
            public string guildTitle{get; private set;}
            public string guildEmblem{get; private set;}
            public byte[] details2{get; private set;}

            public S_SPAWN_USER(CustomReader r)
            {

                iconsCount = r.ReadUInt16();
                iconsOffset = r.ReadUInt16();

                count2 = r.ReadUInt16();
                offset2 = r.ReadUInt16();

                nameOffset = r.ReadUInt16();
                guildNameOffset = r.ReadUInt16();
                guildRankOffset = r.ReadUInt16();
                detailsOffset = r.ReadUInt16();
                detailsCount = r.ReadUInt16();
                guildTitleOffset = r.ReadUInt16();
                guildEmblemOffset = r.ReadUInt16();
                details2Offset = r.ReadUInt16();
                details2Count = r.ReadUInt16();

                cId = r.ReadInt64();
                userId = r.ReadInt64();

                y = r.ReadSingle();
                x = r.ReadSingle();
                z = r.ReadSingle();

                heading = r.ReadUInt16();

                relation = r.ReadInt32();

                templateId = r.ReadUInt32();

                unk1 = r.ReadUInt16();
                unk2 = r.ReadUInt16();
                unk3 = r.ReadUInt16();
                unk4 = r.ReadUInt16();
                unk5 = r.ReadUInt16();

                unk6 = r.ReadByte();
                unk7 = r.ReadByte();

                appearance = r.ReadInt64();

                weaponId = r.ReadUInt32();
                chestId = r.ReadUInt32();
                glovesId = r.ReadUInt32();
                bootsId = r.ReadUInt32();
                innerWearId = r.ReadUInt32();
                headItemId = r.ReadUInt32();
                faceItemId = r.ReadUInt32();

                unk8 = r.ReadInt32();
                unk9 = r.ReadInt32();
                unk10 = r.ReadInt32();

                unk11 = r.ReadUInt32();

                unk12 = r.ReadInt32();
                unk13 = r.ReadInt32();

                guildLaurelId = r.ReadUInt32();

                unk14 = r.ReadByte();

                unk15 = r.ReadInt32();
                unk16 = r.ReadInt32();
                unk17 = r.ReadInt32();
                unk18 = r.ReadInt32();
                unk19 = r.ReadInt32();

                unk20 = r.ReadUInt16();

                rchest = r.ReadByte();
                gchest  = r.ReadByte();
                bchest  = r.ReadByte();
                achest  = r.ReadByte();

                rgloves = r.ReadByte();
                ggloves = r.ReadByte();
                bgloves = r.ReadByte();
                agloves = r.ReadByte();

                rboots  = r.ReadByte();
                gboots  = r.ReadByte();
                bboots  = r.ReadByte();
                aboots  = r.ReadByte();

                unk21 = r.ReadUInt32();
                unk22 = r.ReadUInt32();
                unk23 = r.ReadUInt32();
                unk24 = r.ReadUInt32();

                weaponEnchant = r.ReadUInt32();

                unk25 = r.ReadUInt16();

                level = r.ReadUInt32();

                unk26 = r.ReadInt32();
                unk27 = r.ReadInt32();

                unk28 = r.ReadByte();

                headCostumeId = r.ReadUInt32();
                faceCostumeId   = r.ReadUInt32();
                backCostumeId   = r.ReadUInt32();
                weaponCostumeId = r.ReadUInt32();
                bodyCostumeId   = r.ReadUInt32();

                rBodyCostume = r.ReadByte();
                gBodyCostume = r.ReadByte();
                bBodyCostume = r.ReadByte();
                aBodyCostume = r.ReadByte();

                unk29 = r.ReadInt32();
                unk30 = r.ReadInt32();
                unk31 = r.ReadUInt16();
                unk32 = r.ReadInt32();
                unk33 = r.ReadInt32();
                unk34 = r.ReadInt32();

                unk35 = r.ReadSingle();

                name = r.ReadTeraString();
                guildName = r.ReadTeraString();
                guildRank = r.ReadTeraString();

                details = new byte[detailsCount];
                for (int i = 0; i < detailsCount; i++)
                {
                    details[i] = r.ReadByte();
                }

                guildTitle = r.ReadTeraString();
                guildEmblem = r.ReadTeraString();

                details2 = new byte[details2Count];
                for (int i = 0; i < details2Count; i++)
                {
                    details2[i] = r.ReadByte();
                }

            }
        }
        class C_PLAYER_LOCATION 
        {
            public C_PLAYER_LOCATION(CustomReader r)
            {
                Y = r.ReadSingle();
                X = r.ReadSingle();
                Z = r.ReadSingle();

                Heading = r.ReadUInt32();

                Y2 = r.ReadSingle();
                X2 = r.ReadSingle();
                Z2 = r.ReadSingle();

                Unk1 = r.ReadUInt16();
                Unk2 = r.ReadByte();
                Time = r.ReadUInt32();
            }

            public float Y{get; private set;}
            public float X{get; private set;}
            public float Z{get; private set;}

            public uint Heading{get; private set;}

            public float Y2{get; private set;}
            public float X2{get; private set;}
            public float Z2{get; private set;}

            public ushort Unk1{get; private set;}
            public byte Unk2{get; private set;}
            public uint Time{get; private set;}

        }
        class C_VISIT_NEW_SECTION 
        {
            public C_VISIT_NEW_SECTION(CustomReader r)
            {
                WorldId = r.ReadUInt32();
                GuardId = r.ReadUInt32();
                SectionId = r.ReadUInt32();
            }

            public uint WorldId { get; private set; }
            public uint GuardId { get; private set; }
            public uint SectionId { get; private set; }
        }

        class C_PLAYER_FLYING_LOCATION 
        {
            public C_PLAYER_FLYING_LOCATION(CustomReader r)
            {

                State = r.ReadUInt32();
                Y = r.ReadSingle();
                X = r.ReadSingle();
                Z = r.ReadSingle();

                Y2 = r.ReadSingle();
                X2 = r.ReadSingle();
                Z2 = r.ReadSingle();

                Unk1 = r.ReadSingle();

                Unk2 = r.ReadUInt16();
                Unk3 = r.ReadUInt16();

                Unk4 = r.ReadSingle();
                Unk5 = r.ReadSingle();
                Vy = r.ReadSingle();
                Vx = r.ReadSingle();
                Vz = r.ReadSingle();


            }

            public uint State { get; private set; }

            public float Y { get; private set; }
            public float X { get; private set; }
            public float Z { get; private set; }

            public float Y2 { get; private set; }
            public float X2 { get; private set; }
            public float Z2 { get; private set; }
            public float Unk1 { get; private set; }

            public ushort Unk2 { get; private set; }
            public ushort Unk3 { get; private set; }

            public float Unk4 { get; private set; }
            public float Unk5 { get; private set; }

            public float Vy { get; private set; }
            public float Vx {get; private set;}
            public float Vz { get; private set; }

            public double Heading
            {
                get
                {
                    return 0x8000 * (((-Math.Atan2(Vx, -Vy)) / Math.PI) + 1);
                }
                private set
                {

                }
            }

        }


        public Action<Location> MovePlayer;

        public Action<Location> MoveUser;
        public Action<Location> MoveNpc;

        public Action<Location> SpawnUser;
        public Action<Location> SpawnNpc;

        public Action<long> DespawnUser;
        public Action<long> DespawnNpc;

        public Action<uint[]> ChangeSection;

        class C_CHECK_VERSION_CUSTOM
        {
            public C_CHECK_VERSION_CUSTOM(CustomReader reader)
            {
                var count = reader.ReadUInt16();
                var offset = reader.ReadUInt16();
                for (var i = 1; i <= count; i++)
                {
                    reader.BaseStream.Position = offset - 4;
                    var pointer = reader.ReadUInt16();
                    var nextOffset = reader.ReadUInt16();
                    var VersionKey = reader.ReadUInt32();
                    var VersionValue = reader.ReadUInt32();
                    Versions.Add(VersionKey, VersionValue);
                    offset = nextOffset;
                }
            }

            public Dictionary<uint, uint> Versions { get; } = new Dictionary<uint, uint>();
        }
        
        class CustomReader : BinaryReader
        {
            public CustomReader(Tera.Message message)
            : base(GetStream(message), Encoding.Unicode)
            {
                Message = message;
            }

            public Tera.Message Message { get; private set; }
            public string OpCodeName { get; private set; }
            public uint Version { get; private set; }
            internal OpCodeNamer SysMsgNamer { get; private set; }

            private static MemoryStream GetStream(Tera.Message message)
            {
                return new MemoryStream(message.Payload.Array, message.Payload.Offset, message.Payload.Count, false, true);
            }

            public EntityId ReadEntityId()
            {
                var id = ReadUInt64();
                return new EntityId(id);
            }

            public Vector3f ReadVector3f()
            {
                Vector3f result;
                result.X = ReadSingle();
                result.Y = ReadSingle();
                result.Z = ReadSingle();
                return result;
            }

            public Angle ReadAngle()
            {
                return new Angle(ReadInt16());
            }

            public void Skip(int count)
            {
                ReadBytes(count);
            }

            // Tera uses null terminated litte endian UTF-16 strings
            public string ReadTeraString()
            {
                var builder = new StringBuilder();
                while (true)
                {
                    var c = ReadChar();
                    if (c == 0)
                        return builder.ToString();
                    builder.Append(c);
                }
            }

        }

        //public class ParsedMessage : BinaryReader
        //{

        //    public ParsedMessage(string p) : base(GetStream(p), Encoding.Unicode)
        //    {
        //        packet = p;
        //    }

        //    private static MemoryStream GetStream(string packet)
        //    {
        //        return new MemoryStream(StringUtils.StringToByteArray(packet));
        //    }
        //    protected string packet;


        //    public ushort Size { get; protected set; }
        //    public ushort OpCode { get; protected set; }


        //}

    }
}
//   S_LOAD_TOPO  >>  S_SPAWN_ME  >>  S_SPAWN_USER/NPC  >>  C_PLAYER_LOCATION   >>  C_VISIT_NEW_SECTION