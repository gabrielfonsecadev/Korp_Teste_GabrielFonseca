import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ProductFormComponent } from '../product-form/product-form.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'app-product-list',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        MatTableModule,
        MatButtonModule,
        MatIconModule,
        MatCardModule
    ],
    templateUrl: './product-list.component.html',
    styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
    products: Product[] = [];
    displayedColumns: string[] = ['code', 'description', 'stock', 'actions'];
    private productFormDialogRef?: MatDialogRef<ProductFormComponent>;

    constructor(
        private productService: ProductService,
        private dialog: MatDialog) { }

    ngOnInit(): void {
        this.loadProducts();
    }

    loadProducts(): void {
        this.productService.getAll().subscribe({
            next: (data) => this.products = data,
            error: (err) => console.error('Erro ao carregar produtos', err)
        });
    }

    deleteProduct(id: number): void {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '400px',
            data: {
                title: 'Confirmar Exclusão',
                message: 'Tem certeza que deseja excluir este produto?',
                confirmText: 'Excluir',
                cancelText: 'Cancelar',
                color: 'warn'
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.productService.delete(id).subscribe({
                    next: () => this.loadProducts(),
                    error: (err) => alert('Erro ao excluir produto: ' + err.message)
                });
            }
        });
    }

    openProductFormModal(id?: number): void {
        this.productFormDialogRef = this.dialog.open<ProductFormComponent>(ProductFormComponent, {
            width: '400px',
            maxWidth: '400px',
            maxHeight: '90vh',
            hasBackdrop: true,
            panelClass: 'sidebar-modal',
            restoreFocus: false,
            data: { id: id }
        });

        this.productFormDialogRef.afterClosed().subscribe((result: any) => {
            if (result) {
                this.loadProducts();
            }
        });
    }
}
