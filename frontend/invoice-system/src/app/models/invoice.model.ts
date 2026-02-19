export enum InvoiceStatus {
    Open = 0,
    Closed = 1
}

export interface InvoiceItem {
    id?: number;
    invoiceId?: number;
    productId: number;
    productCode: string;
    productDescription: string;
    quantity: number;
}

export interface Invoice {
    id?: number;
    number: number;
    status: InvoiceStatus;
    createdAt: string;
    printedAt?: string;
    items: InvoiceItem[];
}

export interface CreateInvoiceItemRequest {
    productId: number;
    quantity: number;
}

export interface CreateInvoiceRequest {
    items: CreateInvoiceItemRequest[];
}
