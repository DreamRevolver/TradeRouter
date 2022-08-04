import { AfterViewInit, ChangeDetectorRef, Directive, ElementRef } from '@angular/core';

@Directive({
  selector: '[appAutofocus]',
})
export class AutoFocusDirective implements AfterViewInit {
  constructor(private elementRef: ElementRef, private cd: ChangeDetectorRef) {}

  ngAfterViewInit(): void {
    this.elementRef.nativeElement.focus();
    // To prevent errors like this one:
    //   Error: NG0100: ExpressionChangedAfterItHasBeenCheckedError: Expression has changed after it was checked.
    //   Previous value for 'mat-form-field-should-float': 'false'. Current value: 'true'..
    this.cd.detectChanges();
  }
}
