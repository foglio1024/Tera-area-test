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
    public partial class UserDot : UserControl
    {
        public UserDot(Point pos, int hdng, Color mCol, Color sCol)
        {
            InitializeComponent();

            position = pos;
            heading = hdng;

            MainColor = mCol;
            SecondColor = sCol;

            this.RenderTransformOrigin = new Point(.5, .5);

            Canvas.SetLeft(this,
                (pos.X - MapProvider.CurrentMapData.Position.X) * MapProvider.CurrentMapScale - this.ActualWidth / 2
                );

            Canvas.SetBottom(this,
                (pos.Y - MapProvider.CurrentMapData.Position.Y + MapProvider.CurrentMapData.Size.Height) * MapProvider.CurrentMapScale - this.ActualHeight / 2
                );
            OnNewPosition += Move;
            OnNewAngle += Rotate;
            OnSpawnedChanged += SetSpawnVisuals;
            OnFlyingChanged += SetFligthVisuals;

        }

        private Point position;
        public Point Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    var handler = OnNewPosition;
                    handler.Invoke();
                }
            }
        }
        private int oldA;
        private int heading;
        public int Heading
        {
            get { return heading; }
            set {
                if (heading != value)
                {
                    heading = value;
                    var handler = OnNewAngle;
                    handler.Invoke();
                }
            }
        }

        private Color mainColor;
        public Color MainColor
        {
            get { return mainColor; }

            set
            {
                ell.Fill = new SolidColorBrush(value);
                mainColor = value;
            }
        }

        private Color secondColor;
        public Color SecondColor
        {
            get { return secondColor; }
            set
            {
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
                if (isSpawned != value)
                {
                    isSpawned = value;
                    var handler = OnSpawnedChanged;
                    handler.Invoke();
                }
            }
        }


        private bool isFlying;
        public bool IsFlying
        {
            get { return isFlying; }
            set
            {
                if (isFlying != value)
                {
                    isFlying = value;
                    var handler = OnFlyingChanged;
                    handler.Invoke();
                }
            }
        }

        private void SetFligthVisuals()
        {
            if (isFlying)
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
        private void SetSpawnVisuals()
        {
            if (isSpawned)
            {
                ell.Opacity = 1;
            }
            else
            {
                ell.Opacity = .3;
            }

        }
        private void Move()
        {
            var newX = (Position.X - MapProvider.CurrentMapData.Position.X) * MapProvider.CurrentMapScale - this.ActualWidth / 2;
            var newY = (Position.Y - MapProvider.CurrentMapData.Position.Y + MapProvider.CurrentMapData.Size.Height) * MapProvider.CurrentMapScale - this.ActualHeight / 2;

            var x = new DoubleAnimation(newX, TimeSpan.FromMilliseconds(500));
            var y = new DoubleAnimation(newY, TimeSpan.FromMilliseconds(500));


            this.BeginAnimation(Canvas.LeftProperty, x);
            this.BeginAnimation(Canvas.BottomProperty, y);
        }
        private void Rotate()
        {
            var newA = heading * 360 / 65536;
            var rt = new RotateTransform(newA);
            this.RenderTransform = rt;
            //var r = new DoubleAnimation(newA, TimeSpan.FromMilliseconds(200));
            //rt.BeginAnimation(RotateTransform.AngleProperty, r);
            oldA = newA;
        }
        public void Reposition()
        {
            this.BeginAnimation(Canvas.LeftProperty, null);
            this.BeginAnimation(Canvas.BottomProperty, null);
            Canvas.SetLeft(this,
                (position.X - MapProvider.CurrentMapData.Position.X) * MapProvider.CurrentMapScale - this.ActualWidth / 2
                );
            Canvas.SetBottom(this,
                (position.Y - MapProvider.CurrentMapData.Position.Y + MapProvider.CurrentMapData.Size.Height) * MapProvider.CurrentMapScale - this.ActualHeight / 2
                );
        }

        public event Action OnNewPosition;
        public event Action OnNewAngle;
        public event Action OnFlyingChanged;
        public event Action OnSpawnedChanged;
    }
}
/*
 *         public UserDot(Point position, Color mColor, Color sColor)
        {
            InitializeComponent();
            Spawn += SetSpawnVisuals;
            NewPosition += UpdateLocation;
            FlyingChanged += SetFlyingVisuals;
            //location = loc;
            //id = loc.Id;
            this.RenderTransformOrigin = new Point(.5, .5);
            Canvas.SetLeft(this,
                (position.X - MapProvider.CurrentMapData.Position.X) * MapProvider.CurrentMapScale - this.ActualWidth / 2
                );
            Canvas.SetBottom(this,
                (position.Y - MapProvider.CurrentMapData.Position.Y + MapProvider.CurrentMapData.Size.Height) * MapProvider.CurrentMapScale - this.ActualHeight / 2
                );


        }
        private Point position;
        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                if(position != value)
                {
                    position = value;
                    NewPosition.Invoke();
                }
            }
        }
        
        //private long id;
        //public long Id
        //{
        //    get { return id; }
        //    set { id = value;
        //        ToolTip = value;
        //    }
        //}

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
                //ell.Fill = new SolidColorBrush(Colors.CadetBlue);
                //this.Visibility = Visibility.Visible;
            }
            else
            {
                isSpawned = false;
                ell.Opacity = .3;
                //ell.Fill = new SolidColorBrush(Colors.Red);
                //this.Visibility = Visibility.Hidden;
            }

        }

        //private Location location;
        //public Location Location
        //{
        //    get { return location; }
        //    set {
        //            location = value;
        //            OnLocationChanged(location);
        //    }
        //}
        public event Action<Location> NewPosition;
        //private void OnLocationChanged(Location location)
        //{
        //    var handler = NewPosition;
        //    handler.Invoke(location);
        //}
        private void UpdateLocation()
        {
            var newX = (Position.X - MapProvider.CurrentMapData.Position.X) *MapProvider.CurrentMapScale - this.ActualWidth / 2;
            var newY = (Position.Y - MapProvider.CurrentMapData.Position.Y + MapProvider.CurrentMapData.Size.Height) * MapProvider.CurrentMapScale - this.ActualHeight / 2;
            var newA = l.Heading * 360 / 65536;


            var x = new DoubleAnimation(newX, TimeSpan.FromMilliseconds(300));
            var y = new DoubleAnimation(newY, TimeSpan.FromMilliseconds(300));
            var r = new DoubleAnimation(newA, TimeSpan.FromMilliseconds(300));

            var rt = new RotateTransform(l.Heading * 360 / 65536);
            this.RenderTransform = rt;

            this.BeginAnimation(Canvas.LeftProperty, x);
            this.BeginAnimation(Canvas.BottomProperty, y);
            rt.BeginAnimation(RotateTransform.AngleProperty, r);
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


 * */
