export enum TraderStatus {
  Stopped = 0,
  Paused = 1,
  Running = 2,
}

export namespace TraderStatus {
  export function stringify(status: TraderStatus): string {
    switch (status) {
      case TraderStatus.Stopped:
        return 'Stopped';
      case TraderStatus.Paused:
        return 'Paused';
      case TraderStatus.Running:
        return 'Running';
    }

    return `UNKNOWN TRADER STATUS '${status}'`;
  }
}
