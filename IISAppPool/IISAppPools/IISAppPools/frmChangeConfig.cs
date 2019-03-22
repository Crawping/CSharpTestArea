using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace IISAppPools
{
    public partial class frmChangeConfig : Form
    {
        public frmChangeConfig()
        {
            InitializeComponent();
        }
    
        public frmChangeConfig(string frmCaption, string sitePath)
        {
            InitializeComponent();
            this.Text = this.frmCaption = frmCaption;
            this.sitePath = sitePath;
            this.webConfigPath = this.sitePath + "\\web.config";

            FillControl();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeConfig(LoadConfigDocument(this.webConfigPath));
            }
            catch (Exception ex)
            {
                this.Close();
            }

            MessageBox.Show("修改成功");
            this.Close();
        }

        private void FillControl()
        {            
            ShowCurrentDataSource(LoadConfigDocument(webConfigPath));
        }

        private void ShowCurrentDataSource(ConfigXmlDocument doc)
        {
            XmlNode xmlNode = doc.SelectSingleNode("//connectionStrings");
            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                int num = (int)MessageBox.Show("配置文件中不存在有效的 connectionStrings 节点或节点下没有有效的配置信息");
            }
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    this.rtbConnectString.Text = (Utility.DeBase64(((XmlElement)childNode).GetAttribute("connectionString")));  
                }
            }
        }

        private bool ChangeConfig(ConfigXmlDocument doc)
        {
            foreach (XmlNode childNode in doc.SelectSingleNode("//connectionStrings").ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    XmlElement xmlElement = (XmlElement)childNode;
                    string str = Utility.EnBase64(this.rtbConnectString.Text);
                    xmlElement.SetAttribute("connectionString", str);
                }
            }
            try
            {
                doc.Save(this.webConfigPath);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(string.Format("更改配置失败：{0}", (object)ex));
                return false;
            }
            return true;
        }
        private ConfigXmlDocument LoadConfigDocument(string configPath)
        {
            try
            {
                ConfigXmlDocument configXmlDocument = new ConfigXmlDocument();
                configXmlDocument.Load(configPath);
                return configXmlDocument;
            }
            catch (Exception ex)
            {
                throw new Exception("目录下的配置文件有异常，请检查！");
            }
        }

        private string frmCaption;
        private string sitePath;
        private string webConfigPath;
    }
}
