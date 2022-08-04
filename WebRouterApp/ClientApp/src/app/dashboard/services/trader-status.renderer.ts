import { Injectable } from '@angular/core';
import { TraderStatus } from '../models/trader-status';

@Injectable({ providedIn: 'root' })
export class TraderStatusRenderer {
  colorFor(status: string): string {
    switch (status) {
      case TraderStatus.stringify(TraderStatus.Running):
        return 'green';
      case TraderStatus.stringify(TraderStatus.Paused):
        return 'grey';
      default:
        return 'red';
    }
  }
}
