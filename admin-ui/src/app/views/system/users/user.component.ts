import { AfterContentInit, ChangeDetectionStrategy, ChangeDetectorRef, Component } from '@angular/core';
import { IconDirective } from '@coreui/icons-angular';
import { DocsExampleComponent } from '@docs-components/public-api';
import { TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, RowComponent, ColComponent, WidgetStatBComponent, ProgressBarDirective, ProgressComponent, WidgetStatFComponent, TemplateIdDirective, CardGroupComponent, WidgetStatCComponent } from '@coreui/angular';

@Component({
    selector: 'app-users',
    templateUrl: './user.component.html',
    styleUrls: ['./user.component.scss'],
    changeDetection: ChangeDetectionStrategy.Default,
    standalone: true,
    imports: [TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, DocsExampleComponent, RowComponent, ColComponent, WidgetStatBComponent, ProgressBarDirective, ProgressComponent, WidgetStatFComponent, TemplateIdDirective, IconDirective, CardGroupComponent, WidgetStatCComponent]
})
export class UserComponent implements AfterContentInit {
  constructor(
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngAfterContentInit(): void {
    this.changeDetectorRef.detectChanges();
  }
}
