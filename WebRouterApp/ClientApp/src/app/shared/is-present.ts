export function isDefined(it: unknown | undefined): boolean {
  return it !== undefined;
}

export function isFilled(it: unknown | null): boolean {
  return it !== null;
}

export function isPresent(it: unknown | null | undefined): boolean {
  return isDefined(it) && isFilled(it);
}

function throwError(valueName: string | undefined, valueAsString: string): void {
  throw new Error(`ASSERT: ${valueName ?? 'the value'} is ${valueAsString}.`);
}

export function assertDefined<T>(value: T | undefined, valueName?: string): asserts value is T {
  if (!isFilled(value)) {
    throwError(valueName, 'undefined');
  }
}

export function assertFilled<T>(value: T | null, valueName?: string): asserts value is T {
  if (!isFilled(value)) {
    throwError(valueName, 'null');
  }
}

export function assertPresent<T>(
  value: T | null | undefined,
  valueName?: string
): asserts value is T {
  if (!isPresent(value)) {
    throwError(valueName, 'null or undefined');
  }
}
