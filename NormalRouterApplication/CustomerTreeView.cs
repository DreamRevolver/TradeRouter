using System.Drawing;
using System.Windows.Forms;
using Shared.Core;
using Shared.Interfaces;
using Shared.Models;

namespace NormalRouterApplication
{
    public class CustomerTreeView : TreeView
    {

    }
    public class CustomerTreeNode : TreeNode
    {
        private IEndpoint Client { get; set; }
        private IDataStorage Settings { get; set; }
        private CustomerRole Role { get; set; }
        private string ClientName { get; set; }
        private void SetColor(ConnectionEvent @event)
        {
            switch (@event)
            {
                case ConnectionEvent.Started:
                    BackColor = Color.LightGray;
                    break;
                case ConnectionEvent.Stopped:
                    BackColor = Color.White;
                    break;
                case ConnectionEvent.Logon:
                    BackColor = Color.Green;
                    break;
                case ConnectionEvent.Logout:
                    BackColor = Color.Red;
                    break;
                default:
                    BackColor = Color.White;
                    break;
            }
        }

        public CustomerTreeNode(IEndpoint client, string clientName)
        {
            Client = client;
            ClientName = clientName;
            Role = (Client as BackEndClient) is null ? CustomerRole.Publisher : CustomerRole.Subscriber;
            Settings = Role == CustomerRole.Subscriber ? (Client as BackEndClient)?.Settings : (Client as FrontEndClient)?.Settings;
            if (Settings != null)
            {
                foreach (var key in Settings.AvailableKeys)
                {
                    Nodes.Add(new TreeNode {Name = key, Text = Settings.Get(key)});
                }
            }
            if (Role == CustomerRole.Subscriber)
            {
                (Client as BackEndClient).ConnectionEvent += SetColor;
            }
            else
            {
                (Client as FrontEndClient).ConnectionEvent += SetColor;
            }
        }
    }
}
