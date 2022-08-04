import { ModuleWithProviders, NgModule } from '@angular/core';
import { ScopedSubscription } from './scoped-subscription';

@NgModule()
export class ScopedSubscriptionModule {
  public static forRoot(): ModuleWithProviders<ScopedSubscriptionModule> {
    return {
      ngModule: ScopedSubscriptionModule,
      providers: [
        {
          provide: ScopedSubscription,
          useFactory: () => new ScopedSubscription(),
        },
      ],
    };
  }
}
