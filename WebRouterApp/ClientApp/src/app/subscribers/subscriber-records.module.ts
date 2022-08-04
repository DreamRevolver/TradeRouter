import { NgModule } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SubscriberRecordsPageComponent } from './components/subscriber-records-page.component';
import { SubscriberRecordDialogComponent } from './components/subscriber-record-dialog.component';
import { NgxsModule } from '@ngxs/store';
import { SubscriberRecordsState } from './state/subscriber-records.state';
import { SharedModule } from '../shared/shared.module';

const routes: Routes = [
  {
    path: '',
    component: SubscriberRecordsPageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [],
})
class SubscriberRecordsRoutingModule {}

@NgModule({
  declarations: [SubscriberRecordsPageComponent, SubscriberRecordDialogComponent],
  imports: [
    CommonModule, //
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    SubscriberRecordsRoutingModule,
    NgxsModule.forFeature([SubscriberRecordsState]),
  ],
})
export class SubscriberRecordsModule {}
