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
import { Panel } from 'primeng/panel';
import { NgIf, NgFor } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { Ripple } from 'primeng/ripple';
import { InputGroup } from 'primeng/inputgroup';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Checkbox } from 'primeng/checkbox';

@NgModule({
    imports: [
        CommonModule,
        TableModule, 
        UtilitiesModule,
        GridModule,
        ProgressSpinner,
        BlockUI,
        PaginatorModule,
        TooltipModule,
        Panel,
        NgIf,
        InputTextModule,
        ButtonModule,
        Ripple,
        InputGroup,
        ReactiveFormsModule,
        Checkbox,
        NgFor,
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
        TooltipModule,
        Panel,
        NgIf,
        InputTextModule,
        ButtonModule,
        Ripple,
        InputGroup,
        ReactiveFormsModule,
        Checkbox,
        NgFor,
        FormsModule
    ]
})
export class RoleSharedModule { }