import {
  Component,
  Directive,
  Input,
  TemplateRef,
  ContentChild,
  ContentChildren,
  QueryList,
  OnInit,
  AfterContentInit
} from '@angular/core';

@Component({
  selector: 'tw-header',
  template: '<ng-content></ng-content>'
})
export class HeaderComponent { }

@Component({
  selector: 'tw-footer',
  template: '<ng-content></ng-content>'
})
export class FooterComponent { }

@Directive({
  selector: '[twTemplate]',
})
export class TwTemplateDirective {
  // tslint:disable-next-line:no-input-rename
  @Input('twTemplate')
  public name: string;

  public constructor(
    public template: TemplateRef<any>
  ) { }
}

@Component({
  selector: 'tw-list',
  template: `
  <div class="list">
    <div class="list--loading" *ngIf="loading">
      <tw-icon name="spinner" [spin]="true"></tw-icon>
    </div>
    <div class="list__header">
      <ng-content select="tw-header"></ng-content>
    </div>
    <div class="list__content">
      <ng-template ngFor let-rowData let-rowIndex="index" [ngForOf]="rows">
        <ng-container
          *ngTemplateOutlet="itemTemplate; context: {$implicit: rowData, rowIndex: rowIndex}">
        </ng-container>
      </ng-template>
      <div *ngIf="isEmpty()">No data!</div>
    </div>
    <div class="list__footer">
      <ng-content select="tw-footer"></ng-content>
    </div>
  </div>
  `
})
export class ListComponent implements OnInit, AfterContentInit {
  private _rows: any[];

  @Input()
  public loading: boolean;

  @Input()
  public get rows(): any[] {
    return this._rows;
  }
  public set rows(val: any[]) {
    this._rows = val;

    this.loading = false;

    if (!this.isEmpty) {
      this.ngAfterContentInit();
    }
  }

  @ContentChild(HeaderComponent)
  public header;

  @ContentChild(FooterComponent)
  public footer;

  @ContentChildren(TwTemplateDirective)
  public templates: QueryList<TwTemplateDirective>;

  public itemTemplate: TemplateRef<any>;

  public ngOnInit(): void {
    this.loading = true;
  }

  public ngAfterContentInit(): void {
    this.templates.forEach((t: TwTemplateDirective) => {
      this.itemTemplate = t.template;
    });
  }

  public isEmpty() {
    const data = this.rows;
    return data == null || data.length === 0;
  }

  public attachRows(rows: any[]): void {
    if (this.isEmpty()) {
      this.rows = rows;
    } else {
      this.rows.push(rows);
    }
  }
}
