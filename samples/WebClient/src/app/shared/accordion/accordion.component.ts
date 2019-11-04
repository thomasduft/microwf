import {
  Component,
  ContentChildren,
  QueryList,
  AfterContentInit,
  Input,
  HostBinding
} from '@angular/core';

import { AccordionGroupComponent } from './accordion-group.component';

@Component({
  selector: 'tw-accordion',
  template: `
    <ng-content></ng-content>
  `
})
export class AccordionComponent implements AfterContentInit {
  @HostBinding('class')
  public style = 'accordion';

  @Input()
  public useSingleGroup = false;

  @ContentChildren(AccordionGroupComponent)
  public groups: QueryList<AccordionGroupComponent>;

  /**
   * Invoked when all children (groups) are ready
   */
  public ngAfterContentInit(): void {
    console.log(this.groups);
    // Set active to first element
    this.groups.toArray()[0].opened = true;

    // Loop through all Groups
    this.groups.toArray().forEach((t) => {
      // when title bar is clicked
      // (toggle is an @output event of Group)
      t.toggle.subscribe(() => {
        // Open the group
        this.openGroup(t);
      });
      /*t.toggle.subscribe((group) => {
        // Open the group
        this.openGroup(group);
      });*/
    });
  }


  private openGroup(group: AccordionGroupComponent): void {
    console.log(group);

    // close other groups
    if (this.useSingleGroup) {
      this.groups.toArray().forEach((t) => t.opened = false);
    }

    // open current group
    group.opened = !group.opened;
  }
}
