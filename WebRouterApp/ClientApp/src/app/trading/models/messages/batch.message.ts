import { TradingMessage } from './trading.message';
import { TradingMessageTags } from './trading-message-tags';

export interface IBatchMessage {
  tag: TradingMessageTags.Batch;
  messages: TradingMessage[];
}
