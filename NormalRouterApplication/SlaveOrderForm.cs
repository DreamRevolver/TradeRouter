using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Shared.Core;
using Shared.Interfaces;
using Shared.Models;
using SharedBinance.Models;
using WeifenLuo.WinFormsUI.Docking;
namespace NormalRouterApplication
{
    public partial class SlaveOrderForm : DockContent
    {
        public enum MsgColumnType
        {
            time,
            name,
            symbol,
            side,
            status,
            Id,
            ClientOrderId,
            price,
            qty
        }
        private bool _flag = true;
        private List<Order> orderList = new List<Order>();
        private OrderSortField sortField = OrderSortField.None;
        private readonly ILogger _logger;
        private BackEndClient _client;
        public SlaveOrderForm(ILogger logger)
        {
            _logger = logger;
            InitializeComponent();
        }
        public void GetOrdersUpdate(ExecutionReport report, string _name = null)
        {
            if (InvokeRequired)
            {
                var newReport = report;
                BeginInvoke(new Action<ExecutionReport, string>(GetOrdersUpdate), newReport, null);
            }
            else
            {
                switch (report.OrderStatus)
                {
                    case OrderStatus.NEW:
                    case OrderStatus.CANCELED:
                    case OrderStatus.FILLED:
                        var order = new Order
                        {
                            ClientId = report.ClientId,
                            ExchangeId = report.ExchangeId,
                            Price = report.Price,
                            OrderStatus = report.OrderStatus,
                            Amount = report.Amount,
                            OrderType = report.OrderType,
                            Symbol = report.Symbol,
                            OrderSide = report.OrderSide,
                            StopPrice = report.StopPrice,
                            PositionSide = report.PositionSide
                        };
                        if (orderList.Contains(order, Order.CompareClIdAndExchId) && (order.OrderStatus == OrderStatus.CANCELED || order.OrderStatus == OrderStatus.FILLED))
                        {
                            orderList.RemoveAll(i => i.ClientId == order.ClientId);
                        }
                        if (!orderList.Contains(order, Order.CompareClIdAndExchId) && order.OrderStatus == OrderStatus.NEW)
                        {
                            orderList.Add(order);
                        }
                        break;
                }
                if (sortField != OrderSortField.None)
                {
                    orderList.Sort(new Order.Comparer(sortField, _flag));
                }
                TabText = _client == null ? "Slave open orders" : $"{_client.Name} open orders " + (orderList == null ? "" : $"({orderList.Count})");
                orderDataGridView.RowCount = orderList == null ? 0 : orderList.Count;
                orderDataGridView.Refresh();
            }
        }
        public async void SetConnector(BackEndClient client)
        {
            if (_client != null) 
            {
                _client.OrderUpdateEvent -= GetOrdersUpdate;
            }
            _client = client;
            try
            {
                orderList = await _client.Connector.GetAllOpenOrderds();
            } catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in SlaveOrderForm.SetConnector");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in SlaveOrderForm.SetConnector");
            }
            TabText = _client == null ? "Slave open orders" : $"{_client.Name} open orders " + (orderList == null ? "" : $"({orderList.Count})");
            orderDataGridView.RowCount = orderList == null ? 0 : orderList.Count;
            orderDataGridView.Refresh();
            _client.OrderUpdateEvent += GetOrdersUpdate;
        }
        private void orderDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {

            var currentOrderList = orderList;
            if (e.RowIndex < 0 || currentOrderList == null || e.RowIndex >= currentOrderList?.Count)
            {
                return;
            }
            switch (e.ColumnIndex)
            {
                case (int)MsgColumnType.time:
                    e.Value = DateTime.Now.ToShortTimeString();
                    break;
                case (int)MsgColumnType.name:
                    e.Value = _client.Name;
                    break;
                case (int)MsgColumnType.symbol:
                    e.Value = currentOrderList[e.RowIndex].Symbol;
                    break;
                case (int)MsgColumnType.Id:
                    e.Value = currentOrderList[e.RowIndex].ExchangeId;
                    break;
                case (int)MsgColumnType.ClientOrderId:
                    e.Value = currentOrderList[e.RowIndex].ClientId;
                    break;
                case (int)MsgColumnType.status:
                    e.Value = currentOrderList[e.RowIndex].OrderStatus;
                    break;
                case (int)MsgColumnType.price:
                    e.Value = currentOrderList[e.RowIndex].Price;
                    break;
                case (int)MsgColumnType.qty:
                    e.Value = currentOrderList[e.RowIndex].Amount;
                    break;
                case (int)MsgColumnType.side:
                    e.Value = currentOrderList[e.RowIndex].OrderSide;
                    break;
            }
        }
        private void orderDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _flag = !_flag;
            switch (e.ColumnIndex)
            {
                case (int)MsgColumnType.symbol:
                    orderList.Sort(new Order.Comparer(OrderSortField.Symbol, _flag));
                    sortField = OrderSortField.Symbol;
                    break;
                case (int)MsgColumnType.Id:
                    orderList.Sort(new Order.Comparer(OrderSortField.Id, _flag));
                    sortField = OrderSortField.Id;
                    break;
                case (int)MsgColumnType.ClientOrderId:
                    orderList.Sort(new Order.Comparer(OrderSortField.ClientId, _flag));
                    sortField = OrderSortField.ClientId;
                    break;
                case (int)MsgColumnType.status:
                    orderList.Sort(new Order.Comparer(OrderSortField.Status, _flag));
                    sortField = OrderSortField.Status;
                    break;
                case (int)MsgColumnType.price:
                    orderList.Sort(new Order.Comparer(OrderSortField.Price, _flag));
                    sortField = OrderSortField.Price;
                    break;
                case (int)MsgColumnType.qty:
                    orderList.Sort(new Order.Comparer(OrderSortField.Qty, _flag));
                    sortField = OrderSortField.Qty;
                    break;
                case (int)MsgColumnType.side:
                    orderList.Sort(new Order.Comparer(OrderSortField.Side, _flag));
                    sortField = OrderSortField.Side;
                    break;
            }
            orderDataGridView.Refresh();
        }
    }
}
