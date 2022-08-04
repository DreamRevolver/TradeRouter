import { NgModule } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PublisherRecordPageComponent } from './components/publisher-record-page.component';
import { SharedModule } from '../shared/shared.module';
import { NgxsModule } from '@ngxs/store';
import { PublisherRecordsState } from './state/publisher-records.state';

const routes: Routes = [
  {
    path: '',
    component: PublisherRecordPageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [],
})
class PublisherRecordsRoutingModule {}

@NgModule({
  declarations: [PublisherRecordPageComponent],
  imports: [
    CommonModule, //
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    PublisherRecordsRoutingModule,
    NgxsModule.forFeature([PublisherRecordsState]),
  ],
})
export class PublisherRecordsModule {}
