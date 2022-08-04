import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-spinner',
  template: `
    <ngx-spinner
      bdColor="rgba(0,0,0,0.5)"
      size="medium"
      color="#fff"
      type="line-scale"
      [name]="name"
      [zIndex]="3"
      [fullScreen]="false"
      [showSpinner]="_show"
    >
    </ngx-spinner>
  `,
})
export class SpinnerComponent implements OnChanges {
  @Input() name = '';
  @Input() showSpinner = false;
  _show = false;

  ngOnChanges(changes: SimpleChanges): void {
    const showSpinnerChange = changes['showSpinner'];
    if (!showSpinnerChange) {
      return;
    }

    if (showSpinnerChange.isFirstChange()) {
      // A workaround for [Failing to activate spinner using \[showSpinner\]]
      // (https://github.com/Napster2210/ngx-spinner/issues/193)
      this._show = !this.showSpinner;
      setTimeout(() => (this._show = this.showSpinner));

      return;
    }

    this._show = this.showSpinner;
  }
}
