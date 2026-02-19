import { Component, Inject, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { InvoiceService } from '../../services/invoice.service';
import { CreateInvoiceRequest } from '../../models/invoice.model';

@Component({
    selector: 'app-invoice-form',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatButtonModule,
        MatCardModule,
        MatIconModule,
        MatSnackBarModule
    ],
    templateUrl: './invoice-form.component.html',
    styleUrls: ['./invoice-form.component.css']
})
export class InvoiceFormComponent implements OnInit {
    invoiceForm: FormGroup;
    products: Product[] = [];
    private dialogRef = inject(MatDialogRef<InvoiceFormComponent>);

    constructor(
        private fb: FormBuilder,
        private productService: ProductService,
        private invoiceService: InvoiceService,
        private snackBar: MatSnackBar,
        @Inject(MAT_DIALOG_DATA) public data: {}
    ) {
        this.invoiceForm = this.fb.group({
            items: this.fb.array([], [Validators.required])
        });
    }

    get items(): FormArray {
        return this.invoiceForm.get('items') as FormArray;
    }

    ngOnInit(): void {
        this.loadProducts();
        this.addItem();
    }

    loadProducts(): void {
        this.productService.getAll().subscribe({
            next: (data) => this.products = data,
            error: (err) => console.error('Erro ao carregar produtos', err)
        });
    }

    addItem(): void {
        const itemGroup = this.fb.group({
            productId: ['', Validators.required],
            quantity: [1, [Validators.required, Validators.min(1)]]
        });
        this.items.push(itemGroup);
    }

    removeItem(index: number): void {
        if (this.items.length > 1) {
            this.items.removeAt(index);
        }
    }

    getMaxQuantity(index: number): number {
        const item = this.items.at(index);
        const productId = item.get('productId')?.value;

        if (!productId) return 999999;

        const product = this.products.find(p => p.id === productId);
        return product ? product.stock : 0;
    }

    onSubmit(): void {
        if (this.invoiceForm.invalid) return;

        if (this.items.length === 0) {
            this.snackBar.open('Adicione pelo menos um item à nota fiscal', 'Fechar', { duration: 3000 });
            return;
        }

        const request: CreateInvoiceRequest = {
            items: this.items.value.map((item: any) => ({
                productId: item.productId,
                quantity: item.quantity
            }))
        };

        this.invoiceService.create(request).subscribe({
            next: () => {
                this.snackBar.open('Nota fiscal criada com sucesso!', 'Fechar', { duration: 3000 });
                this.dialogRef.close('created');
            },
            error: (err) => {
                this.snackBar.open('Erro ao criar nota fiscal: ' + (err.error?.error || err.message), 'Fechar', { duration: 5000 });
            }
        });
    }

    onCancel(): void {
        this.dialogRef.close();
    }
}
