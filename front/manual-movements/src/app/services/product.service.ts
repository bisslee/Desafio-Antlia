import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import {
  Product,
  GetProductResponse,
  ProductCosif,
  GetProductCosifResponse
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getProducts(
    productCode?: string,
    description?: string,
    active?: boolean,
    offset?: number,
    page?: number,
    fieldName?: string,
    order?: string
  ): Observable<GetProductResponse> {
    console.log('🔧 ProductService.getProducts() chamado');
    console.log('🔧 Environment useMockData:', environment.useMockData);

    let params = new HttpParams();

    if (productCode) params = params.set('ProductCode', productCode);
    if (description) params = params.set('Description', description);
    if (active !== undefined) params = params.set('Active', active.toString());
    if (offset !== undefined) params = params.set('Offset', offset.toString());
    if (page !== undefined) params = params.set('Page', page.toString());
    if (fieldName) params = params.set('FieldName', fieldName);
    if (order) params = params.set('Order', order);

    // Se a API não estiver disponível, usar dados mock diretamente
    if (environment.useMockData) {
      console.log('🔧 Usando produtos mock (flag ativada)');
      return of(this.getMockProducts());
    }

    console.log('🔧 Fazendo chamada HTTP para:', `${this.baseUrl}/Product`);
    console.log('🔧 Parâmetros:', params.toString());

    return this.http.get<GetProductResponse>(`${this.baseUrl}/Product`, { params })
      .pipe(
        catchError((error) => {
          console.warn('❌ API não disponível, usando produtos mock. Erro:', error);
          return of(this.getMockProducts());
        })
      );
  }

  getProductCosifs(
    productCode?: string,
    cosifCode?: string,
    classificationCode?: string,
    active?: boolean,
    offset?: number,
    page?: number,
    fieldName?: string,
    order?: string
  ): Observable<GetProductCosifResponse> {
    let params = new HttpParams();

    if (productCode) params = params.set('ProductCode', productCode);
    if (cosifCode) params = params.set('CosifCode', cosifCode);
    if (classificationCode) params = params.set('ClassificationCode', classificationCode);
    if (active !== undefined) params = params.set('Active', active.toString());
    if (offset !== undefined) params = params.set('Offset', offset.toString());
    if (page !== undefined) params = params.set('Page', page.toString());
    if (fieldName) params = params.set('FieldName', fieldName);
    if (order) params = params.set('Order', order);

    // Se a API não estiver disponível, usar dados mock diretamente
    if (environment.useMockData) {
      return of(this.getMockProductCosifs(productCode));
    }

    return this.http.get<GetProductCosifResponse>(`${this.baseUrl}/ProductCosif`, { params })
      .pipe(
        catchError(() => {
          console.warn('API não disponível, usando COSIFs mock');
          return of(this.getMockProductCosifs(productCode));
        })
      );
  }

  getProductById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/Product/${id}`);
  }

  getProductCosifById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/ProductCosif/${id}`);
  }

  private getMockProducts(): GetProductResponse {
    return {
      success: true,
      data: [
        {
          id: '1',
          createdAt: '2024-01-01T00:00:00Z',
          createdBy: 'SYSTEM',
          updatedAt: '2024-01-01T00:00:00Z',
          updatedBy: 'SYSTEM',
          status: 'active' as any,
          productCode: 'SAV001',
          description: 'Premium Savings Account'
        },
        {
          id: '2',
          createdAt: '2024-01-01T00:00:00Z',
          createdBy: 'SYSTEM',
          updatedAt: '2024-01-01T00:00:00Z',
          updatedBy: 'SYSTEM',
          status: 'active' as any,
          productCode: 'CHK002',
          description: 'Business Checking Account'
        },
        {
          id: '3',
          createdAt: '2024-01-01T00:00:00Z',
          createdBy: 'SYSTEM',
          updatedAt: '2024-01-01T00:00:00Z',
          updatedBy: 'SYSTEM',
          status: 'active' as any,
          productCode: 'INV003',
          description: 'Growth Investment Fund'
        },
        {
          id: '4',
          createdAt: '2024-01-01T00:00:00Z',
          createdBy: 'SYSTEM',
          updatedAt: '2024-01-01T00:00:00Z',
          updatedBy: 'SYSTEM',
          status: 'active' as any,
          productCode: 'CRD004',
          description: 'Platinum Credit Card'
        },
        {
          id: '5',
          createdAt: '2024-01-01T00:00:00Z',
          createdBy: 'SYSTEM',
          updatedAt: '2024-01-01T00:00:00Z',
          updatedBy: 'SYSTEM',
          status: 'active' as any,
          productCode: 'LON005',
          description: 'Personal Loan Premium'
        }
      ],
      isSuccess: true,
      statusCode: 200,
      message: 'Produtos mock carregados',
      exception: null,
      errors: null,
      page: 1,
      pageSize: 10,
      total: 5,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false
    };
  }

  private getMockProductCosifs(productCode?: string): GetProductCosifResponse {
    const allCosifs = [
      {
        id: '1',
        createdAt: '2024-01-01T00:00:00Z',
        createdBy: 'SYSTEM',
        updatedAt: '2024-01-01T00:00:00Z',
        updatedBy: 'SYSTEM',
        status: 'active' as any,
        productCode: 'SAV001',
        cosifCode: '1.1.1',
        classificationCode: 'Caixa e Banco'
      },
      {
        id: '2',
        createdAt: '2024-01-01T00:00:00Z',
        createdBy: 'SYSTEM',
        updatedAt: '2024-01-01T00:00:00Z',
        updatedBy: 'SYSTEM',
        status: 'active' as any,
        productCode: 'CHK002',
        cosifCode: '1.2.1',
        classificationCode: 'Ações'
      },
      {
        id: '3',
        createdAt: '2024-01-01T00:00:00Z',
        createdBy: 'SYSTEM',
        updatedAt: '2024-01-01T00:00:00Z',
        updatedBy: 'SYSTEM',
        status: 'active' as any,
        productCode: 'INV003',
        cosifCode: '1.3.1',
        classificationCode: 'Empréstimos'
      },
      {
        id: '4',
        createdAt: '2024-01-01T00:00:00Z',
        createdBy: 'SYSTEM',
        updatedAt: '2024-01-01T00:00:00Z',
        updatedBy: 'SYSTEM',
        status: 'active' as any,
        productCode: 'CRD004',
        cosifCode: '2.1.1',
        classificationCode: 'Depósitos'
      },
      {
        id: '5',
        createdAt: '2024-01-01T00:00:00Z',
        createdBy: 'SYSTEM',
        updatedAt: '2024-01-01T00:00:00Z',
        updatedBy: 'SYSTEM',
        status: 'active' as any,
        productCode: 'LON005',
        cosifCode: '2.2.1',
        classificationCode: 'Empréstimos'
      }
    ];

    const filteredCosifs = productCode
      ? allCosifs.filter(cosif => cosif.productCode === productCode)
      : allCosifs;

    return {
      success: true,
      data: filteredCosifs,
      isSuccess: true,
      statusCode: 200,
      message: 'COSIFs mock carregados',
      exception: null,
      errors: null,
      page: 1,
      pageSize: 10,
      total: filteredCosifs.length,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false
    };
  }
}
