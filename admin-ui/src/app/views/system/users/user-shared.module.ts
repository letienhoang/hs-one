import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import {
    UtilitiesModule,
    GridModule,
    TooltipModule
} from '@coreui/angular';
import { PaginatorModule } from 'primeng/paginator';
import { ProgressSpinner } from 'primeng/progressspinner';
import { BlockUI } from 'primeng/blockui';
import { PanelModule } from 'primeng/panel';
import { NgIf, NgFor } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { Ripple } from 'primeng/ripple';
import { InputGroup } from 'primeng/inputgroup';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Checkbox } from 'primeng/checkbox';
import { BadgeModule } from 'primeng/badge';
import { KeyFilter } from 'primeng/keyfilter';
import { ImageModule } from 'primeng/image';
import { HSOneSharedModule } from '../../../shared/modules/hs-one-shared.module';
import { PickListModule } from 'primeng/picklist';

@NgModule({
    imports: [
        CommonModule,
        TableModule, 
        UtilitiesModule,
        GridModule,
        ProgressSpinner,
        BlockUI,
        PaginatorModule,
        PanelModule,
        NgIf,
        InputTextModule,
        ButtonModule,
        Ripple,
        InputGroup,
        ReactiveFormsModule,
        Checkbox,
        NgFor,
        TooltipModule,
        BadgeModule,
        KeyFilter,
        ImageModule,
        HSOneSharedModule,
        PickListModule,
        FormsModule
    ],
    exports: [
        CommonModule,
        TableModule, 
        UtilitiesModule,
        GridModule,
        ProgressSpinner,
        BlockUI,
        PaginatorModule,
        PanelModule,
        NgIf,
        InputTextModule,
        ButtonModule,
        Ripple,
        InputGroup,
        ReactiveFormsModule,
        Checkbox,
        NgFor,
        TooltipModule,
        BadgeModule,
        KeyFilter,
        ImageModule,
        HSOneSharedModule,
        PickListModule,
        FormsModule
    ]
})
export class UserSharedModule { }