using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.Administration;
using System.DirectoryServices;
using Microsoft.Win32;

namespace IISAppPools
{
    public partial class frmMain : Form
    {        
        private void InitializeCheck()
        {
            IISJudgment iisJudgment = new IISJudgment();
            if (!iisJudgment.IsWebServer())
            {
                if (MessageBox.Show(this, "IIS服务未安装。\n请安装IIS服务之后继续。", "IIS未安装", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }

            if (!iisJudgment.IsPassWebServer())
            {
                if (MessageBox.Show(this, "IIS依赖组件未安装。\n请安装“静态内容”和“ASP.NET”之后再继续。", "IIS组件未安装", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
        }

        private List<string> GetAppNames(string siteName = "SiSS")
        {
            List<string> appPoolNames = new List<string> { };

            ServerManager serverManager = new ServerManager();

            foreach (var s in serverManager.Sites)
            {
                if (s.Name.Contains(siteName))
                {
                    foreach (var a in s.Applications)
                    {
                        string appName = a.ToString().Split('/')[1];
                        if (appName != null & appName != "")
                            appPoolNames.Add(appName);
                    }
                }
            }
            return appPoolNames;
        }

        private string GetAppPoolName(string appName, string siteName = "SiSS")
        {
            ServerManager serverManager = new ServerManager();
            foreach (var a in serverManager.Sites[siteName].Applications)
            {
                string l_appName = a.ToString().Split('/')[1];
                if (l_appName != null & l_appName != "")
                {
                    if (l_appName == appName)
                    {
                        return a.ApplicationPoolName;
                    }                    
                }
            }
            return "";
        }

        private string GetAppPhysicalPath(string appName, string siteName = "SiSS")
        {
            ServerManager serverManager = new ServerManager();
            foreach (var a in serverManager.Sites[siteName].Applications)
            {
                string l_appName = a.ToString().Split('/')[1];
                if (l_appName != null & l_appName != "")
                {
                    if (l_appName == appName)
                    {
                        return a.VirtualDirectories["/"].PhysicalPath;
                    }
                }
            }
            return "";
        }

        private string GetAppPoolBit(string appPoolName)
        {
            DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            List<string> bitList = new List<string> { "64", "32" };
            
            foreach (DirectoryEntry appPool in appPools.Children)
            {            
                if (appPool.Name == appPoolName)
                {
                    return bitList[Convert.ToUInt16(appPool.InvokeGet("enable32BitAppOnWin64"))];
                }
            }
            return "32";
        }

        private string GetAppCustomerName(string appName)
        {
            RegistryKey hklmKey = Registry.LocalMachine;
            

            return "";
        }

        private List<string> GetRegistryKey(string appName)
        {
            List<string> regKeyList = new List<string> {"{E72B1992-9382-423D-BBE3-11CF601E5920}",       //ESHOP5
                                                        "{3E61FE1F-4DD5-426A-A50F-E81AE25D825A}",       //EWEIGHT
                                                        "{721353BC-0D50-41BB-BE99-5CBA73E7DD04}",       //EBEAUTY
                                                        "{19AB8FB4-B406-4296-A480-FEDE0FDBBE8A}",       //ESHOPFS5
                                                        "{6AC87E0E-C4CF-487B-B75C-B94D0B5F5168}",       //EYBABY6
                                                        "{C08335B5-7F38-4DE7-A9FF-544EFACCAAF7}",       //ESHOP4
                                                        "{C08335B5-7F38-4DE7-A9FF-544EFACCAAF7}"        //ESHOPFS4
                                                        };    
            RegistryKey hklmKey = Registry.LocalMachine;
            RegistryKey uninstallKey = hklmKey.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\");
            string[] uninstallKeyNames = uninstallKey.GetSubKeyNames();

            foreach (string keyName in regKeyList)
            {
                string appKeyName = keyName + ".{" + appName + "}";
                List<string> productInfo = new List<string> { };
               // Console.WriteLine(appKeyName);
                foreach (string uninstallKeyName in uninstallKeyNames)
                {
                    if (uninstallKeyName == appKeyName)
                    {
                        RegistryKey productKey = uninstallKey.OpenSubKey(uninstallKeyName);
                        string productName = productKey.GetValue("ProductName").ToString();
                        string customerName = productKey.GetValue("CustomerName").ToString();
                        productInfo.Add(productName);
                        productInfo.Add(customerName);

                        return productInfo;

                    }
            }

            }

            return new List<string> { "", "" };
        }

        private Dictionary<string, Dictionary<string, string>> GetAppInfos()
        {
            Dictionary<string, Dictionary<string, string>> appInfos = new Dictionary<string, Dictionary<string, string>> { };

            foreach (string siteName in GetAppNames())
            {
                Dictionary<string, string> appInfo = new Dictionary<string, string> { };
                string appProductName = GetRegistryKey(siteName)[0];
                string appCustomerName = GetRegistryKey(siteName)[1];

                appInfo.Add("产品名称", appProductName);
                appInfo.Add("库名称", appCustomerName);
                appInfo.Add("应用程序池", GetAppPoolName(siteName));                
                appInfo.Add("应用程序池位数", GetAppPoolBit(appInfo["应用程序池"]));
                appInfo.Add("站点路径", GetAppPhysicalPath(siteName));



                appInfos.Add(siteName, appInfo);
            };

            return appInfos;
        }

        public void FillControls()
        {
            Dictionary<string, Dictionary<string, string>> appInfos = GetAppInfos();

            foreach (string key in appInfos.Keys)
            {
                Console.WriteLine(key);
                int rowCount = 0;


                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();                
                tableLayoutPanel.Name = key + "tableLayoutPannel";
                tableLayoutPanel.ColumnCount = 2;
                tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
                tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
                tableLayoutPanel.RowCount = 5;
                tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
                tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
                tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
                tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
                tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
                tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;

                GroupBox groupBox = new GroupBox();                
                groupBox.Name = key;
                groupBox.Text = key;
                
                groupBox.Size = new System.Drawing.Size(320, 150);
                groupBox.Click += GroupBox_DoubleClick;


                groupBox.Controls.Add(tableLayoutPanel);
                flpaMain.Controls.Add(groupBox);

                foreach (KeyValuePair<string, string> kv in appInfos[key])
                {
                    int colCount = 0;

                    Label label = new Label();
                    label.AutoSize = true;
                    label.Name = "label" + Convert.ToString(rowCount);
                    label.Text = kv.Key;
                    tableLayoutPanel.Controls.Add(label, colCount, rowCount);
                    colCount++;
                    Label label2 = new Label();
                    label2.AutoSize = true;
                    label2.Name = "label" + Convert.ToString(rowCount);
                    label2.Text = kv.Value;
                    tableLayoutPanel.Controls.Add(label2, colCount, rowCount);

                    rowCount++;
                } 
            }

            
        }

        private void GroupBox_DoubleClick(object sender, EventArgs e)
        {
            GroupBox groupBox = (GroupBox)sender;
            string siteName = groupBox.Name;
            string sitePath = GetAppPhysicalPath(siteName);
            Form changeFrm = new frmChangeConfig( siteName, sitePath);
            changeFrm.ShowDialog();
        }



        private void tmp()
        {
            DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");

            foreach (DirectoryEntry appPool in appPools.Children)
            {
                Console.WriteLine(appPool.Name);
                Console.WriteLine(appPool.InvokeGet("ManagedRuntimeVersion"));
                Console.WriteLine(appPool.InvokeGet("AppPoolCommand"));
                Console.WriteLine(appPool.InvokeGet("ManagedPipelineMode"));
                Console.WriteLine(appPool.InvokeGet("AppPoolIdentityType"));
                Console.WriteLine(appPool.InvokeGet("enable32BitAppOnWin64"));

                Console.WriteLine("*****************");
            }
            

            ServerManager serverManger = new ServerManager();
            
                        foreach (var s in serverManger.Sites)
                        {
                            if (s.Name.Contains("SiSS"))
                            {                    
                                foreach( var a in s.Applications)
                                {
                                    Console.WriteLine(a.ToString());
                                    Console.WriteLine(a.Schema.Name);
                                    Console.WriteLine(a.Path);
                                    Console.WriteLine(a.VirtualDirectories["/"].PhysicalPath); //应用程序物理路径
                                    Console.WriteLine(a.ApplicationPoolName);   //应用池名称
                                    Console.WriteLine(a.EnabledProtocols);                        
                                    Console.WriteLine("----------");
                                }
                            }
                        }
            
            List<string> strList = new List<string> { };

            strList = GetAppNames();

            foreach(string s in strList)
            {
                Console.WriteLine($"站点名称:{s}");
                Console.WriteLine($"应用池名称：{GetAppPoolName(s)}");
                Console.WriteLine($"应用程序物理路径：{GetAppPhysicalPath(s)}");
                Console.WriteLine($"应用程序位数：{GetAppPoolBit(GetAppPoolName(s))}");
                Console.WriteLine($"应用程序产品名：{GetRegistryKey(s)[0]}");
                Console.WriteLine($"应用程序库名：{GetRegistryKey(s)[1]}");

                Console.WriteLine("**************");

            }        
            

            Dictionary<string, Dictionary<string, string>> tmp = GetAppInfos();

            foreach(string key in tmp.Keys)
            {
                Console.WriteLine(key);
                foreach (KeyValuePair<string, string> kv in tmp[key])
                {
                    Console.WriteLine("{0} \t {1}", kv.Key, kv.Value);
                }
            }
            

            FillControls();
        }
    }
}
