using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using BuildSync.Core.Controls.Graph;
using BuildSync.Core.Networking;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class StatisticsForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Statistic> Stats = new List<Statistic>();

        /// <summary>
        /// 
        /// </summary>
        private int DownloadSpeedIndex = 0;
        private int UploadSpeedIndex = 0;
        private int DiskWriteSpeedIndex = 0;
        private int DiskReadSpeedIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Max"></param>
        public int AddStatistic(Statistic stat)
        {
            string[] PathSplit = stat.Name.Split('\\');
            string Name = PathSplit[PathSplit.Length - 1];

            stat.Series.Name = Name;
            stat.Series.SlidingWindow = true;
            stat.Series.XAxis.MinLabel = "";
            stat.Series.XAxis.MaxLabel = "5 Minutes";
            stat.Series.XAxis.Min = 0;
            stat.Series.XAxis.Max = 5 * 60;
            stat.Series.XAxis.GridInterval = 30;
            stat.Series.YAxis.MinLabel = "";
            stat.Series.YAxis.MaxLabel = stat.MaxLabel;
            stat.Series.YAxis.Max = stat.MaxValue;
            stat.Series.YAxis.GridInterval = stat.MaxValue / 10;

            Stats.Add(stat);

            TreeNode node = AddNodeByPath(StatsTreeView.Nodes, stat.Name, stat);
            node.Checked = stat.DefaultShown;

            return Stats.Count - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nodes"></param>
        /// <param name="Path"></param>
        private TreeNode AddNodeByPath(TreeNodeCollection Nodes, string Path, object Tag)
        {
            string[] split = Path.Split('\\');
            if (split.Length == 1)
            {
                TreeNode NewNode = new TreeNode();
                NewNode.Text = Path;
                NewNode.Tag = Tag;
                NewNode.Name = Path;
                Nodes.Add(NewNode);

                return NewNode;
            }
            else
            {
                TreeNode[] Parent = Nodes.Find(split[0], false);
                TreeNodeCollection NextCollection = null;
                if (Parent.Length == 0)
                {
                    TreeNode NewNode = new TreeNode();
                    NewNode.Text = split[0];
                    NewNode.Tag = "";
                    NewNode.Name = split[0];
                    Nodes.Add(NewNode);

                    NextCollection = NewNode.Nodes;
                }
                else
                {
                    NextCollection = Parent[0].Nodes;
                }

                string NewPath = "";
                for (int i = 1; i < split.Length; i++)
                {
                    if (i != 1)
                    {
                        NewPath += "\\";
                    }
                    NewPath += split[i];
                }

                return AddNodeByPath(NextCollection, NewPath, Tag);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="CheckedNodes"></param>
        private void GetCheckedStats(TreeNodeCollection Nodes, List<Statistic> CheckedNodes)
        {
            foreach (TreeNode Child in Nodes)
            {
                if (Child.Checked)
                {
                    Statistic Stat = Child.Tag as Statistic;
                    if (Stat != null)
                    {
                        CheckedNodes.Add(Stat);
                    }
                }

                GetCheckedStats(Child.Nodes, CheckedNodes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Value"></param>
        public void AddDataPoint(int Index, float Value)
        {
            Stats[Index].Series.AddDataPoint(Environment.TickCount / 1000.0f, Value);
        }

        /// <summary>
        /// 
        /// </summary>
        public StatisticsForm()
        {
            InitializeComponent();

            Statistic.Instantiate();

            lock (Statistic.Instances)
            {
                foreach (var pair in Statistic.Instances)
                {
                    AddStatistic(pair.Value);
                }
            }

            foreach (TreeNode node in StatsTreeView.Nodes)
            {
                node.ExpandAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateVisibleSeries()
        {
            List<Statistic> VisibleStats = new List<Statistic>();
            GetCheckedStats(StatsTreeView.Nodes, VisibleStats);

            // Add non-visible statistics.
            foreach (Statistic stat in VisibleStats)
            {
                bool Exists = false;
                foreach (GraphControl control in StatPanel.Controls)
                {
                    if (control.Tag == stat)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    GraphControl Control = new GraphControl();
                    Control.Series = new GraphSeries[1] { stat.Series };
                    Control.Dock = DockStyle.Top;
                    Control.Size = new Size(100, 110);
                    Control.Tag = stat;
                    StatPanel.Controls.Add(Control);
                }
            }

            // Remove none visible statistics.
            foreach (GraphControl control in StatPanel.Controls)
            {
                bool Exists = false;
                foreach (Statistic stat in VisibleStats)
                {
                    if (control.Tag == stat)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    StatPanel.Controls.Remove(control);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetSamples(object sender, EventArgs e)
        {
            lock (Statistic.Instances)
            {
                foreach (var pair in Statistic.Instances)
                {
                    pair.Value.Gather();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, EventArgs e)
        {
            UpdateVisibleSeries();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticCheckChanged(object sender, TreeViewEventArgs e)
        {
            UpdateVisibleSeries();
        }
    }
}
