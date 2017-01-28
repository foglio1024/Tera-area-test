using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCTMain
{
    /// <summary>
    /// Logica di interazione per UserDot.xaml
    /// </summary>
    public partial class UserDot : UserControl
    {
        public UserDot()
        {
            InitializeComponent();
            Spawn += SetSpawnVisuals;
            NewLocation += UpdateLocation;
            FlyingChanged += SetFlyingVisuals;

            location = new Location(0, 0, 0, 0, 0, false);
        }


        private long id;
        public long Id
        {
            get { return id; }
            set { id = value;
                ToolTip = value;
            }
        }

        private Color mainColor;
        public Color MainColor
        {
            get { return mainColor; }
        
            set {
                ell.Fill = new SolidColorBrush(value);
                mainColor = value;
            }
        }

        private Color secondColor;
        public Color SecondColor
        {
            get { return secondColor; }
            set {
                ell.Stroke = new SolidColorBrush(value);
                r.Stroke = new SolidColorBrush(value);
                r.Fill = new SolidColorBrush(value);
                d.Fill = new SolidColorBrush(value);
                secondColor = value;
            }
        }


        private bool isSpawned;
        public bool IsSpawned
        {
            get { return isSpawned; }
            set {
                if (value != isSpawned)
                {
                    isSpawned = value;
                    OnSpawnedChanged(value);
                }
            }
        }
        public event Action<bool> Spawn;
        private void OnSpawnedChanged(bool _spawned)
        {
            var handler = Spawn;
            handler.Invoke(_spawned);
        }
        private void SetSpawnVisuals(bool _spawned)
        {
            if (_spawned)
            {
                isSpawned = true;
                ell.Opacity = 1;
                ell.Fill = new SolidColorBrush(Colors.CadetBlue);
                this.Visibility = Visibility.Visible;
            }
            else
            {
                isSpawned = false;
                //ell.Opacity = .3;
                //ell.Fill = new SolidColorBrush(Colors.Red);
                this.Visibility = Visibility.Hidden;
            }

        }

        private Location location;
        public Location Location
        {
            get { return location; }
            set {
                    location = value;
                    OnLocationChanged(location);
            }
        }
        public event Action<Location> NewLocation;
        private void OnLocationChanged(Location location)
        {
            var handler = NewLocation;
            handler.Invoke(location);
        }
        private void UpdateLocation(Location l)
        {
            Canvas.SetLeft(this,
                (l.Position.X  - Section.Current.MapData.Position.X) * MapData.Scale 
                );
            Canvas.SetBottom(this,
                (l.Position.Y - Section.Current.MapData.Position.Y+Section.Current.MapData.Size.Height) * MapData.Scale
                );
            
            this.RenderTransform = new RotateTransform(l.Heading * 360 / 65536);
            IsFlying = l.IsFlying;
        }


        private bool isFlying;
        public bool IsFlying
        {
            get { return isFlying; }
            set {
                if (isFlying != value)
                {
                    isFlying = value;
                    OnFlyingChanged(value);
                }
            }
        }
        public event Action<bool> FlyingChanged;
        private void OnFlyingChanged(bool value)
        {
            var handler = FlyingChanged;
            handler.Invoke(value);
        }
        private void SetFlyingVisuals(bool flight)
        {
            if (flight)
            {
                r.Visibility = Visibility.Hidden;
                d.Visibility = Visibility.Visible;
            }
            else
            {
                d.Visibility = Visibility.Hidden;
                r.Visibility = Visibility.Visible;
            }

        }

        public void Translate(MapData newMap, MapData oldMap)
        {
            Canvas.SetLeft(this,
                (this.Location.Position.X - newMap.Position.X + oldMap.Position.X) * MapData.Scale
                );
            Canvas.SetBottom(this,
                (this.Location.Position.Y - newMap.Position.Y  + oldMap.Position.X + newMap.Size.Height) * MapData.Scale
                );

        }

    }
}
