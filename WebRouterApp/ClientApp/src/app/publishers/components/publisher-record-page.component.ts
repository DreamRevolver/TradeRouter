import { Component, OnInit } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { ScopedSubscription } from '../../shared/scoped-subscription/scoped-subscription';
import { PublisherRecordsState } from '../state/publisher-records.state';
import { IPublisherRecord } from '../models/publisher-record';
import { PublisherRecordsApiActions } from '../state/publisher-records.actions';
import { Guid } from '../../shared/types/guid';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-publisher-record-page',
  template: `
    <mat-card class="mat-elevation-z0">
      <app-spinner
        [name]="'app-publisher-record-page-spinner'"
        [showSpinner]="(isBusy$ | async) || false"
      ></app-spinner>
      <mat-card-title>Publisher Settings</mat-card-title>
      <mat-card-content>
        <form class="form-size" #form="ngForm" (ngSubmit)="save()" ngNativeValidate>
          <mat-form-field appearance="fill">
            <mat-label>Name</mat-label>
            <input matInput [(ngModel)]="publisherRecord.name" name="name" required />
          </mat-form-field>
          <p>
            <mat-form-field appearance="fill">
              <mat-label>Description</mat-label>
              <textarea
                matInput
                [(ngModel)]="publisherRecord.description"
                name="description"
              ></textarea>
            </mat-form-field>
          </p>

          <p>
            <mat-form-field appearance="fill">
              <mat-label>Api Key</mat-label>
              <input matInput [(ngModel)]="publisherRecord.apiKey" required name="apikey" />
            </mat-form-field>
          </p>

          <p>
            <mat-form-field appearance="fill">
              <mat-label>Api Secret</mat-label>
              <input
                matInput
                type="{{ showApiSecret ? 'text' : 'password' }}"
                [(ngModel)]="publisherRecord.apiSecret"
                name="apiSecret"
                required
              />
              <button
                type="button"
                mat-icon-button
                matSuffix
                aria-label="Show/hide apisecret"
                (click)="showApiSecret = !showApiSecret"
              >
                <mat-icon>visibility</mat-icon>
              </button>
            </mat-form-field>
          </p>

          <p>
            <mat-slide-toggle
              [(ngModel)]="publisherRecord.tradeAllOrdersAsMarket"
              name="tradeAllOrdersAsMarket"
              >Trade all orders as market
            </mat-slide-toggle>
          </p>
          <p>
            <button mat-button type="submit">Save</button>
          </p>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      @import '../../../assets/styles/ftables.scss';
      @import '../../../assets/styles/panels.scss';

      .example-form mat-slide-toggle {
        margin: 8px 0;
        display: block;
      }

      mat-form-field {
        width: 600px;
      }

      .example-form-field {
        width: 200px;
      }
    `,
  ],
})
export class PublisherRecordPageComponent implements OnInit {
  constructor(private readonly store: Store, private readonly ss: ScopedSubscription) {
    ss.on(
      store.select(PublisherRecordsState.publisherRecord), //
      (it) => (this.publisherRecord = { ...it })
    );
  }

  showApiSecret = false;
  publisherRecord: IPublisherRecord = {
    id: Guid.empty,
    apiKey: '',
    apiSecret: '',
    name: '',
    description: '',
    tradeAllOrdersAsMarket: false,
  };

  @Select(PublisherRecordsState.isBusy)
  readonly isBusy$!: Observable<boolean>;

  ngOnInit(): void {
    this.store.dispatch(new PublisherRecordsApiActions.GetRecords());
  }

  save(): void {
    if (!this.publisherRecord) {
      return;
    }

    this.store.dispatch(new PublisherRecordsApiActions.Update({ record: this.publisherRecord }));
  }
}
