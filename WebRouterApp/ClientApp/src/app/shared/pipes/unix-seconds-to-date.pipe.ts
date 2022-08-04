import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'unixSecondsToDate', pure: true })
export class UnixSecondsToDatePipe implements PipeTransform {
  transform(unixSeconds: number): Date {
    return new Date(unixSeconds * 1000);
  }
}
