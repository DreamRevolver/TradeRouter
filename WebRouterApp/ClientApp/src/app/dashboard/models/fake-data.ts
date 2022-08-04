export interface ITraderNode {
  node: string;
  uptime: number;
  ping: number;
  balance: number;
  orders: number;
  positions: number;
  description: string;
  errors: number;
  status: string;
}

export const PUBLISHERS_DATA: ITraderNode[] = [
  {
    node: 'Master1',
    ping: 1,
    uptime: 90061000,
    balance: 150000,
    orders: 6,
    positions: 3,
    description: 'USDT future account',
    errors: 1,
    status: 'Running',
  },
];

export const SUBSCRIBERS_DATA: ITraderNode[] = [];
for (let i = 0; i < 67; i++) {
  SUBSCRIBERS_DATA[i] = {
    node: 'Subscriber' + i,
    ping: Math.round(Math.random() * 10),
    uptime: 89061000 + Math.random() * 1000000,
    balance: Math.round(Math.random() * 100000),
    orders: (Math.round(Math.random() * PUBLISHERS_DATA[0].orders) % PUBLISHERS_DATA[0].orders) + 1,
    positions: (Math.round(Math.random() * PUBLISHERS_DATA[0].positions) % PUBLISHERS_DATA[0].positions) + 1,
    description: 'Lores ispum descriptious',
    errors: i % 5 === 0 ? Math.round(Math.random() > 0.55 ? (Math.random() % 0.2) * 10 : 0) : 0,
    status: '',
  };
  SUBSCRIBERS_DATA[i].status =
    SUBSCRIBERS_DATA[i].positions == PUBLISHERS_DATA[0].positions
      ? 'Running'
      : SUBSCRIBERS_DATA[i].positions < PUBLISHERS_DATA[0].positions / 2
      ? 'Stopped'
      : 'Paused';
}
