/* eslint-disable @typescript-eslint/no-namespace */
import {v4 as uuidv4, NIL } from 'uuid';

// https://stackoverflow.com/questions/49432350/how-to-represent-guid-in-typescript
export type Guid = string & { isGuid: true }

export namespace Guid {

  export function from(value: string): Guid {
    // TODO: Validate the parameter is an actual guid
    return value as Guid;
  }

  export const empty = Guid.from(NIL);

  export function generate(): Guid {
    const idValue = uuidv4();
    return Guid.from(idValue);
  }

}
