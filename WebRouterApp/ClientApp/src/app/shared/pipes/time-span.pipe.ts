import { Pipe, PipeTransform } from '@angular/core';
import { parseMilliseconds } from '../utils';

@Pipe({ name: 'timeSpan', pure: true })
export class TimeSpanPipe implements PipeTransform {
  transform(milliseconds: number): string {
    const { hours, minutes, seconds } = parseMilliseconds(milliseconds);
    return hours + 'h:' + minutes + 'm:' + seconds + 's';
  }
}
