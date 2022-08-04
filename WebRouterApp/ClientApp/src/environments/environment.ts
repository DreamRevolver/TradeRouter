// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { NgxsLoggerPluginModule } from '@ngxs/logger-plugin';
import { TradingMessageReceivedActions } from '../app/trading/state/trading.actions';
import { getActionTypeFromInstance } from '@ngxs/store';
import { LogsApiActions } from '../app/logs/state/logs.actions'; // Included with Angular CLI.

export const environment = {
  production: false,
  imports: [
    NgxsLoggerPluginModule.forRoot({
      filter: (action) =>
        ![
          TradingMessageReceivedActions.Batch.type, //
          LogsApiActions.GetAll.type,
        ].includes(getActionTypeFromInstance(action) ?? ''),
    }),
  ],
  apiUrl: 'http://localhost:5000',
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
import 'zone.js/plugins/zone-error';
