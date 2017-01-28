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
                    Analyze(packet);
                    MovePlayer.Invoke(LocationParser.PlayerFlyingLocation(packet));
                    break;
                case "S_USER_LOCATION":
                    MoveUser.Invoke(LocationParser.UserLocation(packet));
                    break;
                case "S_USER_FLYING_LOCATION":
                    MoveUser.Invoke(LocationParser.UserFlyingLocation(packet));
                    break;
                case "S_SPAWN_USER":
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
        private void Analyze(string p)
        {
             var m = new C_PLAYER_FLYING_LOCATION(p);

            var v = new Vector3f { X = m.vx, Y = m.vy };

            Console.WriteLine("{0} {1} {2} - ({3}, {4}, {5}) -> {6}",
                m.unk1.ToString("000000.00"),
                m.unk4.ToString("000000.00"),
                m.unk5.ToString("000000.00"),
                m.vx.ToString("0.00"),
                m.vy.ToString("0.00"),
                m.vz.ToString("0.00"),
                m.Heading
                );

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

        class C_PLAYER_LOCATION : ParsedMessage
        {
            public C_PLAYER_LOCATION(string p)
            {
                packet = p;
                Size = ReadUInt16();
                OpCode = ReadUInt16();

                y = ReadFloat();
                x = ReadFloat();
                z = ReadFloat();

                heading = ReadUInt32();

                y2 = ReadFloat();
                x2 = ReadFloat();
                z2 = ReadFloat();

                unk1 = ReadUInt16();
                unk2 = ReadByte();
                time = ReadUInt32();
            }

            public float y{get; private set;}
            public float x{get; private set;}
            public float z{get; private set;}

            public uint heading{get; private set;}

            public float y2{get; private set;}
            public float x2{get; private set;}
            public float z2{get; private set;}

            public ushort unk1{get; private set;}
            public byte unk2{get; private set;}
            public uint time{get; private set;}

        }
        class C_PLAYER_FLYING_LOCATION : ParsedMessage
        {
            public C_PLAYER_FLYING_LOCATION(string p)
            {
                packet = p;

                Size = ReadUInt16();
                OpCode = ReadUInt16();

                state = ReadUInt32();
                y = ReadFloat();
                x = ReadFloat();
                z = ReadFloat();

                y2 = ReadFloat();
                x2 = ReadFloat();
                z2 = ReadFloat();

                unk1 = ReadFloat();

                unk2 = ReadUInt16();
                unk3 = ReadUInt16();

                unk4 = ReadFloat();
                unk5 = ReadFloat();
                vy = ReadFloat();
                vx = ReadFloat();
                vz = ReadFloat();


            }

            public uint state { get; private set; }

            public float y { get; private set; }
            public float x { get; private set; }
            public float z { get; private set; }

            public float y2 { get; private set; }
            public float x2 { get; private set; }
            public float z2 { get; private set; }
            public float unk1 { get; private set; }

            public ushort unk2 { get; private set; }
            public ushort unk3 { get; private set; }

            public float unk4 { get; private set; }
            public float unk5 { get; private set; }

            public float vy { get; private set; }
            public float vx {get; private set;}
            public float vz { get; private set; }

            public double Heading
            {
                get
                {
                    return 0x8000 * (((-Math.Atan2(vx, -vy)) / Math.PI) + 1);
                }
                private set
                {

                }
            }

        }
        class S_SPAWN_NPC : ParsedMessage
        {
            public S_SPAWN_NPC(string p)
            {
                packet = p;
                Size = ReadUInt16();
                OpCode = ReadUInt16();

                unk1 = ReadInt32();
                unk2 = ReadInt32();

                pointer1 = ReadUInt16();
                id = ReadLong();
                targetId = ReadLong();
                y = ReadFloat();
                x = ReadFloat();
                z = ReadFloat();
                heading = ReadUInt16();
                unk3 = ReadInt32();
                templateId = ReadUInt32();
                zoneId = ReadUInt16();
                unk4 = ReadUInt16();
                unk5 = ReadUInt32();
                unk6 = ReadUInt16();
                unk7 = ReadUInt16();
                unk8 = ReadUInt16();
                flag1 = ReadBool();
                flag2 = ReadBool();
                flag3 = ReadBool();
                unk12_1  = ReadUInt32();
                unk12_2  = ReadUInt32();
                unk12_3  = ReadUInt32();
                unk12_4  = ReadUInt32();
                unk12_5  = ReadUInt32();
                unk12_6  = ReadUInt32();
                unk12_7  = ReadUInt32();
                unk12_8  = ReadUInt32();
                unk12_9  = ReadUInt32();
                unk12_10 = ReadUInt32();
                unk12_11 = ReadUInt32();
                unk12_12 = ReadUInt32();
                unk13 = ReadBool();
                unk14 = ReadUInt16();
                unk15 = ReadUInt16();
                unk16 = ReadUInt16();
                unk17 = ReadUInt32();
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
            public bool flag1{get; private set;}  // true
            public bool flag2{get; private set;} // false for unknown and enemies (invincible?)
            public bool flag3{get; private set;} // almost always true
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
            public bool unk13{get; private set;}
            public ushort unk14{get; private set;} //0x0
            public ushort unk15{get; private set;} //0x1 (bool x2?)
            public ushort unk16{get; private set;}
            public uint unk17{get; private set;}

            //more stuff for longer packets




        }

        class ParsedMessage
        {
            protected string packet;

            protected int pos = 0;

            protected ushort ReadUInt16()
            {
                var result = (ushort)StringUtils.Hex2BStringToInt(packet.Substring(pos));
                pos += 4;
                return result;
            }
            protected uint ReadUInt32()
            {
                var result = (uint)StringUtils.Hex2BStringToInt(packet.Substring(pos));
                pos += 8;
                return result;
            }
            protected int ReadInt32()
            {
                var result = StringUtils.Hex2BStringToInt(packet.Substring(pos));
                pos += 8;
                return result;
            }
            protected long ReadLong()
            {
                var result = StringUtils.Hex8BStringToInt(packet.Substring(pos));
                pos += 16;
                return result;
            }
            protected float ReadFloat()
            {
                var result = StringUtils.Hex4BStringToFloat(packet.Substring(pos));
                pos += 8;
                return result;
            }
            protected bool ReadBool()
            {
                var r = StringUtils.Hex1BStringToInt(packet.Substring(pos));
                if (r == 1)
                {
                    pos += 2;
                    return true;

                }
                else
                {
                    pos += 2;
                    return false;
                }
            }
            protected byte ReadByte()
            {
                var result = StringUtils.Hex1BStringToByte(packet);
                pos += 2;
                return result;
            }

            public ushort Size { get; protected set; }
            public ushort OpCode { get; protected set; }

        }

    }
}
//9067 S_LOAD_TOPO      uint continentId -- float y -- float x -- float z -- byte unk
//CD2E S_SPAWN_ME       long id -- float y -- float x -- float z -- int heading -- byte alive -- byte unk
//spawn npcs and users
//807F C_PLAYER_LOCATION
//E94F C_VISIT_NEW_SECTION