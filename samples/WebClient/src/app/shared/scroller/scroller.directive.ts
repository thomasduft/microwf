import {
  Directive,
  AfterViewInit,
  OnDestroy,
  HostListener,
  EventEmitter,
  Input,
  Output
} from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil, filter, map } from 'rxjs/operators';

import { Scroll } from './models';

/**
 * @example
 * ```html
 * <div
 *   twScroller
 *   [offset]="100"
 *   [debounce]="100"
 *   [disabled]="disabled"
 *   (scrollEnd)="onScrollEnd()">
 * </div>
 * ```
 */
@Directive({
  selector: '[twScroller]'
})
export class ScrollerDirective implements AfterViewInit, OnDestroy {
  /**
   * An offset from the bottom of the element to trigger
   * `scrollEnd` event
   */
  @Input()
  public offset = 0;

  /**
   * Specify debounce duration in ms
   */
  @Input()
  public debounce = 100;

  /**
   * If true then `scrollEnd` event should NOT be emitted
   */
  @Input()
  public disabled = false;

  /**
   * Event that will be triggered when user has scrolled to
   * bottom of the element
   */
  @Output()
  public scrollEnd = new EventEmitter<void>();

  /**
   * Emits a new value on element scroll event
   */
  private scroll$: Subject<Scroll> = new Subject<Scroll>();
  private ngUnsubscribe$ = new Subject<void>();

  /**
   * Subscribe to `scroll$` observable and emit `scrollEnd` event
   * when element scroll position is at the end of the element
   */
  public ngAfterViewInit(): void {
    this.scroll$
      .pipe(
        takeUntil(this.ngUnsubscribe$),
        debounceTime(this.debounce),
        map(scroll => {
          const y = scroll.y + this.offset;
          return { y, height: scroll.height };
        }),
        filter(() => !this.disabled),
        filter(scroll => scroll.y >= scroll.height)
      )
      .subscribe(() => this.scrollEnd.emit());
  }

  /**
   * On element scroll event emit next `scroll$` observable value
   */
  @HostListener('scroll', [
    '$event.target.scrollTop',
    '$event.target.scrollHeight',
    '$event.target.offsetHeight'
  ])
  public onScroll(
    scrollY: number,
    scrollheight: number,
    offsetHeight: number
  ): void {
    const height = scrollheight;
    const y = scrollY + offsetHeight;

    this.scroll$.next({ y, height });
  }

  /**
   * trigger `ngUnsubscribe` complete on
   * component destroy lifecycle hook
   */
  public ngOnDestroy(): void {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
