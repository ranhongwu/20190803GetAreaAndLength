using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace _20190803GetAreaAndLength
{
    public partial class FrmAreaLength : DevExpress.XtraEditors.XtraForm
    {
        public FrmAreaLength(IMap pmap)
        {
            InitializeComponent();

            pMap = pmap;
            DT.Columns.Add("要素名称", typeof(String));
            DT.Columns.Add("要素类型", typeof(String));
            DT.Columns.Add("测量结果", typeof(Double));
            DT.Columns.Add("单位", typeof(String));
        }

        IMap pMap;
        DataTable DT = new DataTable();
        string LengthUnit = "米";
        string AreaUnit = "平方米";

        private void InitTable()
        {
            DT.Clear();
        }

        private void FrmAreaLength_Load(object sender, EventArgs e)
        {
            InitTable();
            gridControl1.DataSource = DT;
            AppendToTable();

            //调整gridView的显示，使其与窗体适应
            gridView1.Columns[0].BestFit();
            gridView1.Columns[1].BestFit();
            gridView1.Columns[2].BestFit();
            gridView1.Columns[3].BestFit();
        }
        
        //遍历各图层，将数据加载到表中
        private void AppendToTable()
        {
            List<IFeatureLayer> FeatureLayerList = new List<IFeatureLayer>();
            List<string> RowInfo = new List<string>();
            FeatureLayerList = getAllShp(pMap);
            foreach(IFeatureLayer pFeatureLayer in FeatureLayerList)
            {
                RowInfo = getRowInfo(pFeatureLayer);
                DT.Rows.Add(RowInfo[0], RowInfo[1], RowInfo[2], RowInfo[3]);
            }
        }

        //返回Map中所有的ShapeFile图层
        private List<IFeatureLayer> getAllShp(IMap map)
        {
            IFeatureLayer pFeatureLayer;
            List<IFeatureLayer> shpLayreList = new List<IFeatureLayer>();
            for(int i = 0; i < map.LayerCount; i++)
            {

                if (map.Layer[i] is GroupLayer)
                {
                    ICompositeLayer pComLayer = map.Layer[i] as ICompositeLayer;
                    for (int j = 0; j < pComLayer.Count; j++)
                    {
                        pFeatureLayer = pComLayer.Layer[j] as IFeatureLayer;
                        if (pFeatureLayer.DataSourceType.Contains("Shapefile"))
                            shpLayreList.Add(pFeatureLayer);
                    }
                }
                else if (map.Layer[i] is IFeatureLayer)
                {
                    pFeatureLayer = map.Layer[i] as IFeatureLayer;
                    if (pFeatureLayer.DataSourceType.Contains("Shapefile"))
                        shpLayreList.Add(pFeatureLayer);
                }
                else
                    continue;
            }
            return shpLayreList;
        }

        //生成每一行数据
        private List<string> getRowInfo(IFeatureLayer pFeatureLayer)
        {
            List<string> s = new List<string>();
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            switch (pFeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    s.Add(pFeatureClass.AliasName);
                    s.Add("点");
                    s.Add(getPointCount(pFeatureClass));
                    s.Add("个");
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    s.Add(pFeatureClass.AliasName);
                    s.Add("线");
                    s.Add(getLineLength(pFeatureClass));
                    s.Add(LengthUnit);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    s.Add(pFeatureClass.AliasName);
                    s.Add("面");
                    s.Add(getPolygonArea(pFeatureClass));
                    s.Add(AreaUnit);
                    break;
            }
            return s;
        }

        //返回要素点的个数
        private string getPointCount(IFeatureClass pFeatureClass)
        {
            return pFeatureClass.FeatureCount(null).ToString();
        }

        //返回线要素的总长度
        private string getLineLength(IFeatureClass pFeatureClass)
        {
            double Length = 0;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            IFeature pFeature;
            IPolyline pPolyline;
            while ((pFeature = pFeatureCursor.NextFeature()) != null)
            {
                pPolyline = pFeature.Shape as IPolyline;
                Length += pPolyline.Length;
            }
            switch (LengthUnit)
            {
                case "米":
                    break;
                case "千米":
                    Length = Length / 1000;
                    break;
                default:
                    break;
            }
            return Math.Round(Length, 3).ToString();
        }

        //返回面要素的总面积
        private string getPolygonArea(IFeatureClass pFeatureClass)
        {
            double Area = 0;
            IArea pArea;
            IFeature pFeature;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            while ((pFeature = pFeatureCursor.NextFeature()) != null)
            {
                pArea = pFeature.Shape as IArea;
                Area += pArea.Area;
            }
            switch (AreaUnit)
            {
                case "平方米":
                    break;
                case "平方千米":
                    Area = Area / 1000000;
                    break;
                case "亩":
                    Area = Area / 666.6666667;
                    break;
                case "公顷":
                    Area = Area / 66666.6666667;
                    break;
                default:
                    break;
            }
            return Math.Round(Area, 3).ToString();
        }

        //改变长度单位
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LengthUnit = comboBoxEdit1.Text;
            FrmAreaLength_Load(sender, e);
        }

        //改变面积单位
        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AreaUnit = comboBoxEdit2.Text;
            FrmAreaLength_Load(sender, e);
        }

        //刷新
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            FrmAreaLength_Load(sender, e);
        }
    }
}