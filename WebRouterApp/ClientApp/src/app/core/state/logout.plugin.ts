import { getActionTypeFromInstance, NGXS_PLUGINS, NgxsNextPluginFn } from '@ngxs/store';
import { AuthActions } from '../../auth/state/auth.actions';
import { ModuleWithProviders, NgModule } from '@angular/core';

export function logoutPlugin(
  state: Record<string, unknown>,
  action: unknown,
  next: NgxsNextPluginFn
): unknown {
  if (getActionTypeFromInstance(action) === AuthActions.LogOut.type) {
    for (const featureKey in state) {
      const featureState = state[featureKey] as Record<string, Record<string, unknown>>;
      const defaults = featureState['defaults'];

      state = {
        ...state,
        ...{ [featureKey]: defaults ? { ...featureState, ...defaults } : {} },
      };
    }
  }

  return next(state, action);
}

@NgModule()
export class NgxsLogOutPluginModule {
  static forRoot(): ModuleWithProviders<NgxsLogOutPluginModule> {
    return {
      ngModule: NgxsLogOutPluginModule,
      providers: [
        {
          provide: NGXS_PLUGINS,
          useValue: logoutPlugin,
          multi: true,
        },
      ],
    };
  }
}
