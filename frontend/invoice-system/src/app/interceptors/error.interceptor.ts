import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const snackBar = inject(MatSnackBar);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            let errorMessage = 'Ocorreu um erro inesperado';

            if (error.status === 0) {
                // A client-side or network error occurred.
                errorMessage = 'Não foi possível completar a ação. Por favor, verifique sua conexão ou tente novamente mais tarde.';
            } else if (error.status === 404) {
                errorMessage = 'O recurso solicitado não foi encontrado.';
            } else {
                // The backend returned an unsuccessful response code.
                errorMessage = error.error?.error || 'Ocorreu um erro no processamento. Por favor, tente novamente.';
            }

            snackBar.open(errorMessage, 'Fechar', {
                duration: 5000,
                horizontalPosition: 'end',
                verticalPosition: 'bottom',
                panelClass: ['error-snackbar']
            });

            return throwError(() => error);
        })
    );
};
