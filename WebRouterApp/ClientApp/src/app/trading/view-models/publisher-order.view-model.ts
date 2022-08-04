export interface IPublisherOrderViewModel {
  id: string;
  opentime: Date;
  node: string;
  status: string;
  symbol: string;
  entryprice: number;
  size: number;
  side: string;
  unsynchronized: number;
  client_Id: string;
}
