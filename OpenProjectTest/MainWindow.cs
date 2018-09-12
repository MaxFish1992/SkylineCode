using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TerraExplorerX;

namespace OpenProjectTest
{
    public partial class MainWindow : Form
    {
        private SGWorld66 sgWorld = null;
        private double x = 0;
        private double y = 0;
        private string effectXML = null;

        public MainWindow()
        {
            InitializeComponent();

            this.InitEvent();
            this.OpenProject();
        }
        private void InitEvent()
        {
            this.txttextLabelMenuItem.Click += TxttextLabelMenuItem_Click;
            this.txtCircleMenuItem.Click += TxtCircleMenuItem_Click;
            this.txtPolylineMenuItem.Click += TxtPolylineMenuItem_Click;
            this.popupsMenuItemm.Click += PopupsMenuItemm_Click;
            this.treeGroupMenuItem.Click += TreeGroupMenuItem_Click;
            this.arrowMenuItem.Click += ArrowMenuItem_Click;
            this.getLayerIDMenuItem.Click += GetLayerIDMenuItem_Click;
            this.fireMenuItem.Click += FireMenuItem_Click;
            this.imageLabelToolStripMenuItem.Click += imageLabelToolStripMenuItem_Click;
            this.btnRouteRoam.Click += BtnRouteRoam_Click;
        }
        #region 事件
        /// <summary>
        /// 路径漫游
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRouteRoam_Click(object sender, EventArgs e)
        {
            RouteRoam();
        }
        /// <summary>
        /// 创建图片标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateImageLabel();
        }
        private void FireMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateFireEffect();
        }

        private void GetLayerIDMenuItem_Click(object sender, EventArgs e)
        {
            this.effectXML = this.GetEffectXML();
        }

        private void ArrowMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateArrow();
        }

        private void TreeGroupMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateHierarchy();
        }

        private void PopupsMenuItemm_Click(object sender, EventArgs e)
        {
            this.CreatePopupMessage();
        }

        private void TxtPolylineMenuItem_Click(object sender, EventArgs e)
        {
            this.CreatePolyline();
        }

        private void TxttextLabelMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateLabel();
        }

        private void TxtCircleMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateCircle();
        }

        private void SgWorld_OnFrame()
        {
            this.sgWorld.OnLButtonClicked += SgWorld_OnLButtonClicked;
            //var cPos = this.sgWorld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
            //var cX = cPos.X;
            //var cY = cPos.Y;
            //if (x == cX && y == cY)
            //{
            //    Console.WriteLine("飞行结束：" + "X:" + cX + "______Y:" + cY);
            //}
            //else
            //{
            //    x = cX;
            //    y = cY;
            //    Console.WriteLine("正在飞行中....");
            //} 
        }

        private bool SgWorld_OnLButtonClicked(int Flags, int X, int Y)
        {
            //返回地形上的真实世界坐标，位于窗口中指定像素的后面。如果所选像素是对象的一部分，那么也会返回该对象。
            var worldPointInfo66 = sgWorld.Window.PixelToWorld(X, Y, WorldPointType.WPT_DEFAULT);
            //根据是否获取到对象Id,判断所选是否包含对象
            if (worldPointInfo66.ObjectID == "") return false;
            //获取对象
            var explorerObject66 = sgWorld.ProjectTree.GetObject(worldPointInfo66.ObjectID);
            //判断对象类型[特征要素对象]
            if (explorerObject66 == null) return false;
            if (explorerObject66.ObjectType != ObjectTypeCode.OT_IMAGE_LABEL) return false;
            var imageLabel = explorerObject66 as ITerrainImageLabel66;
            var data = imageLabel.ClientData["id"];
            var type = imageLabel.GetParam(1).ToString();
            return true;
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 打开本地工程文件
        /// </summary>
        private void OpenProject()
        {
            string msg = String.Empty;

            string appRoot = Path.GetDirectoryName(Application.ExecutablePath);
            string projectUrl = @"F:\项目管理\智慧抚州\使用的Fly\Default2.fly";
            bool isAsync = false;
            string user = String.Empty;
            string password = String.Empty;

            try
            {
                sgWorld = new SGWorld66();
                sgWorld.OnFrame += SgWorld_OnFrame;
                sgWorld.Project.Open(projectUrl, isAsync, user, password);
            }
            catch (Exception ex)
            {
                msg = String.Format("OpenProjectButton_Click Exception: {0}", ex.Message);
                MessageBox.Show(msg);
            }
        }
        /// <summary>
        /// 创建TextLabel
        /// </summary>
        private void CreateLabel()
        {
            string msg = String.Empty;
            IPosition66 cPos = null;
            ILabelStyle66 cLabelStyle = null;
            ITerrainImageLabel66 cTextLabel = null;

            try
            {
                cPos = CreatePosition();

                SGLabelStyle eLabelStyle = SGLabelStyle.LS_DEFAULT;
                cLabelStyle = this.sgWorld.Creator.CreateLabelStyle(eLabelStyle);

                uint nBGRValue = 0xFF0000;  // Blue
                double dAlpha = 0.5;        // 50% opacity
                var cBackgroundColor = cLabelStyle.BackgroundColor; // Get label style background color
                cBackgroundColor.FromBGRColor(nBGRValue);               // Set background to blue
                cBackgroundColor.SetAlpha(dAlpha);                      // Set transparency to 50%
                cLabelStyle.BackgroundColor = cBackgroundColor;         // Set label style background color
                cLabelStyle.FontName = "Arial";                         // Set font name to Arial
                cLabelStyle.Italic = true;                              // Set label style font to italic
                cLabelStyle.Scale = 3;                                  // Set label style scale

                string tText = "Skyline";
                cTextLabel = this.sgWorld.Creator.CreateTextLabel(cPos, tText, cLabelStyle, string.Empty, "TextLabel");

                //FlyTo text label
                var cFlyToPos = cPos.Copy();
                cFlyToPos.Pitch = -89.0;
                this.sgWorld.Navigate.FlyTo(cFlyToPos, ActionCode.AC_FLYTO);

            }
            catch (Exception ex)
            {
                msg = String.Format("CreateLabelButton_Click Exception: {0}", ex.Message);
                MessageBox.Show(msg);
            }
        }
        /// <summary>
        /// 创建图片标签
        /// </summary>
        private void CreateImageLabel()
        {
            IPosition66 cPos = CreatePosition();
            string imageFileName = @"F:\项目管理\智慧抚州\使用的Fly\data11\汽车图标\汽车整车.png";
            var imageLabel = this.sgWorld.Creator.CreateImageLabel(cPos, imageFileName,null,"","1");
            imageLabel.set_ClientData("id", "1");
            imageLabel.SetParam(1, "汽车整车");
            var cFlyToPos = cPos.Copy();
            cFlyToPos.Pitch = -89.0;
            this.sgWorld.Navigate.FlyTo(cFlyToPos, ActionCode.AC_FLYTO);
        }
        /// <summary>
        /// 创建圆
        /// </summary>
        private void CreateCircle()
        {
            string msg = string.Empty;
            IPosition66 cPos = null;
            IColor66 cFillColor = null;
            ITerrainRegularPolygon66 cCircle = null;
            ITerraExplorerMessage66 cMessage = null;

            try
            {
                cPos = CreatePosition();

                uint nLineColor = 0xFFFF0000;
                double dCircleRadius = 200;

                cCircle = this.sgWorld.Creator.CreateCircle(cPos, dCircleRadius, nLineColor, cFillColor, string.Empty, "Circle");
                
                double dNewCircleRadius = 300;
                double dCurrentCircleRadius = cCircle.Radius;
                cCircle.Radius = dNewCircleRadius;
                uint nRGB_Red = 0xFF0000;
                double dAlpha = 0.2;
                var cFillStyle = cCircle.FillStyle;
                cFillStyle.Color.FromRGBColor(nRGB_Red);
                cFillStyle.Color.SetAlpha(dAlpha);

                MsgTargetPosition eMsgTarget = MsgTargetPosition.MTP_POPUP;
                string tMessage = "Hello Circle";
                MsgType eMsgType = MsgType.TYPE_TEXT;
                bool bIsBringToFront = true;

                cMessage = this.sgWorld.Creator.CreateMessage(eMsgTarget, tMessage, eMsgType, bIsBringToFront);
                cCircle.Message.MessageID = cMessage.ID;

                var cFlyToPos = cPos.Copy();
                cFlyToPos.Pitch = -89.0;
                this.sgWorld.Navigate.FlyTo(cFlyToPos, ActionCode.AC_FLYTO);

            }
            catch (Exception ex)
            {
                msg = String.Format("CreateCircleButton_Click Exception: {0}", ex.Message);
                MessageBox.Show(msg);
            }
        }
        /// <summary>
        /// 创建Polyline
        /// </summary>
        private void CreatePolyline()
        {
            double[] cVerticesArray = null;
            cVerticesArray = new double[] { 114.35, 36.01659, 0, 116.35, 36.15498, 0, 115.35, 34.05090, 0 };
            // geometry creator can work on WKT, WKB or array of x,z,y coordinates
            var geometry = this.sgWorld.Creator.GeometryCreator.CreateLineStringGeometry(cVerticesArray);
            var color = this.sgWorld.Creator.CreateColor(255, 0, 0, 0);
            // 2 in AltitudeTypeCode means on terrain, 0 means add to root 
            AltitudeTypeCode eAltitudeTypeCode = AltitudeTypeCode.ATC_ON_TERRAIN;
            var line = this.sgWorld.Creator.CreatePolyline(geometry, color, eAltitudeTypeCode, string.Empty, "my poly on terrain");
            line.LineStyle.Width = 15000; // 15000m (15km)
            line.Position.Distance = 600000.0; // set max viewing distance in meters
            this.sgWorld.Navigate.FlyTo(line);
        }
        /// <summary>
        /// 创建Popups
        /// </summary>
        private void CreatePopupMessage()
        {
            var popup = this.sgWorld.Creator.CreatePopupMessage("My popup", "http://www.baidu.com");
            var width = popup.Width;
            var height = popup.Height;
            this.sgWorld.Window.ShowPopup(popup);
        }
        /// <summary>
        /// 创建文字标注
        /// </summary>
        private void CreateHierarchy()
        {
            try
            {
                // create group
                var newEngland = this.sgWorld.ProjectTree.CreateGroup("New England");
                var states = this.sgWorld.ProjectTree.CreateGroup("States", newEngland);
                // create 5 labels inside group
                var stateLabelStyle = this.sgWorld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
                stateLabelStyle.LineToGround = true;
                stateLabelStyle.TextColor.FromARGBColor((uint)Color.Beige.ToArgb());
                var Vermont = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-72.75206, 43.91127, 80000.0, 0, 0.0, 0, -85, 800000.0), "Vermont", stateLabelStyle, states, "Vermont");
                var Maine = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-69.40414, 45.12594, 80000.0, 0, 0.0, 0, -85, 800000.0), "Maine", stateLabelStyle, states, "Maine");
                var Massachusetts = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-71.88455, 42.34216, 80000.0, 0, 0.0, 0, -85, 800000.0), "Massachusetts", stateLabelStyle, states, "Massachusetts");
                var RhodeIsland = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-71.57073, 41.62953, 80000.0, 0, 0.0, 0, -85, 800000.0), "Rhode Island", stateLabelStyle, states, "Rhode Island");
                var Connecticut = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-72.64295, 41.57912, 80000.0, 0, 0.0, 0, -85, 800000.0), "Connecticut", stateLabelStyle, states, "Connecticut");
                // expand the group
                this.sgWorld.ProjectTree.ExpandGroup(newEngland, true);
                // fly to first label
                this.sgWorld.Navigate.FlyTo(Vermont, ActionCode.AC_FLYTO);
                MessageBox.Show("Created group and 5 labels in it. Click Ok to continue");
                var places = this.sgWorld.ProjectTree.CreateLockedGroup("Places", newEngland);
                var placeLabelStyle = this.sgWorld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
                placeLabelStyle.LineToGround = true;
                placeLabelStyle.TextColor.FromARGBColor((uint)Color.Cyan.ToArgb());
                var LakeChamplain = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-73.333333, 44.533333, 160000.0, 0, 0.0, 0, -85, 800000.0), "Lake Champlain", placeLabelStyle, places, "Lake Champlain");
                var Windsor = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-72.401111, 43.476667, 160000.0, 0, 0.0, 0, -85, 800000.0), "Windsor, Vermont", placeLabelStyle, places, "Windsor, Vermont");
                var NewHaven = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-72.923611, 41.31, 160000.0, 0, 0.0, 0, -85, 800000.0), "New Haven, Connecticut", placeLabelStyle, places, "New Haven, Connecticut");
                var Hartford = this.sgWorld.Creator.CreateTextLabel(this.sgWorld.Creator.CreatePosition(-72.677, 41.767, 160000.0, 0, 0.0, 0, -85, 800000.0), "Hartford, Connecticut", placeLabelStyle, places, "Hartford, Connecticut");
                MessageBox.Show("Created locked group 'Places' and 4 labels in it. You may use right click menu of the group to unlock it and edit its content");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message);
            }

        }
        /// <summary>
        /// 创建箭头
        /// </summary>
        private void CreateArrow()
        {
            var cPos = CreatePosition();

            double length = 1000;
            int style = 4;
            IColor66 lineColor = this.sgWorld.Creator.CreateColor(255, 0, 0, 0);
            IColor66 fillColor = this.sgWorld.Creator.CreateColor(255, 0, 0, 0);
            string groupID = string.Empty;
            string description = "箭头→";
            var arrow = this.sgWorld.Creator.CreateArrow(cPos, length, style, lineColor, fillColor, groupID, description);
            this.sgWorld.Navigate.FlyTo(arrow, ActionCode.AC_FLYTO);
        }
        /// <summary>
        /// 获取特效XML
        /// </summary>
        private string GetEffectXML()
        {
            var itemID = this.sgWorld.ProjectTree.FindItem(@"篝火1");
            var projectObject = this.sgWorld.ProjectTree.GetObject(itemID);
            ITerrainEffect66 effect = projectObject as ITerrainEffect66;
            string effectXML = effect.EffectXML;
            return effectXML;
        }

        private int CloudState = 0;//云层状态，默认为关闭
        private void TraverseLayerTree()
        {
            //this.sgWorld.Command.GetValue(1154);
            if (CloudState == 0)
            {
                this.sgWorld.Command.Execute(1154, 1);
                CloudState = 1;
            }
            else
            {
                this.sgWorld.Command.Execute(1154, 0);
                CloudState = 0;
            }
        }
        /// <summary>
        /// 创建火焰特效(但是无法设置火焰初始大小)
        /// </summary>
        private void CreateFireEffect()
        {
            if(string.IsNullOrEmpty(this.effectXML))
            {
                MessageBox.Show("工程树操作→获取特效XML");
                return;
            }
            var cPos = CreatePosition();

            string groupID = string.Empty;
            string description = "火焰1";
            var fire = this.sgWorld.Creator.CreateEffect(cPos, this.effectXML, groupID, description);
            //添加此段代码能达到控制比例效果，但是由于图片有白底，导致火焰外围存在白色
            //var newXML = effectXML.Replace("CampFire.png", "C:\\Program Files (x86)\\Skyline\\TerraExplorer Pro\\Tools\\ParticleEditor\\ParticleImages\\CampFire.png");

            //var pos = this.sgWorld.Creator.CreatePosition(fire.Position.X + 0.001, fire.Position.Y, fire.Position.Altitude);
            //var cLabelStyle = this.sgWorld.Creator.CreateLabelStyle();

            //var cTextLabel = this.sgWorld.Creator.CreateTextLabel(pos, newXML, cLabelStyle, "", "");
            //cTextLabel.Style.BackgroundColor.SetAlpha(1);
            //cTextLabel.Style.Scale = 1000;
            this.sgWorld.Navigate.FlyTo(fire, ActionCode.AC_FLYTO);
        }
        /// <summary>
        /// 创建位置
        /// </summary>
        /// <returns></returns>
        private IPosition66 CreatePosition()
        {
            double dXCoord = 116.35;
            double dYCoord = 27.98;
            double dAltitude = 0;
            AltitudeTypeCode eAltitudeTypeCode = AltitudeTypeCode.ATC_TERRAIN_RELATIVE;
            double dYaw = 0.0;
            double dPitch = 0.0;
            double dRoll = 0.0;
            double dDistance = 500;
            var cPos = this.sgWorld.Creator.CreatePosition(dXCoord, dYCoord, dAltitude, eAltitudeTypeCode, dYaw, dPitch, dRoll, dDistance);
            return cPos;
        }
        /// <summary>
        /// 路径漫游
        /// </summary>
        private void RouteRoam()
        {
            //绘制路径
            double[] cVerticesArray = null;
            cVerticesArray = new double[] {
                        116.35,  27.98,  0,
                        116.45,  28.98,  0,
                        116.45,  28.11,  0,
                        116.65,  28.45,  0,
                     };

            ILineString pILineString = sgWorld.Creator.GeometryCreator.CreateLineStringGeometry(cVerticesArray);
            IColor66 color = sgWorld.Creator.CreateColor(255, 0, 0, 125);
            var polyline = sgWorld.Creator.CreatePolyline(pILineString, color);
            var dynamicObject = this.sgWorld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_GROUND_VEHICLE, DynamicObjectType.DYNAMIC_IMAGE_LABEL, @"F:\项目管理\智慧抚州\使用的Fly\data11\汽车图标\整车.png", 50, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, "", "动态对象");
            var wayPoint1 = this.sgWorld.Creator.CreateRouteWaypoint(116.35, 27.98, 0, 2000);
            var wayPoint2 = this.sgWorld.Creator.CreateRouteWaypoint(116.45, 28.98, 0, 2000);
            var wayPoint3 = this.sgWorld.Creator.CreateRouteWaypoint(116.55, 28.11, 0, 800);
            var wayPoint4 = this.sgWorld.Creator.CreateRouteWaypoint(116.65, 28.45, 0, 800);
            dynamicObject.Waypoints.AddWaypoint(wayPoint1);
            dynamicObject.Waypoints.AddWaypoint(wayPoint2);
            dynamicObject.Waypoints.AddWaypoint(wayPoint3);
            dynamicObject.Waypoints.AddWaypoint(wayPoint4);
            dynamicObject.CircularRoute = false;
            dynamicObject.RestartRoute(0);
            //dynamicObject.MoveByTime = true;
            sgWorld.Navigate.FlyTo(dynamicObject.ID, ActionCode.AC_JUMP);
        }

        #endregion

        private void 遍历ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TraverseLayerTree();
        }
    }
}
