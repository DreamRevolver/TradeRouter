import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { IPublisherOverviewViewModel } from '../view-models/publisher-overview.view-model-queries';
import { TraderStatus } from '../models/trader-status';

@Component({
  selector: 'app-publishers',
  template: `
    <app-traders
      [traders]="publishers"
      [filterErrorsEnabled]="filterErrorsEnabled"
      [statusFilter]="statusFilter"
      [pagingEnabled]="false"
    >
      <ng-template #orders let-publisher>
        {{ publisher.orders }}
      </ng-template>
      <ng-template #positions let-publisher>
        {{ publisher.positions }}
      </ng-template>
    </app-traders>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PublishersComponent {
  @Input() publishers: IPublisherOverviewViewModel[] | null = [];
  @Input() filterErrorsEnabled: boolean | null = false;
  @Input() statusFilter: TraderStatus | null = null;
}
