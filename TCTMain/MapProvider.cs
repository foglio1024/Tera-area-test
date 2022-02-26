using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TCTMain
{
    public static class MapProvider
    {
        public static string CurrentMapName { get; set; } = "";
        public static double CurrentMapScale { get; set; } = .01;

        static MapData currentMapData = new MapData();
        public static MapData CurrentMapData
        {
            get
            {
                return currentMapData;
            }
            set
            {
                if (currentMapData != value)
                {
                    currentMapData = value;
                    //MapChanged.Invoke();
                }
            }
        }

        public static Scope CurrentScope;
        static int currentSectionId;
        static int currentGuardId;

        public static int CurrentId
        {
            get
            {
                if (CurrentScope == Scope.Section)
                {
                    return currentSectionId;
                }
                else
                {
                    return currentGuardId;
                }
            }

            set { }
        }

        public static void SetNewMap(Section s)
        {

            CurrentMapName = s.MapId;
            CurrentMapData = s.MapData;
            currentSectionId = s.Id;
            CurrentScope = Scope.Section;
            UI.AreaWindow.Dispatcher.Invoke(() =>
            {
                //CurrentMapScale = UI.MapImage.Height / CurrentMapData.Size.Height;

                var fileName = CurrentMapName;
                if (fileName.Contains("Empty"))
                {
                    fileName = fileName.Replace("Empty", "Field");
                }

                //UI.MapImage.Source = new BitmapImage(new Uri("resources/maps/" + fileName + ".png", UriKind.RelativeOrAbsolute));
            });

        }
        public static void SetNewMap(Guard g)
        {

            CurrentMapName = g.MapId;
            CurrentMapData = g.MapData;
            currentGuardId = g.Id;
            CurrentScope = Scope.Guard;

            //CurrentMapScale = UI.MapImage.Height / CurrentMapData.Size.Height;

            //UI.MapImage.Source = new BitmapImage(new Uri("resources/maps/" + CurrentMapName + ".png", UriKind.RelativeOrAbsolute));


        }

    }
    public enum Scope
    {
        Section,
        Guard
    }
}
