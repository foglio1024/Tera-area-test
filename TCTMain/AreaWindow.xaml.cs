using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCTParser;
using Tera;
using Tera.Game;
using static TCTMain.AreaDataParser;

namespace TCTMain
{
    public static class UI
    {
        public static AreaWindow AreaWindow;
        public static Image MapImage;

        public static void LogWrite(string format, params object[] args)
        {
            var s = String.Format(format, args);
            AreaWindow.Dispatcher.Invoke(() => AreaWindow.TBlog.Text = s);
        }
        public static void UserCountLogWrite(string format, params object[] args)
        {
            var s = String.Format(format, args);
            AreaWindow.Dispatcher.Invoke(() => AreaWindow.TBusers.Text = s);
        }

    }
    public partial class AreaWindow : Window
    {
        void LoadDB()
        {
            NewWorldMapData.PopulateWorldMapData();
            ContinentData.PopulateContinentData();
            StrSheet_Creature.PopulateHuntingZones();
            AreaDatabase.PopulateAreas();
            UserData.PopulateUserData();
            ItemToolTip.PopulateItemToolTip();
            //ItemsDatabase.PopulateItems();

        }
        public AreaWindow()
        {
            AreaDataParser = new AreaDataParser();
            InitializeComponent();

            DamageMeter.Sniffing.TeraSniffer.Instance.Enabled = true;
            DamageMeter.Sniffing.TeraSniffer.Instance.NewConnection += (Tera.Game.Server s) => Dispatcher.Invoke(() => TBc.Text = "Status: connected");
            DamageMeter.Sniffing.TeraSniffer.Instance.MessageReceived += (Message m) => AreaDataParser.ProcessLastPacket(m);
            DamageMeter.Sniffing.TeraSniffer.Instance.EndConnection += () => Dispatcher.Invoke(() => TBc.Text = "Status: disconnected");
            Thread t = new Thread(new ThreadStart(LoadDB));
            t.Start();


            AreaDataParser.SpawnUser += NewUser;

            AreaDataParser.SpawnMe += SpawnPlayer;
            AreaDataParser.MovePlayer += MovePlayer;
            AreaDataParser.MoveUser += MoveUser;
            AreaDataParser.MoveNpc += MoveNpc;
            AreaDataParser.SpawnNpc += SpawnNpc;
            AreaDataParser.DespawnUser += DespawnUser;
            AreaDataParser.DespawnNpc += DespawnNpc;

            AreaDataParser.ChangeSection += SetNewMap;


            UI.AreaWindow = this;
            DataContext = this;

            Users = new ObservableCollection<User>();
            BindingOperations.EnableCollectionSynchronization(Users, new object());
            //lview.ItemsSource = Users;
        }


        ObservableCollection<User> Users;
        List<string> IncompleteUsers;
        List<string> CompleteUsers;
        AreaDataParser AreaDataParser;
        public static MapData CurrentMap;

        public static Canvas EntityCanvas;
        public static Canvas UsersCanvas;
        public static Canvas PlayerCanvas;

        //static List<UserDot> UserDots;
        static User Player;

        //public void AddUser(User u)
        //{
        //    for (int i = 0; i < Users.Count; i++)
        //    {
        //        if(Users[i].Id == u.Id)
        //        {
        //            return;
        //        }
        //    }

        //    var q = new SqlCommand(String.Format("SELECT * FROM TeraUsers WHERE id={0};", u.Id), cn);
        //    SqlDataReader r = q.ExecuteReader();
        //    if (!r.HasRows)
        //    {
        //        r.Close();
        //        var c = new SqlCommand(String.Format("INSERT INTO TeraUsers (id, name, class) VALUES ('{0}', '{1}', '{2}')", u.Id, u.Name, u.Class), cn);
        //        c.ExecuteNonQuery();
        //        c.Dispose();
                
        //    }
                
        //    else r.Close();

        //    Users.Add(u);
        //    users++;
        //}

        private void NewUser(S_SPAWN_USER m)
        {

            User newUser;
            var template = UserData.UserTemplates.Find(x => x.Id == m.templateId);

            if (CompleteUsers.Contains(m.name))
            {
                return;
            }
            else if (IncompleteUsers.Contains(m.name))
            {
                for (int i = IncompleteUsers.Count - 1; i > 0; i--)
                {
                    if (IncompleteUsers[i] == m.name)
                    {
                        newUser = new User(m.userId, m.name, m.guildName, m.guildRank, template.Race, template.Gender, template.TeraClass, m.level, m.weaponId, m.weaponEnchant, m.chestId, m.glovesId, m.bootsId, new Point(m.x, m.y), m.heading, m.srvId);
                        IncompleteUsers.RemoveAt(i);
                        CompleteUsers.Add(newUser.Name);
                        UpdateDataBaseEntry(newUser);
                    }
                }
            }
            else
            {
                newUser = new User(m.userId, m.name, m.guildName, m.guildRank, template.Race, template.Gender, template.TeraClass, m.level, m.weaponId, m.weaponEnchant, m.chestId, m.glovesId, m.bootsId, new Point(m.x, m.y), m.heading, m.srvId);
                CompleteUsers.Add(newUser.Name);
                AddDataBaseEntry(newUser);
            }
        }
        private void SpawnPlayer(S_SPAWN_ME m)
        {
            Dispatcher.Invoke(() =>
            {
                if (Player == null)
                {
                    Player = new User(new Point(m.X, m.Y),m.Heading);
                }
                else
                {
                    Player.Dot.Heading = m.Heading;
                    Player.Dot.Position = new Point(m.X, m.Y);
                }
            });
        }
        int total = 0;
        void AddDataBaseEntry(User u)
        {
            using (SqlConnection cn = new SqlConnection(CS))
            {
                var q = new SqlCommand(String.Format("SELECT * FROM TeraUsers WHERE charName='{0}';", u.Name), cn);
                cn.Open();
                SqlDataReader r = q.ExecuteReader();
                if (!r.HasRows)
                {
                    r.Close();
                    var c = new SqlCommand(String.Format("INSERT INTO TeraUsers (charName, class, race, gender, serverId) VALUES ('{0}', '{1}', '{2}','{3}', {4})", u.Name, u.Class, u.Race, u.Gender, u.ServerId), cn);
                    c.ExecuteNonQuery();
                    c.Dispose();
                    UI.LogWrite("Last user: {0}", u.Name);
                }

                else r.Close();

                q.Dispose();
                q = new SqlCommand("SELECT COUNT(charName) FROM TeraUsers", cn);
                total = (int)q.ExecuteScalar();

                UI.UserCountLogWrite("{0} tot, {1}/{2} c/i", total, CompleteUsers.Count, IncompleteUsers.Count);
            }
        }
        void UpdateDataBaseEntry(User u)
        {
            using (var cn = new SqlConnection(CS))
            {

                var q = new SqlCommand(String.Format("UPDATE TeraUsers SET gender='{1}', race='{2}', serverId='{3}' WHERE charName='{0}';", u.Name, u.Gender, u.Race, u.ServerId), cn);
                cn.Open();
                q.ExecuteNonQuery();
                q.Dispose();

                UI.LogWrite("{0}'s data updated.", u.Name);
            }
        }

        //Movement handlers
        private void MovePlayer(Location l)
        {
            Dispatcher.Invoke(() =>
            {
                Player.Dot.Position = l.Position;
                Player.Dot.Heading = l.Heading;
                Player.Dot.IsFlying = l.IsFlying;
                //Console.WriteLine("[PLAYER][MOVE] {0}", l.Position);
            });
        }
        private void MoveUser(Location l)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (User u in Users)
                {
                    if(u.Id == l.Id)
                    {
                        u.Dot.Position = l.Position;
                        u.Dot.Heading = l.Heading;
                    }
                }
                //if (Users.Find(x => x.Id == l.Id) != null)
                //{
                //    Users.Find(x => x.Id == l.Id).Location = l;

                //    //Console.WriteLine("[USER][MOVE] {0} > {1} -- flying = {2}", l.Id, l.Position, l.IsFlying);
                //}
                //else
                //{
                //    //Console.WriteLine("[USER][ERROR] {0} not found (flying = {1})", l.Id, l.IsFlying);
                //}

            });
        }
        private void MoveNpc(Location l)
        {
            //Dispatcher.Invoke(() => {

            //    if (NpcDots.Find(x => x.Id == l.Id) != null)
            //    {
            //        NpcDots.Find(x => x.Id == l.Id).Location = l;

            //        //Console.WriteLine("[NPC][MOVE] {0} > {1} -- flying = {2}", l.Id, l.Position, l.IsFlying);
            //    }
            //    else
            //    {
            //        //Console.WriteLine("[NPC][ERROR] {0} not found (flying = {1})", l.Id, l.IsFlying);
            //    }

            //});
        }
    
        private void SpawnUser(Location l)
        {
            //Dispatcher.Invoke(() =>
            //{
            //    if (Users.Where(x => x.Id == l.Id) == null)
            //    {
            //        var newUserDot = new UserDot(l);

            //        newUserDot.IsSpawned = true;
            //        newUserDot.MainColor = Colors.CadetBlue;
            //        newUserDot.SecondColor = Colors.White;
                        
                    

            //        UserDots.Add(newUserDot);
            //        EntityCanvas.Children.Add(newUserDot);


            //        //users++;
            //        us.Text = users.ToString();
            //        //Console.WriteLine("[USER][SPAWN] {0}", l.Id);
            //    }
            //    else
            //    {
            //        UserDots.Find(x => x.Id == l.Id).IsSpawned = true;
            //        //Console.WriteLine("[USER][RESPAWN] user: {0}", l.Id);
            //    }
            //});
        }
        private void DespawnUser(long id)
        {
            if (!Properties.Settings.Default.despawn) return;

            Dispatcher.Invoke(() =>
            {
                for (int i = Users.Count - 1; i > 0; i--)
                {
                    if (Users[i].Id == id)
                        Users[i].Dot.IsSpawned = false;

                }
                //users--;
                //us.Text = users.ToString();
                //Console.WriteLine("[USER][REMOVED] {0} removed.", id);
            });
        }

        private void SpawnNpc(Location l)
        {
            //Dispatcher.Invoke(() =>
            //{
            //    if (UserDots.Find(x => x.Id == l.Id) == null)
            //    {
            //        var newNpcDot = new UserDot(l);

            //        newNpcDot.Id = l.Id;
            //        newNpcDot.IsSpawned = true;
            //        newNpcDot.RenderTransformOrigin = new Point(.5, .5);
            //        newNpcDot.MainColor = Colors.Red;
            //        newNpcDot.SecondColor = Colors.White;
                    

            //        NpcDots.Add(newNpcDot);
            //        EntityCanvas.Children.Add(newNpcDot);

            //        //users++;
            //        us.Text = users.ToString();
            //        //Console.WriteLine("[NPC][SPAWN] {0}", l.Id);
            //    }
            //    else
            //    {
            //        UserDots.Find(x => x.Id == l.Id).IsSpawned = true;
            //        //Console.WriteLine("[NPC][RESPAWN] {0}", l.Id);
            //    }
            //});
        }
        private void DespawnNpc(long id)
        {
            //if (!!Properties.Settings.Default.despawn) return;
            //Dispatcher.Invoke(() =>
            //{
            //    for (int i = NpcDots.Count - 1; i > 0; i--)
            //    {
            //        if (NpcDots[i].Id == id)
            //            NpcDots[i].IsSpawned = false;

            //    }
            //    //users--;
            //    //us.Text = users.ToString();
            //    //Console.WriteLine("[NPC][REMOVED] {0} removed.", id);
            //});
        }

        private void Reposition()
        {
            if (!Properties.Settings.Default.S_VISIT_NEW_LOCATION) return;
            Dispatcher.Invoke(() =>
            {
                if (Player != null && Users != null/* && NpcDots != null*/)
                {
                    //PlayerDot.Location = PlayerDot.Location;
                    Player.Dot.Reposition();
                    foreach (var ud in Users)
                    {
                        //try
                        //{
                        //    ud.Location = ud.Location;
                        //}
                        //catch (Exception) { }
                        ud.Dot.Reposition();

                    }
                    //foreach (var nd in NpcDots)
                    //{
                    //    try
                    //    {

                    //    //nd.Location = nd.Location;
                    //    }
                    //    catch (Exception) { }
                    //}
                }

            });
        }

        private void SetNewMap(uint[] ids)
        {
            if (!Properties.Settings.Default.S_VISIT_NEW_LOCATION) return;

            var wId = (int)ids[0];
            var gId = (int)ids[1];
            var sId = (int)ids[2];

            if (sId != MapProvider.CurrentId)
            {
                Dispatcher.Invoke(() =>
                {
                    MapProvider.SetNewMap(NewWorldMapData.GetSection(wId, gId, sId));
                    this.Title = MapProvider.CurrentMapName;
                    try
                    {
                        Reposition();
                    }
                    catch
                    {

                    }
                });
            }


            //Console.WriteLine("[NEW_SECTION] {0}",Section.Current.MapId );
        }
        //Event handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //UI.MapImage = img;
            //MapProvider.SetNewMap(NewWorldMapData.GetGuard("WMap_RNW_Guard"));


            //root.Width = img.ActualWidth;
            //root.Height = img.ActualHeight;



            //EntityCanvas = new Canvas
            //{
            //    Width =  root.Width,
            //    Height = root.Height,
            //    Margin = new Thickness(0)
            //};
            //UsersCanvas = new Canvas
            //{
            //    Width = root.Width,
            //    Height = root.Height,
            //    Margin = new Thickness(0)
            //};
            //PlayerCanvas = new Canvas
            //{
            //    Width = root.Width,
            //    Height = root.Height,
            //    Margin = new Thickness(0),
            //};

            //root.Children.Add(EntityCanvas);
            //root.Children.Add(UsersCanvas);
            //root.Children.Add(PlayerCanvas);

            //UserDots = new List<UserDot>();
            //NpcDots = new List<UserDot>();

            //ChangeSection(new uint[] { 1, 24, 183001 });


            //if (Properties.Settings.Default.S_VISIT_NEW_LOCATION) newSectionCB.IsChecked = true;
            //else newSectionCB.IsChecked = false;
            //
            //if (Properties.Settings.Default.despawn) despawnCB.IsChecked = true;
            //else despawnCB.IsChecked = false;

            
            using(var cn = new SqlConnection(CS))
            {
                var q = new SqlCommand("select count(*) from TeraUsers", cn);
                cn.Open();

                total = (int)q.ExecuteScalar();

                q = new SqlCommand("select charName from TeraUsers where ServerId is null", cn);
                IncompleteUsers = new List<string>();
                using (var r = q.ExecuteReader())
                {
                    while (r.Read())
                    {
                        IncompleteUsers.Add(Convert.ToString(r["charName"]));
                    }
                }
                q = new SqlCommand("select charName from TeraUsers where ServerId is not null", cn);
                CompleteUsers = new List<string>();
                using (var r = q.ExecuteReader())
                {
                    while (r.Read())
                    {
                        CompleteUsers.Add(Convert.ToString(r["charName"]));
                    }
                }
                q = new SqlCommand("select * from TeraServers", cn);
                Servers = new List<Server>();
                using (var r = q.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Servers.Add(new Server(Convert.ToInt32(r["ServerId"]), Convert.ToString(r["ServerName"])));
                    }
                }
                UI.UserCountLogWrite("{0} users ({1}/{2} c/i)", total, CompleteUsers.Count, IncompleteUsers.Count);
            }

        }
        List<Server> Servers;
        class Server
        {
            public int ServerId { get; set; }
            public string ServerName { get; set; }
            public Server(int id, string n)
            {
                ServerId = id;
                ServerName = n;
            }
        }
        string CS = @"Data Source=PC-SOGGIORNO\SQLEXPRESS; Initial Catalog=TCS;Integrated Security=False;User ID=tc;Password=tcstalker;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            DamageMeter.Sniffing.TeraSniffer.Instance.Enabled = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.S_VISIT_NEW_LOCATION = true;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.despawn = true;

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.S_VISIT_NEW_LOCATION = false;

        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.despawn = false;

        }

        private void lview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var l = sender as ListView;
            var i = l.SelectedItem as User;
            //wep.Text = i.WeaponName;
            //chest.Text = i.ArmorName;
            //gloves.Text = i.GlovesName;
            //boots.Text = i.BootsName;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            menu.IsEnabled = true;
            menu.Placement = PlacementMode.Mouse;
            menu.IsOpen = true;

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }



}
