﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Game.Engine;
using Game.Engine.BridgeObjects;
using Point = Game.Engine.Point;

namespace Game
{
    class WpfDrawer : IDrawer
    {
        private Dictionary<uint, IObjectDrawer> _drawSamples;

        private readonly Canvas _canvas;
        private readonly ListBox _listBox;
        private readonly ListBox _heroListBox;
        private readonly Path _appearance;
        private readonly LineGeometry _hands;
        private readonly EllipseGeometry _hat;
        private readonly EllipseGeometry _nose;
        private readonly int _dcenter;
        private RotateTransform _t;
        private PointCollection _visWayCollection;
        private Polyline _visibleWay;
        private BitmapImage appletreeImage;
        private BitmapImage appletree1Image;
        private BitmapImage appletree2Image;

        private BitmapImage plantImage;
        private BitmapImage dryplantImage;
        private BitmapImage growingPlantImage;
        private BitmapImage rockImage;
        private BitmapImage fireImage;

        private BitmapImage appleImage;
        private BitmapImage branchImage;
        private BitmapImage bushImage;
        private BitmapImage raspberryImage;
        private BitmapImage stoneAxeImage;
        private BitmapImage logImage;
        private BitmapImage attenuatingFireImage;
        private BitmapImage spruceTreeImage;
        private BitmapImage coneImage;

        private BitmapImage dikabrozikImage;
        private BitmapImage dikabrozikWithBundleImage;

        private BitmapImage mushroomImage;
        private BitmapImage growingMushroomImage;

        private BitmapImage roastedMushroomImage;
        private BitmapImage roastedAppleImage;

        private BitmapImage twigImage;
        private BitmapImage grassBed0;
        private BitmapImage grassBed1;
        private BitmapImage grassBed2;
        private BitmapImage grassBed3;

        private TextBlock _acting = new TextBlock();
        private int _drawCount = 0;

        public WpfDrawer(Canvas canvas, ListBox listBox, ListBox heroListBox)
        {
            _drawSamples = new Dictionary<uint, IObjectDrawer>();
            _drawSamples[0x00000100] = new TreeDrawer(); // automize
            //_drawSamples[0x00000100] = new TreeDrawer(); // automize

            _dcenter = 8;

            _canvas = canvas;
            _listBox = listBox;
            _heroListBox = heroListBox;
            
            appletreeImage = CreateBitmapImage(@"apple tree icon.png");
            appletree1Image = CreateBitmapImage(@"apple-tree1 icon.png");

            appletree2Image = CreateBitmapImage(@"apple-tree2 icon.png");
            plantImage = CreateBitmapImage(@"plant icon.png");
            dryplantImage = CreateBitmapImage(@"dry plant icon.png");
            growingPlantImage = CreateBitmapImage(@"growing plant icon.png");

            rockImage = CreateBitmapImage(@"rock icon2.png");
            fireImage = CreateBitmapImage(@"fire icon.png");
            appleImage = CreateBitmapImage(@"apple icon.png");
            branchImage = CreateBitmapImage(@"branch icon.png");
            bushImage = CreateBitmapImage(@"brush icon.png");
            raspberryImage = CreateBitmapImage(@"Raspberry icon.png");
            stoneAxeImage = CreateBitmapImage(@"Stone axe icon.png");
            logImage = CreateBitmapImage(@"Log icon.png");
            attenuatingFireImage = CreateBitmapImage(@"attenuating fire small.png");
            spruceTreeImage = CreateBitmapImage(@"spruce tree.png");
            coneImage = CreateBitmapImage(@"cone small.png");

            dikabrozikImage = CreateBitmapImage(@"dikabroyozik small.png");
            dikabrozikWithBundleImage = CreateBitmapImage(@"dikabroyozik with bundle small.png");

            mushroomImage = CreateBitmapImage(@"mushroom small.png");
            growingMushroomImage = CreateBitmapImage(@"mushroom growing small.png");

            roastedMushroomImage = CreateBitmapImage(@"roasted mushroom small.png");
            roastedAppleImage = CreateBitmapImage(@"roasted apple icon.png");

            twigImage = CreateBitmapImage(@"twig icon.png");

            grassBed0 = CreateBitmapImage(@"grassbed0.png");
            grassBed1 = CreateBitmapImage(@"grassbed1.png");
            grassBed2 = CreateBitmapImage(@"grassbed2.png");
            grassBed3 = CreateBitmapImage(@"grassbed3.png");

            CreateActing();

            _appearance = new Path { Fill = Brushes.Yellow, Stroke = Brushes.Brown, Height = 16, Width = 16 };
            Canvas.SetTop(_appearance, 0);
            Canvas.SetLeft(_appearance, 0);

            _hands = new LineGeometry();
            _hands.StartPoint = new System.Windows.Point(8, 0);
            _hands.EndPoint = new System.Windows.Point(8, 16);


            // Create the ellipse geometry to add to the Path
            _hat = new EllipseGeometry();

            _hat.Center = new System.Windows.Point(8, 8);
            _hat.RadiusX = 5;
            _hat.RadiusY = 5;


            _nose = new EllipseGeometry();
            _nose.Center = new System.Windows.Point(14, 8);
            _nose.RadiusX = 2;
            _nose.RadiusY = 2;

            GeometryGroup ggroup = new GeometryGroup();

            ggroup.Children.Add(_hands);
            ggroup.Children.Add(_hat);
            ggroup.Children.Add(_nose);
            ggroup.FillRule = FillRule.Nonzero;

            _appearance.Data = ggroup;

            _t = new RotateTransform();
            _appearance.RenderTransform = _t;
            _appearance.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _canvas.Children.Add(_appearance);

            // polyline
            _visibleWay = new Polyline
                              {
                                  Stroke = System.Windows.Media.Brushes.DarkSlateGray,
                                  StrokeThickness = 2//,
                                  // FillRule = FillRule.EvenOdd
                              };
            _visWayCollection = new PointCollection();
            _visibleWay.Points = _visWayCollection;
        }

        private BitmapImage CreateBitmapImage(string uri)
        {
            var packUrl = new Uri("pack://application:,,,/Icons/" + uri);

            var bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = packUrl;
            bi.EndInit();

            bi.Freeze();

            return bi;
        }


        #region Implementation of IDrawer

        public Func<int, int, List<string>> GetAction { get; set; }

        public void Clear()
        {
            _canvas.UpdateLayout();
            foreach (var disposableChild in _canvas.Children.OfType<IDisposable>())
            {
                disposableChild.Dispose();
            }
            _canvas.Children.Clear();
        }

        public void DrawHero(Point position, double angle, List<Point> wayList)
        {
            _canvas.Children.Add(_visibleWay);
            _visWayCollection.Clear();

            System.Windows.Point point = new System.Windows.Point(position.X, position.Y);
            _visWayCollection.Add(point);
            foreach (var wayPoint in wayList)
            {/*
                if (weakReference.Target == null)
                    continue;
                */
                point = new System.Windows.Point(wayPoint.X, wayPoint.Y);
                _visWayCollection.Add(point);
            }

            _canvas.Children.Add(_appearance);
            Canvas.SetLeft(_appearance, position.X - _dcenter);
            Canvas.SetTop(_appearance, position.Y - _dcenter);
            _t.Angle = angle;
        }

        public void DrawObject(uint id, long x, long y)
        {
            if (id == 0x00000100)
            {
                // _canvas.Children

                DrawImage(appletreeImage, x, y);/*
                Ellipse rec = new Ellipse()
                                  {Fill = Brushes.ForestGreen, Stroke = Brushes.Green, Height = 20, Width = 20};
                rec.ContextMenu = _canvas.ContextMenu;
                _canvas.Children.Add(rec);
                Canvas.SetLeft(rec, x);
                Canvas.SetTop(rec, y);*/
            }
            else if (id == 0x00000200)
            {
                // _canvas.Children

                DrawImage(appletree1Image, x, y);
            }
            else if (id == 0x00000300)
            {
                // _canvas.Children

                DrawImage(appletree2Image, x, y);
            }
            else if (id == 0x00001000)
            {
                // _canvas.Children
                /*      Ellipse rec = new Ellipse() { Fill = Brushes.GreenYellow, Stroke = Brushes.Green, Height = 10, Width = 10 };
                      rec.ContextMenu = _canvas.ContextMenu;
                      _canvas.Children.Add(rec);
                      Canvas.SetLeft(rec, x+10);
                      Canvas.SetTop(rec, y+10);*/

                DrawImage(rockImage, x, y);
            }
            else if (id == 0x00001100)
            {
                // _canvas.Children

                DrawImage(plantImage, x, y);
            }
            else if (id == 0x10001100)
            {
                DrawImage(growingPlantImage, x, y);
            }
            else if (id == 0x20001100)
            {
                DrawImage(dryplantImage, x, y);
            }
            else if (id == 0x00001300)
            {
                // _canvas.Children

                DrawImage(stoneAxeImage, x, y);
            }
            else if (id == 0x00001400)
            {
                // _canvas.Children

                DrawImage(logImage, x, y);
            }
            else if (id == 0x00000600)
            {
                // _canvas.Children
                DrawImage(fireImage, x, y);
            }
            else if (id == 0x00000700)
            {
                // _canvas.Children
                DrawImage(appleImage, x, y);
            }
            else if (id == 0x00000800)
            {
                // _canvas.Children
                DrawImage(branchImage, x, y);
            }
            else if (id == 0x00001200)
            {
                // _canvas.Children
                DrawImage(bushImage, x, y);
            }
            else if (id == 0x00000900)
            {
                // _canvas.Children
                DrawImage(raspberryImage, x, y);
            }
            else if (id == 0x00001500)
            {
                // _canvas.Children
                DrawImage(attenuatingFireImage, x, y);
            }
            else if (id == 0x00001600)
            {
                // _canvas.Children
                DrawImage(spruceTreeImage, x, y);
            }
            else if (id == 0x00001700)
            {
                // _canvas.Children
                DrawImage(coneImage, x, y);
            }
            else if (id == 0x00001900)
            {
                // _canvas.Children
                DrawImage(mushroomImage, x, y);
            }
            else if (id == 0x10001900)
            {
                // _canvas.Children
                DrawImage(growingMushroomImage, x, y);
            }
            else if (id == 0x00001A00)
            {
                // _canvas.Children
                DrawImage(roastedMushroomImage, x, y);
            }
            else if (id == 0x00001B00)
            {
                // _canvas.Children
                DrawImage(roastedAppleImage, x, y);
            }
            else if (id == 0x00001C00)
            {
                // _canvas.Children
                DrawImage(twigImage, x, y);
            }
            else if ((id/0x100) == 0x00001D)
            {
                var innerId = id % 0x10;
               
                switch(innerId){
                    case 0: DrawImage(grassBed0, x, y); break;
                    case 1: DrawImage(grassBed1, x, y); break;
                    case 2: DrawImage(grassBed2, x, y); break;
                    case 3: DrawImage(grassBed3, x, y); break;
                }
                // _canvas.Children
            }
            else if ((id/0x1000) == 0x00018)
            {
                DrawRotatedImage(dikabrozikImage, x, y, id % 0x1000);
            }
            else if ((id / 0x1000) == 0x10018)
            {
                DrawRotatedImage(dikabrozikWithBundleImage, x, y, id % 0x1000);
            }
            else if (id == 0x00002000)
            {
                // _canvas.Children
                Rectangle rec = new Rectangle() { Fill = Brushes.DarkBlue, Stroke = Brushes.DarkBlue, Height = 20, Width = 20 };
                _canvas.Children.Add(rec);
                Canvas.SetLeft(rec, x);
                Canvas.SetTop(rec, y);

            }
            else if (id == 0x00002100)
            {
                // _canvas.Children
                Rectangle rec = new Rectangle() { Fill = Brushes.Blue, Stroke = Brushes.Blue, Height = 20, Width = 20 };
                _canvas.Children.Add(rec);
                Canvas.SetLeft(rec, x);
                Canvas.SetTop(rec, y);
            }/*
            else
            {
                // _canvas.Children
                Rectangle rec = new Rectangle() { Fill = Brushes.LightGreen, Stroke = Brushes.Black, Height = 20, Width = 20 };
                _canvas.Children.Add(rec);
                Canvas.SetLeft(rec, x);
                Canvas.SetTop(rec, y);
            }*/
        }

        public void DrawSurface(uint width, uint height)
        {
           /* Rectangle rec = new Rectangle() { Fill = Brushes.DarkBlue, Stroke = Brushes.Blue, Height = 40, Width = width };
            _canvas.Children.Add(rec);
            Canvas.SetLeft(rec, 0);
            Canvas.SetTop(rec, height - 40);*/
        }

        public void DrawMenu(int x, int y, IEnumerable<ClientAction> actions)
        {

            if (_canvas.ContextMenu == null)
            {
                _canvas.ContextMenu = new ContextMenu();
            }

            var cm = _canvas.ContextMenu;
            if (cm != null)
            {
                if (cm.IsOpen)
                    cm.IsOpen = false;

                cm.Items.Clear();

                foreach (var act in actions)
                {
                    var action = act;
                    var menuItem = new MenuItem()
                    {
                        Header = action.Name,
                        IsEnabled = action.CanDo,
                    };

                    menuItem.Click += (sender, args) => action.Do();

                    cm.Items.Add(menuItem);
                }

                cm.IsOpen = true;
            }
        }

        public void DrawContainer(IEnumerable<KeyValuePair<string, Func<IEnumerable<ClientAction>>>> objects)
        {
            _listBox.Items.Clear();

            foreach (var gameObject in objects)
            {
                var image = new Image();
                image.Source = this.GetBitmapImageByName(gameObject.Key);
               
                TextBlock gameObjecttextBlock = new TextBlock();

                gameObjecttextBlock.TextAlignment = TextAlignment.Center;
                gameObjecttextBlock.Text = gameObject.Key;
                gameObjecttextBlock.DataContext = gameObject.Value;
                gameObjecttextBlock.ContextMenuOpening += HandlerForCMO;

                _listBox.Items.Add(image);
                _listBox.Items.Add(gameObjecttextBlock);
            }
        }

        private ImageSource GetBitmapImageByName(string key)
        {
            string name = key.Remove(key.IndexOf('('));
            switch (name)
            {
                case "Plant":
                    return dryplantImage;
                case "Berries":
                    return appleImage;
                case "Branch":
                    return branchImage;
                case "Rock":
                    return rockImage;
                case "Tree":
                    return appletreeImage;
                case "Bush":
                    return bushImage;
                case "Raspberries":
                    return raspberryImage;
                case "Stone Axe":
                    return stoneAxeImage;
                case "Cone":
                    return coneImage;
                case "Apple":
                    return appleImage;
                case "Burovik":
                    return mushroomImage;
                case "Roasted burovik":
                    return roastedMushroomImage;
                case "Roasted apple":
                    return roastedAppleImage;
                case "Twig":
                    return twigImage;
            }
            return appletreeImage;
        }

        public void DrawHeroProperties(IEnumerable<KeyValuePair<string, int>> objects)
        {
            _heroListBox.Items.Clear();

            foreach (var gameObject in objects)
            {
                _heroListBox.Items.Add(string.Format("{0} - {1}", gameObject.Key, gameObject.Value));
            }
        }

        public void DrawActing(bool showActing)
        {
            _drawCount = (_drawCount + 1)%20;
            if (showActing)
            {
                if (_drawCount == 0)
                {
                    _acting.Text = _acting.Text.Length == 9 ? "Acting." : _acting.Text.Length == 8 ? "Acting..." : "Acting..";
                }

                _canvas.Children.Add(_acting);
                Canvas.SetLeft(_acting, _canvas.Width/2);
                Canvas.SetTop(_acting, _canvas.Height/2);
            }
        }

        private void CreateActing()
        {
            _acting.Foreground = Brushes.FloralWhite;
            _acting.TextAlignment = TextAlignment.Center;
            _acting.FontSize = 16;

            _acting.Text =
                "Acting...";
        }

        private void DrawImage(BitmapSource bitmapImage, long x, long y)
        {
            var image = new Image();
            image.Source = bitmapImage;
            _canvas.Children.Add(image);
            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
        }

        private void DrawRotatedImage(BitmapImage bitmapImage, long x, long y, uint number)
        {   
            var image = new Image();
            image.Source = bitmapImage;
            image.RenderTransform = new RotateTransform((int)number-90);
            image.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _canvas.Children.Add(image);
            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
        }

        void HandlerForCMO(object sender, ContextMenuEventArgs e)
        {
            FrameworkElement fe = e.Source as FrameworkElement;
            IEnumerable<ClientAction> actions = ((Func<IEnumerable<ClientAction>>)fe.DataContext)();
            fe.ContextMenu = BuildMenu(actions);
        }
        ContextMenu BuildMenu(IEnumerable<ClientAction> actions)
        {
            ContextMenu theMenu = new ContextMenu();
           // var actions = this.
            foreach (var act in actions)
            {
                var action = act;
                var menuItem = new MenuItem()
                {
                    Header = action.Name,
                    IsEnabled = action.CanDo,
                };

                menuItem.Click += (sender, args) => action.Do();

                theMenu.Items.Add(menuItem);
            }

            theMenu.IsOpen = true;

            return theMenu;
        }

        #endregion
    }
}
