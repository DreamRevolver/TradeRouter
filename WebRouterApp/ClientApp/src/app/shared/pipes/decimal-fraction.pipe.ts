import { Pipe, PipeTransform } from '@angular/core';
import { autoPriceDecimalDigits } from '../utils';

@Pipe({ name: 'decimalFraction', pure: true })
export class DecimalFractionPipe implements PipeTransform {
  transform(decimalFraction: number, fractionalDigits: number): string {
    return decimalFraction.toLocaleString(undefined, {
      minimumFractionDigits: fractionalDigits,
      maximumFractionDigits: fractionalDigits,
    });
  }
}

@Pipe({ name: 'priceDecimalAuto', pure: true })
export class PriceDecimalAutoPipe implements PipeTransform {
  transform(price: number): string {
    return price.toLocaleString(undefined, {
      minimumFractionDigits: autoPriceDecimalDigits(Math.abs(price)), //no matter the direction
      maximumFractionDigits: autoPriceDecimalDigits(Math.abs(price)),
    });
      //Stupid approach to save the amount of calculations. Maybe it could be optimized by the transpiler
  }

  
}