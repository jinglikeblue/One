using One;
using StressTesting.Sources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StressTesting
{
    public partial class MainPanel : Form
    {
        public MainPanel()
        {
            InitializeComponent();
        }

        List<StressTestingThread> _list = new List<StressTestingThread>();

        private void MainPanel_Load(object sender, EventArgs e)
        {

        }        

        private void btnStartText_Click(object sender, EventArgs e)
        {
            StressTestingVO vo = new StressTestingVO();
            vo.host = textHost.Text.Trim();
            vo.port = int.Parse(textPort.Text.Trim());
            vo.threadCount = int.Parse(textThread.Text.Trim());
            vo.connectionCountPerThread = int.Parse(textConnectionCount.Text.Trim());
            string msg = textMsg.Text.Trim();
            ByteArray ba = new ByteArray();
            ba.Write(msg);
            vo.msgData = ba.GetAvailableBytes();

            for(int i = 0; i < vo.threadCount; i++)
            {
                var stt = new StressTestingThread(vo);
                stt.Start();
                _list.Add(stt);
            }
        }
    }
}
