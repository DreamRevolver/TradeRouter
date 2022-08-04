import { Injectable } from '@angular/core';
import { CoeffKinds } from '../models/subscriber-record';

@Injectable({ providedIn: 'root' })
export class CoeffKindsRenderer {
  readonly coeffKinds = (): CoeffKinds[] => [CoeffKinds.CoeffToSize];
  readonly stringify = (coeffKind: CoeffKinds): string => CoeffKinds.stringify(coeffKind);
}
