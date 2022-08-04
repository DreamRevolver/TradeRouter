import { EventEmitter, Injectable } from '@angular/core';
import * as SignalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { IBatchMessage } from '../models/messages/batch.message';

@Injectable({ providedIn: 'root' })
export class TradingHub {
  readonly reconnected = new EventEmitter<{ connectionId: string | null }>();
  readonly closed = new EventEmitter<{ error: Error | null }>();

  readonly batchReceived = new EventEmitter<IBatchMessage>();

  private readonly hubUrl = `${environment.apiUrl}/tradingHub`;
  private hubConnection: SignalR.HubConnection | null = null;

  get connectionState(): SignalR.HubConnectionState {
    return this.hubConnection?.state ?? SignalR.HubConnectionState.Disconnected;
  }

  get connectionId(): string | null {
    return this.hubConnection?.connectionId ?? null;
  }

  async start(): Promise<void> {
    this.hubConnection = new SignalR.HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .configureLogging(SignalR.LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.onclose((error) => this.closed.emit({ error: error ?? null }));

    this.hubConnection.onreconnected((connectionId) => {
      this.reconnected.emit({ connectionId: connectionId ?? null });
    });

    this.hubConnection.on(
      'BatchReceived', //
      (message: IBatchMessage) => this.batchReceived.emit(message)
    );

    await this.hubConnection.start();
  }

  async stop(): Promise<void> {
    this.hubConnection?.stop();
  }
}
