import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import {
    UtilitiesModule,
    GridModule,
    TooltipModule
} from '@coreui/angular';
import { PaginatorModule } from 'primeng/paginator';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { PanelModule } from 'primeng/panel';
import { NgIf, NgFor } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { InputGroupModule } from 'primeng/inputgroup';
import { ReactiveFormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';
import { BadgeModule } from 'primeng/badge';
import { KeyFilterModule } from 'primeng/keyfilter';
import { ImageModule } from 'primeng/image';
import { HSOneSharedModule } from '../../../shared/modules/hs-one-shared.module';
import { PickListModule } from 'primeng/picklist';

@NgModule({
    imports: [
        CommonModule,
        TableModule, 
        UtilitiesModule,
        GridModule,
        ProgressSpinnerModule,
        BlockUIModule,
        PaginatorModule,
        PanelModule,
        NgIf,
        InputTextModule,
        ButtonModule,
        RippleModule,
        InputGroupModule,
        ReactiveFormsModule,
        CheckboxModule,
        NgFor,
        TooltipModule,
        BadgeModule,
        KeyFilterModule,
        ImageModule,
        HSOneSharedModule,
        PickListModule
    ],
    exports: [
        CommonModule,
        TableModule, 
        UtilitiesModule,
        GridModule,
        ProgressSpinnerModule,
        BlockUIModule,
        PaginatorModule,
        PanelModule,
        NgIf,
        InputTextModule,
        ButtonModule,
        RippleModule,
        InputGroupModule,
        ReactiveFormsModule,
        CheckboxModule,
        NgFor,
        TooltipModule,
        BadgeModule,
        KeyFilterModule,
        ImageModule,
        HSOneSharedModule,
        PickListModule
    ]
})
export class UserSharedModule { }