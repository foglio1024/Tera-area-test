using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCTParser;
using Tera;
using Tera.Game;

namespace TCTMain
{
    public static class UI
    {
        public static AreaWindow AreaWindow;
    }
    public class User
    {
        public long Id { get; set; }
        public long CId { get; set; }
        public string Name { get; set; }
        public string Guild { get; set; }
        public string Rank { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }
        public uint Level { get; set; }

        public User(long id,long cid, string n, string g, string r, string race, string gender, string c, uint l)
        {
            Id = id;
            CId = cid;
            Name = n;
            Guild = g;
            Rank = r;
            Class = ItemToolTip.Classes.Find(x => x.Name.Equals(c, StringComparison.OrdinalIgnoreCase)).DisplayedText;
            Level = l;
            Gender = ItemToolTip.Genders.Find(x => x.Name.Equals(gender, StringComparison.OrdinalIgnoreCase)).DisplayedText;

            if (race == "popori" && gender == "female")
            {
                Race = ItemToolTip.Races.Find(x => x.Name.Equals("elin", StringComparison.OrdinalIgnoreCase)).DisplayedText;
            }
            else
            {
                Race = ItemToolTip.Races.Find(x => x.Name.Equals(race, StringComparison.OrdinalIgnoreCase)).DisplayedText; ;
            }

        }
    }
    /// <summary>
    /// Logica di interazione per Window1.xaml
    /// </summary>
    public partial class AreaWindow : Window
    {
        public AreaWindow()
        {
            InitializeComponent();

            DamageMeter.Sniffing.TeraSniffer.Instance.Enabled = true;
            DamageMeter.Sniffing.TeraSniffer.Instance.NewConnection += (Server s) => Dispatcher.Invoke(() => TBc.Text = "Status: connected");         
            DamageMeter.Sniffing.TeraSniffer.Instance.MessageReceived += (Message m) => AreaDataParser.ProcessLastPacket(m);
            DamageMeter.Sniffing.TeraSniffer.Instance.EndConnection += () =>Dispatcher.Invoke(() => TBc.Text = "Status: disconnected");

            NewWorldMapData.PopulateWorldMapData();
            ContinentData.PopulateContinentData();
            StrSheet_Creature.PopulateHuntingZones();
            AreaDatabase.PopulateAreas();
            UserData.PopulateUserData();
            ItemToolTip.PopulateItemToolTip();

            AreaDataParser = new AreaDataParser();

            AreaDataParser.MovePlayer += MovePlayer;

            AreaDataParser.MoveUser += MoveUser;
            AreaDataParser.MoveNpc += MoveNpc;

            AreaDataParser.SpawnUser += SpawnUser;
            AreaDataParser.SpawnNpc += SpawnNpc;

            AreaDataParser.DespawnUser += DespawnUser;
            AreaDataParser.DespawnNpc += DespawnNpc;

            AreaDataParser.ChangeSection += SetNewMap;

            Section.SectionChanged += ChangeSection;

            UI.AreaWindow = this;
            DataContext = this;

            Users = new ObservableCollection<User>();
            BindingOperations.EnableCollectionSynchronization(Users, new object());
            lview.ItemsSource = Users;
        }
        ObservableCollection<User> Users; 

        AreaDataParser AreaDataParser;
        public static Guard CurrentGuard;
        static int users;

        static Canvas EntityCanvas;
        static Canvas UsersCanvas;
        static Canvas PlayerCanvas;

        static List<UserDot> UserDots;
        static List<UserDot> NpcDots;
        static UserDot PlayerDot;

        public void AddUser(User u)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if(Users[i].Id == u.Id)
                {
                    return;
                }
            }
            Users.Add(u);
            users++;
        }


        //Movement handlers
        private void MovePlayer(Location l)
        {
            Dispatcher.Invoke(() =>
            {
                PlayerDot.Location = l;
                //Console.WriteLine("[PLAYER][MOVE] {0}", l.Position);
            });
        }
        private void MoveUser(Location l)
        {
            Dispatcher.Invoke(() => {

                if (UserDots.Find(x => x.Id == l.Id) != null)
                {
                    UserDots.Find(x => x.Id == l.Id).Location = l;

                    //Console.WriteLine("[USER][MOVE] {0} > {1} -- flying = {2}", l.Id, l.Position, l.IsFlying);
                }
                else
                {
                    //Console.WriteLine("[USER][ERROR] {0} not found (flying = {1})", l.Id, l.IsFlying);
                }

            });
        }
        private void MoveNpc(Location l)
        {
            Dispatcher.Invoke(() => {

                if (NpcDots.Find(x => x.Id == l.Id) != null)
                {
                    NpcDots.Find(x => x.Id == l.Id).Location = l;

                    //Console.WriteLine("[NPC][MOVE] {0} > {1} -- flying = {2}", l.Id, l.Position, l.IsFlying);
                }
                else
                {
                    //Console.WriteLine("[NPC][ERROR] {0} not found (flying = {1})", l.Id, l.IsFlying);
                }

            });
        }

        private void SpawnUser(Location l)
        {
            Dispatcher.Invoke(() =>
            {
                if (UserDots.Find(x => x.Id == l.Id) == null)
                {
                    var newUserDot = new UserDot
                    {
                        Id = l.Id,
                        IsSpawned = true,
                        RenderTransformOrigin = new Point(.5, .5),
                        MainColor = Colors.CadetBlue,
                        SecondColor = Colors.White,
                        Location = l
                    };

                    UserDots.Add(newUserDot);
                    EntityCanvas.Children.Add(newUserDot);

                    //users++;
                    us.Text = users.ToString();
                    //Console.WriteLine("[USER][SPAWN] {0}", l.Id);
                }
                else
                {
                    UserDots.Find(x => x.Id == l.Id).IsSpawned = true;
                    //Console.WriteLine("[USER][RESPAWN] user: {0}", l.Id);
                }
            });
        }
        private void DespawnUser(long id)
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = UserDots.Count - 1; i > 0; i--)
                {
                    if (UserDots[i].Id == id)
                        UserDots[i].IsSpawned = false;

                }
                //users--;
                //us.Text = users.ToString();
                //Console.WriteLine("[USER][REMOVED] {0} removed.", id);
            });
        }

        private void SpawnNpc(Location l)
        {
            Dispatcher.Invoke(() =>
            {
                if (UserDots.Find(x => x.Id == l.Id) == null)
                {
                    var newNpcDot = new UserDot
                    {
                        Id = l.Id,
                        IsSpawned = true,
                        RenderTransformOrigin = new Point(.5, .5),
                        MainColor = Colors.Red,
                        SecondColor = Colors.White,
                        Location = l
                    };

                    NpcDots.Add(newNpcDot);
                    EntityCanvas.Children.Add(newNpcDot);

                    //users++;
                    us.Text = users.ToString();
                    //Console.WriteLine("[NPC][SPAWN] {0}", l.Id);
                }
                else
                {
                    UserDots.Find(x => x.Id == l.Id).IsSpawned = true;
                    //Console.WriteLine("[NPC][RESPAWN] {0}", l.Id);
                }
            });
        }
        private void DespawnNpc(long id)
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = NpcDots.Count - 1; i > 0; i--)
                {
                    if (NpcDots[i].Id == id)
                        NpcDots[i].IsSpawned = false;

                }
                //users--;
                //us.Text = users.ToString();
                //Console.WriteLine("[NPC][REMOVED] {0} removed.", id);
            });
        }

        private void ChangeSection()
        {
            Dispatcher.Invoke(() =>
            {
                if (PlayerDot != null && UserDots != null && NpcDots != null)
                {
                    PlayerDot.Location = PlayerDot.Location;

                    foreach (var ud in UserDots)
                    {
                        ud.Location = ud.Location;

                    }
                    foreach (var nd in NpcDots)
                    {
                        nd.Location = nd.Location;
                    }
                }

                string n = Section.Current.MapId;
                if (Section.Current.MapId.Contains("Empty"))
                {
                    n = n.Replace("Empty", "Field");
                }

                img.Source = new BitmapImage(new Uri("resources/maps/" + n + ".png", UriKind.RelativeOrAbsolute));
                this.Title = Section.Current.MapId;
            });
        }

        private void SetNewMap(uint[] ids)
        {
            var wId = (int)ids[0];
            var gId = (int)ids[1];
            var sId = (int)ids[2];

            Section.Current = NewWorldMapData.GetSection(wId,gId,sId);

            Dispatcher.Invoke(() => {
                MapData.Scale = img.Width / Section.Current.MapData.Size.Width;
            });

            //Console.WriteLine("[NEW_SECTION] {0}",Section.Current.MapId );
        }

        //Event handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            Section.Current = NewWorldMapData.GetSection("WMap_RNW_Vill");

            if(Section.Current != null)
            {
                MapData.Scale = img.Width / Section.Current.MapData.Size.Width;
            }


            root.Width = img.ActualWidth;
            root.Height = img.ActualHeight;



            EntityCanvas = new Canvas
            {
                Width =  root.Width,
                Height = root.Height,
                Margin = new Thickness(0)
            };
            UsersCanvas = new Canvas
            {
                Width = root.Width,
                Height = root.Height,
                Margin = new Thickness(0)
            };
            PlayerCanvas = new Canvas
            {
                Width = root.Width,
                Height = root.Height,
                Margin = new Thickness(0),
            };

            root.Children.Add(EntityCanvas);
            root.Children.Add(UsersCanvas);
            root.Children.Add(PlayerCanvas);

            PlayerDot = new UserDot
            {
                RenderTransformOrigin = new Point(.5, .5),       
                MainColor = Colors.White,
                SecondColor = Colors.Black         
            };
            PlayerCanvas.Children.Add(PlayerDot);

            UserDots = new List<UserDot>();
            NpcDots = new List<UserDot>();

            //ChangeSection(new uint[] { 1, 24, 183001 });


            MovePlayer(new Location(0, 0, 0, 0, 0, false));

        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            DamageMeter.Sniffing.TeraSniffer.Instance.Enabled = false;
        }
    }



}
