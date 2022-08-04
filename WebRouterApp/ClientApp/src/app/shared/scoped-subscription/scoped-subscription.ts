import { Injectable, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

// interface ObservableObserverPair<T> {
//   observable$: Observable<T>;
//   observer: (it: T) => void;
// }

@Injectable()
export class ScopedSubscription implements OnDestroy {
  private readonly subscription = new Subscription();

  on<T>(observable$: Observable<T>, observer: (it: T) => void): ScopedSubscription {
    this.subscription.add(observable$.subscribe(observer));
    return this;
  }

  // ons(pairs: ObservableObserverPair<unknown>[]): ScopedSubscription {
  //   for (const { observable$, observer } of pairs) {
  //     this.on(observable$, observer);
  //   }
  //
  //   return this;
  // }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
