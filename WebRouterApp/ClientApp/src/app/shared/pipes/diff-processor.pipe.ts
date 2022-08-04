import { Pipe, PipeTransform } from '@angular/core';
import { autoPriceDecimalDigits } from '../utils';

@Pipe({ name: 'showDiff', pure: true })
export class DiffStringGeneratorPipe implements PipeTransform {
  transform(diff: number, price:number, isReversed:boolean): string {

    if (diff === 0)
      return ' ';
    //TODO: 'LONG' 'SHORT' must become a const
    return `${diff * (isReversed ? -1 : 1) > 0 ? "▲" : "▼"} ${diff.toFixed(autoPriceDecimalDigits (price))} (${Math.abs(diff / price * 100).toFixed(2)}%)`
     
  }
}
