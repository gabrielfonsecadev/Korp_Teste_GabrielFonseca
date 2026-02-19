import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Invoice, InvoiceStatus } from '../../models/invoice.model';
import { InvoiceService } from '../../services/invoice.service';
import { InvoiceFormComponent } from '../invoice-form/invoice-form.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'app-invoice-list',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        MatTableModule,
        MatButtonModule,
        MatIconModule,
        MatCardModule,
        MatChipsModule,
        MatProgressSpinnerModule,
        MatSnackBarModule,
        MatTooltipModule
    ],
    templateUrl: './invoice-list.component.html',
    styleUrls: ['./invoice-list.component.css']
})
export class InvoiceListComponent implements OnInit {
    invoices: Invoice[] = [];
    displayedColumns: string[] = ['number', 'status', 'createdAt', 'actions'];
    loadingPrint: { [key: number]: boolean } = {};
    InvoiceStatus = InvoiceStatus;
    private invoiceFormDialogRef?: MatDialogRef<InvoiceFormComponent>;

    constructor(
        private invoiceService: InvoiceService,
        private snackBar: MatSnackBar,
        private dialog: MatDialog
    ) { }

    ngOnInit(): void {
        this.loadInvoices();
    }

    loadInvoices(): void {
        this.invoiceService.getAll().subscribe({
            next: (data) => this.invoices = data,
            error: (err) => console.error('Erro ao carregar notas fiscais', err)
        });
    }

    openInvoiceFormModal(): void {
        this.invoiceFormDialogRef = this.dialog.open<InvoiceFormComponent>(InvoiceFormComponent, {
            width: '580px',
            maxHeight: '90vh',
            hasBackdrop: true,
            panelClass: 'sidebar-modal',
            restoreFocus: false,
            data: {}
        });

        this.invoiceFormDialogRef.afterClosed().subscribe((result: any) => {
            if (result) {
                this.loadInvoices();
            }
        });
    }

    printInvoice(id: number): void {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '400px',
            data: {
                title: 'Confirmar Impressão',
                message: 'Tem certeza que deseja imprimir esta nota fiscal?',
                confirmText: 'Imprimir',
                cancelText: 'Cancelar',
                color: 'primary'
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.loadingPrint[id] = true;

                this.invoiceService.print(id).subscribe({
                    next: (updatedInvoice) => {
                        const index = this.invoices.findIndex(i => i.id === id);
                        if (index !== -1) {
                            this.invoices[index] = updatedInvoice;
                        }
                        this.snackBar.open('Nota fiscal impressa com sucesso!', 'Fechar', { duration: 3000 });
                        this.loadingPrint[id] = false;
                        this.loadInvoices();
                    },
                    error: (err) => {
                        this.loadingPrint[id] = false;
                        this.snackBar.open('Erro ao imprimir nota: ' + (err.error?.error || err.message), 'Fechar', { duration: 5000 });
                    }
                });
            }
        });
    }

    deleteInvoice(id: number): void {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '400px',
            data: {
                title: 'Confirmar Exclusão',
                message: 'Tem certeza que deseja excluir esta nota fiscal?',
                confirmText: 'Excluir',
                cancelText: 'Cancelar',
                color: 'warn'
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.invoiceService.delete(id).subscribe({
                    next: () => {
                        this.snackBar.open('Nota fiscal excluída com sucesso!', 'Fechar', { duration: 3000 });
                        this.loadInvoices();
                    },
                    error: (err) => {
                        this.snackBar.open('Erro ao excluir nota: ' + (err.error?.error || err.message), 'Fechar', { duration: 5000 });
                    }
                });
            }
        });
    }
}
