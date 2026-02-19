import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice, CreateInvoiceRequest } from '../models/invoice.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class InvoiceService {
    private apiUrl = `${environment.billingApiUrl}/Invoices`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<Invoice[]> {
        return this.http.get<Invoice[]>(this.apiUrl);
    }

    getById(id: number): Observable<Invoice> {
        return this.http.get<Invoice>(`${this.apiUrl}/${id}`);
    }

    create(request: CreateInvoiceRequest): Observable<Invoice> {
        return this.http.post<Invoice>(this.apiUrl, request);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    print(id: number): Observable<Invoice> {
        return this.http.post<Invoice>(`${this.apiUrl}/${id}/print`, {});
    }
}
