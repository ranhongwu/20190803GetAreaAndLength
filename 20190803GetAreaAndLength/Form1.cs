using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Carto;

namespace _20190803GetAreaAndLength
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAddData_Click(object sender, EventArgs e)
        {
            string path;
            List<string> fullNameList = new List<string>();
            List<string> nameList = new List<string>();
            int index;
            

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "打开shp文件";
            openFileDialog.Filter = "shp文件(*.shp)|*.shp";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                for(int i=0;i<openFileDialog.FileNames.Count();i++)
                {
                    fullNameList.Add(openFileDialog.FileNames[i]);
                }
            }
            string fullname = fullNameList[0];
            index = fullname.LastIndexOf("\\");
            path = fullname.Substring(0, index);
            foreach(string eachName in fullNameList)
            {
                nameList.Add(eachName.Substring(index+1));
            }
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
            IFeatureWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(path,0) as IFeatureWorkspace;
            foreach(string name in nameList)
            {
                IFeatureClass pFeatureClass = pWorkspace.OpenFeatureClass(name);
                IFeatureLayer pFeatureLayer = new FeatureLayer();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pFeatureClass.AliasName;
                axMapControl1.AddLayer(pFeatureLayer as ILayer);
            }
            axMapControl1.Refresh();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            FrmAreaLength frmAreaLength = new FrmAreaLength(axMapControl1.Map);
            frmAreaLength.Show();
        }
    }
}
