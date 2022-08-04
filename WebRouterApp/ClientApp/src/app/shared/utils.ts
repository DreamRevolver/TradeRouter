export function parseMilliseconds(milliseconds: number): {
  hours: number;
  minutes: number;
  seconds: number;
} {
  const millisecondsInSecond = 1000;
  const millisecondsInMinute = millisecondsInSecond * 60;
  const millisecondsInHour = millisecondsInMinute * 60;

  const hours = Math.floor(milliseconds / millisecondsInHour);
  milliseconds -= hours * millisecondsInHour;

  const minutes = Math.floor(milliseconds / millisecondsInMinute);
  milliseconds -= minutes * millisecondsInMinute;

  const seconds = Math.floor(milliseconds / millisecondsInSecond);

  return { hours, minutes, seconds };
}

export function hasAnyProperties(object: { [propName: string]: unknown }): boolean {
  // noinspection LoopStatementThatDoesntLoopJS
  for (const prop in object) {
    return true;
  }

  return false;
}

export function autoPriceDecimalDigits(price: number): number {
  price = Math.abs(price);
  if (price >= 100) return 2;
  if (price >= 10) return 3;
  /*if (price >= 1)
    return 4;*/

  return 4;
}
