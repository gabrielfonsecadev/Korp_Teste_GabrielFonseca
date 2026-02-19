import { Component, Inject, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { ProductService } from '../../services/product.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'app-product-form',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatCardModule,
        MatSnackBarModule
    ],
    templateUrl: './product-form.component.html',
    styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {
    productForm: FormGroup;
    isEditMode = false;
    productId?: number;
    private dialogRef = inject(MatDialogRef<ProductFormComponent>);

    constructor(
        private fb: FormBuilder,
        private productService: ProductService,
        private router: Router,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        @Inject(MAT_DIALOG_DATA) public data: { id?: number }
    ) {
        this.productForm = this.fb.group({
            code: ['', [Validators.required, Validators.maxLength(50)]],
            description: ['', [Validators.required, Validators.maxLength(200)]],
            stock: [0, [Validators.required, Validators.min(0)]]
        });
        if (this.data.id) {
            this.productId = this.data.id;
        }
    }

    ngOnInit(): void {
        if (this.productId) {
            this.loadProduct(this.productId);
        }
    }

    loadProduct(id: number): void {
        this.productService.getById(id).subscribe({
            next: (product) => {
                this.productForm.patchValue(product);
            },
            error: (err) => {
                this.snackBar.open('Erro ao carregar produto', 'Fechar', { duration: 3000 });
                this.router.navigate(['/products']);
            }
        });
    }

    onSubmit(): void {
        if (this.productForm.invalid) return;

        const product = this.productForm.value;

        if (this.productId) {
            this.productService.update(this.productId, product).subscribe({
                next: () => {
                    this.snackBar.open('Produto atualizado com sucesso!', 'Fechar', { duration: 3000 });
                    this.dialogRef.close("updated");
                },
                error: (err) => {
                    this.snackBar.open('Erro ao atualizar produto: ' + (err.error?.error || err.message), 'Fechar', { duration: 5000 });
                }
            });
        } else {
            this.productService.create(product).subscribe({
                next: () => {
                    this.snackBar.open('Produto criado com sucesso!', 'Fechar', { duration: 3000 });
                    this.dialogRef.close("created");
                },
                error: (err) => {
                    this.snackBar.open('Erro ao criar produto: ' + (err.error?.error || err.message), 'Fechar', { duration: 5000 });
                }
            });
        }
    }

    onCancel(): void {
        this.dialogRef.close();
    }
}
