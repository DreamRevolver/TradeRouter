using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Shared.Broker;
using Shared.Interfaces;
using Shared.Models;

using SharedBinance.Models;
using WeifenLuo.WinFormsUI.Docking;
namespace NormalRouterApplication
{
    public sealed partial class Positions : DockContent
    {
        public enum MsgColumnType
        {
            time,
            name,
            symbol,
            entryPrice,
            leverage,
            liquidationPrice,
            positionAmt,
            unrealizedProfit,
            positionSide
        }
        private bool _flag = true;
        private readonly RecurrentAction _recurrentAction;
        private List<Position> positions;
        private PositionSortField sortField = PositionSortField.none;
        private FrontEndClient _master;
        private readonly ILogger _logger;
        internal Positions(ILogger logger)
        {
            _logger = logger;
            _recurrentAction = new RecurrentAction(AsyncRefresh, 3000);
            InitializeComponent();
        }
        private void UpdateUnrealizedProfit(MarketBook book, string _symbol)
        {
            positions.Select((a, b) => (a, b)).Where(i => i.a.symbol == _symbol && i.a.positionSide == PositionSide.LONG).AsParallel()
               .ForAll(i => i.a.unRealizedProfit = (book.Bid - i.a.entryPrice) * i.a.positionAmt);
            positions.Select((a, b) => (a, b)).Where(i => i.a.symbol == _symbol && i.a.positionSide == PositionSide.SHORT).AsParallel()
               .ForAll(i => i.a.unRealizedProfit = (i.a.entryPrice - book.Ask) * i.a.positionAmt);
        }
        private void AsyncRefresh()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(AsyncRefresh));
            }
            else
            {
                if (positionsDataGridView != null)
                {
                    positionsDataGridView.Refresh();
                }
            }
        }
        public void GetPositionsUpdate(Position pos)
        {
            if (InvokeRequired)
            {
                var newPositions = pos;
                BeginInvoke(new Action<Position>(GetPositionsUpdate), newPositions);
            }
            else
            {
                if (positions.Contains(pos, new Position()))
                {
                    positions.RemoveAll(i => i.symbol == pos.symbol && i.positionSide == pos.positionSide);
                    try
                    {
                        _master.Connector.Unsibscribe(
                            new Instrument
                            {
                                Symbol = pos.symbol
                            }
                        );
                    } catch (Exception ex)
                    {
                        _logger.Log(LogPriority.Error, ex, Name, $"Exception in SlavePositions.GetPositionsUpdate while unsubscribing from {pos.symbol}");
                        _logger.Log(LogPriority.Debug, ex, Name, $"Exception in SlavePositions.GetPositionsUpdate while unsubscribing from {pos.symbol}");
                    }
                }
                if (pos.positionAmt != 0)
                {
                    positions.Add(pos);
                    try
                    {
                        _master.Connector.Subscribe(
                            new Instrument
                            {
                                Symbol = pos.symbol
                            }, SubscriptionModel.TopBook
                        );
                    } catch (Exception ex)
                    {
                        _logger.Log(LogPriority.Error, ex, Name, $"Exception in SlavePositions.GetPositionsUpdate while subscribing on {pos.symbol}");
                        _logger.Log(LogPriority.Debug, ex, Name, $"Exception in SlavePositions.GetPositionsUpdate while subscribing on {pos.symbol}");
                    }
                }
                if (sortField != PositionSortField.none)
                {
                    positions.Sort(new Position.Comparer(sortField, _flag));
                }
                TabText = _master == null ? "Master positions" : "Master positions " + (positions == null ? "" : $"({positions.Count})");
                positionsDataGridView.RowCount = positions == null ? 0 : positions.Count;
                positionsDataGridView.Refresh();
            }
        }
        public async void SetConnector(FrontEndClient client)
        {
            _master = client;
            try
            {
                positions = (await _master.Connector.GetPositions())?.Where(i => i.positionAmt != 0).ToList();
            } catch (Exception ex)
            {
                _logger.Log(LogPriority.Error, ex, Name, "Exception in Positions.SetConnector while getting positions");
                _logger.Log(LogPriority.Debug, ex, Name, "Exception in Positions.SetConnector while getting positions");
            }
            TabText = _master == null ? "Master positions" : "Master positions " + (positions == null ? "" : $"({positions.Count})");
            positionsDataGridView.RowCount = positions == null ? 0 : positions.Count;
            positionsDataGridView.Refresh();
            foreach (var position in positions)
            {
                try
                {
                    await _master.Connector.Subscribe(
                        new Instrument
                        {
                            Symbol = position.symbol
                        }, SubscriptionModel.TopBook
                    );
                } catch (Exception ex)
                {
                    _logger.Log(LogPriority.Error, ex, Name, $"Exception in Positions.SetConnector while subscribing on {position.symbol}");
                    _logger.Log(LogPriority.Debug, ex, Name, $"Exception in Positions.SetConnector while subscribing on {position.symbol}");
                }
            }
            _master.MarketDataUpdateEvent += UpdateUnrealizedProfit;
            _master.PositionChanged += GetPositionsUpdate;
        }

        private void positionsDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            var currentPositions = positions;
            if (e.RowIndex < 0 || currentPositions == null || e.RowIndex >= currentPositions.Count)
            {
                return;
            }
            switch (e.ColumnIndex)
            {
                case (int)MsgColumnType.time:
                    e.Value = DateTime.Now.ToShortTimeString();
                    break;
                case (int)MsgColumnType.name:
                    e.Value = _master.Connector.Name;
                    break;
                case (int)MsgColumnType.symbol:
                    e.Value = currentPositions[e.RowIndex].symbol;
                    break;
                case (int)MsgColumnType.entryPrice:
                    e.Value = currentPositions[e.RowIndex].entryPrice;
                    break;
                case (int)MsgColumnType.leverage:
                    e.Value = currentPositions[e.RowIndex].leverage;
                    break;
                case (int)MsgColumnType.liquidationPrice:
                    e.Value = currentPositions[e.RowIndex].liquidationPrice;
                    break;
                case (int)MsgColumnType.positionAmt:
                    e.Value = currentPositions[e.RowIndex].positionAmt;
                    break;
                case (int)MsgColumnType.unrealizedProfit:
                    e.Value = currentPositions[e.RowIndex].unRealizedProfit;
                    break;
                case (int)MsgColumnType.positionSide:
                    e.Value = currentPositions[e.RowIndex].positionSide;
                    break;
            }
        }
        private void positionsDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _flag = !_flag;
            switch (e.ColumnIndex)
            {
                case (int)MsgColumnType.symbol:
                    positions.Sort(new Position.Comparer(PositionSortField.symbol, _flag));
                    sortField = PositionSortField.symbol;
                    break;
                case (int)MsgColumnType.entryPrice:
                    positions.Sort(new Position.Comparer(PositionSortField.entryPrice, _flag));
                    sortField = PositionSortField.entryPrice;
                    break;
                case (int)MsgColumnType.leverage:
                    positions.Sort(new Position.Comparer(PositionSortField.leverage, _flag));
                    sortField = PositionSortField.leverage;
                    break;
                case (int)MsgColumnType.liquidationPrice:
                    positions.Sort(new Position.Comparer(PositionSortField.liquidationPrice, _flag));
                    sortField = PositionSortField.liquidationPrice;
                    break;
                case (int)MsgColumnType.positionAmt:
                    positions.Sort(new Position.Comparer(PositionSortField.positionAmt, _flag));
                    sortField = PositionSortField.positionAmt;
                    break;
                case (int)MsgColumnType.unrealizedProfit:
                    positions.Sort(new Position.Comparer(PositionSortField.unrealizedProfit, _flag));
                    sortField = PositionSortField.unrealizedProfit;
                    break;
                case (int)MsgColumnType.positionSide:
                    positions.Sort(new Position.Comparer(PositionSortField.positionSide, _flag));
                    sortField = PositionSortField.positionSide;
                    break;
            }
            positionsDataGridView.Refresh();
        }
        private void Positions_FormClosing(object sender, FormClosingEventArgs e)
            => _recurrentAction.Dispose();

    }
}
