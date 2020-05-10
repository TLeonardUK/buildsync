/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/


using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using BuildSync.Core.Utils;
using BuildSync.Core.Manifests;
using BuildSync.Core.Users;
using BuildSync.Core.Tags;
using BuildSync.Core.Routes;
using WeifenLuo.WinFormsUI.Docking;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using BuildSync.Client.Properties;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ManageRoutesForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public class RouteTreeNode : Node
        {
            public Image Icon;
            public string Source;
            public string Destination;
            public string Blacklisted;
            public string BandwidthLimit;

            public Route Route;
        }

        /// <summary>
        /// 
        /// </summary>
        private TreeModel Model = null;

        /// <summary>
        /// 
        /// </summary>
        public ManageRoutesForm()
        {
            InitializeComponent();

            Program.NetClient.OnRouteListRecieved += RoutesRecieved;
                        
            Model = new TreeModel();
            MainTreeView.Model = Model;

            TreeColumn SourceColumn = new TreeColumn();
            SourceColumn.Header = "Source";
            SourceColumn.Width = 200;
            MainTreeView.Columns.Add(SourceColumn);
            
                ScaledNodeIcon IconControl = new ScaledNodeIcon();
                IconControl.ParentColumn = SourceColumn;
                IconControl.DataPropertyName = "Icon";
                IconControl.FixedSize = new Size((int)(MainTreeView.RowHeight * 1.25f), (int)(MainTreeView.RowHeight * 1.25f));
                IconControl.Offset = new Size(0, 5);
                MainTreeView.NodeControls.Add(IconControl);

                NodeTextBox TextControl = new NodeTextBox();
                TextControl.ParentColumn = SourceColumn;
                TextControl.DataPropertyName = "Source";
                MainTreeView.NodeControls.Add(TextControl);

            TreeColumn DestinationColumn = new TreeColumn();
            DestinationColumn.Header = "Destination";
            DestinationColumn.Width = 200;
            MainTreeView.Columns.Add(DestinationColumn);

                NodeTextBox DestinationTextControl = new NodeTextBox();
                DestinationTextControl.ParentColumn = DestinationColumn;
                DestinationTextControl.DataPropertyName = "Destination";
                MainTreeView.NodeControls.Add(DestinationTextControl);

            TreeColumn BlacklistedColumn = new TreeColumn();
            BlacklistedColumn.Header = "Blacklisted";
            BlacklistedColumn.Width = 200;
            MainTreeView.Columns.Add(BlacklistedColumn);

                NodeTextBox BlacklistedTextControl = new NodeTextBox();
                BlacklistedTextControl.ParentColumn = BlacklistedColumn;
                BlacklistedTextControl.DataPropertyName = "Blacklisted";
                MainTreeView.NodeControls.Add(BlacklistedTextControl);

            TreeColumn BandwidthColumn = new TreeColumn();
            BandwidthColumn.Header = "Bandwidth Limit";
            BandwidthColumn.Width = 200;
            MainTreeView.Columns.Add(BandwidthColumn);

                NodeTextBox BandwidthTextControl = new NodeTextBox();
                BandwidthTextControl.ParentColumn = BandwidthColumn;
                BandwidthTextControl.DataPropertyName = "BandwidthLimit";
                MainTreeView.NodeControls.Add(BandwidthTextControl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            Program.NetClient.RequestRouteList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTagList(object sender, EventArgs e)
        {
            Program.NetClient.RequestRouteList();
        }

        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void RoutesRecieved(List<Route> InRoutes)
        {
            // Add new tags.
            foreach (Route Route in InRoutes)
            {
                RouteTreeNode ExistingRoute = null;

                foreach (RouteTreeNode Node in Model.Nodes)
                {
                    if (Node.Route.Id == Route.Id)
                    {
                        ExistingRoute = Node;
                        break;
                    }
                }

                if (ExistingRoute == null)
                {
                    ExistingRoute = new RouteTreeNode();
                    ExistingRoute.Route = Route;
                    Model.Nodes.Add(ExistingRoute);
                }

                ExistingRoute.Source = Program.TagRegistry.IdToString(Route.SourceTagId);
                ExistingRoute.Destination = Program.TagRegistry.IdToString(Route.DestinationTagId);
                ExistingRoute.Blacklisted = Route.Blacklisted ? "Yes" : "No";
                ExistingRoute.BandwidthLimit = Route.BandwidthLimit == 0 ? "Unlimited" : StringUtils.FormatAsTransferRate(Route.BandwidthLimit);
                ExistingRoute.Icon = Resources.appbar_arrow_up_down;
            }

            // Remove old tags.
            List<RouteTreeNode> RemovedNodes = new List<RouteTreeNode>();
            foreach (RouteTreeNode Node in Model.Nodes)
            {
                bool Found = false;

                foreach (Route Route in InRoutes)
                {
                    if (Node.Route.Id == Route.Id)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    RemovedNodes.Add(Node);
                }
            }

            foreach (RouteTreeNode Node in RemovedNodes)
            {
                Model.Nodes.Remove(Node);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedChanged(object sender, EventArgs e)
        {
            ValidateState();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateState()
        {
            RouteTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as RouteTreeNode;

            deleteToolStripMenuItem.Enabled = (Node != null);
            addTagToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClicked(object sender, EventArgs e)
        {
            RouteTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as RouteTreeNode;
            if (Node == null)
            {
                return;
            }

            Program.NetClient.DeleteRoute(Node.Route.Id);
            Program.NetClient.RequestRouteList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClicked(object sender, EventArgs e)
        {
            AddRouteForm form = new AddRouteForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Program.NetClient.CreateRoute(form.SourceTagId, form.DestinationTagId, form.Blacklisted, form.BandwidthLimit);
                Program.NetClient.RequestRouteList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditClicked(object sender, EventArgs e)
        {
            RouteTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as RouteTreeNode;
            if (Node == null)
            {
                return;
            }

            AddRouteForm form = new AddRouteForm();
            form.SourceTagId = Node.Route.SourceTagId;
            form.DestinationTagId = Node.Route.DestinationTagId;
            form.Blacklisted = Node.Route.Blacklisted;
            form.BandwidthLimit = Node.Route.BandwidthLimit;
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Node.Route.SourceTagId = form.SourceTagId;
                Node.Route.DestinationTagId = form.DestinationTagId;
                Node.Route.Blacklisted = form.Blacklisted;
                Node.Route.BandwidthLimit = form.BandwidthLimit;

                Node.Source = Program.TagRegistry.IdToString(form.SourceTagId);
                Node.Destination = Program.TagRegistry.IdToString(form.DestinationTagId);
                Node.Blacklisted = form.Blacklisted ? "Yes" : "No";
                Node.BandwidthLimit = form.BandwidthLimit == 0 ? "Unlimited" : StringUtils.FormatAsTransferRate(form.BandwidthLimit);

                Program.NetClient.UpdateRoute(Node.Route.Id, form.SourceTagId, form.DestinationTagId, form.Blacklisted, form.BandwidthLimit);
                Program.NetClient.RequestRouteList();
            }
        }
    }
}
