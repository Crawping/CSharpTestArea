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
                if (s.Name == "SiSS")
                {
                    foreach( var a in s.Applications)
                    {
                        Console.WriteLine(a.ToString());
                        Console.WriteLine(a.Schema.Name);
                        Console.WriteLine(a.Path);
                        Console.WriteLine(a.VirtualDirectories["/"].PhysicalPath);
                        Console.WriteLine(a.ApplicationPoolName);
                        Console.WriteLine(a.EnabledProtocols);

                        Console.WriteLine("----------");
                    }
                }
            }

        }
    }
}
