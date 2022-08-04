export abstract class ActionWith<TPayload> {
  constructor(public payload: TPayload) {}
}
