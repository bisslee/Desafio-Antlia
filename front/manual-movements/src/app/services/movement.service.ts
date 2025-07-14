import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ManualMovement,
  GetManualMovementResponse,
  AddManualMovementRequest,
  AddManualMovementResponse
} from '../models';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MovementService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMovements(
    description?: string,
    startDate?: string,
    endDate?: string,
    minValue?: number,
    maxValue?: number,
    active?: boolean,
    offset?: number,
    page?: number,
    fieldName?: string,
    order?: string,
    pageSize?: number
  ): Observable<GetManualMovementResponse> {
    console.log('🔧 MovementService.getMovements() chamado');
    console.log('🔧 Environment useMockData:', environment.useMockData);

    let params = new HttpParams();

    if (description) params = params.set('Description', description);
    if (startDate) params = params.set('StartDate', startDate);
    if (endDate) params = params.set('EndDate', endDate);
    if (minValue !== undefined) params = params.set('MinValue', minValue.toString());
    if (maxValue !== undefined) params = params.set('MaxValue', maxValue.toString());
    if (active !== undefined) params = params.set('Active', active.toString());
    if (offset !== undefined) params = params.set('Offset', offset.toString());
    if (page !== undefined) params = params.set('Page', page.toString());
    if (fieldName) params = params.set('FieldName', fieldName);
    if (order) params = params.set('Order', order);
    if (pageSize !== undefined) params = params.set('PageSize', pageSize.toString());

    // Se a API não estiver disponível, usar dados mock diretamente
    if (environment.useMockData) {
      console.log('🔧 Usando dados mock (flag ativada)');
      return of(this.getMockMovements());
    }

    console.log('🔧 Fazendo chamada HTTP para:', `${this.baseUrl}/ManualMovement`);
    console.log('🔧 Parâmetros:', params.toString());

    return this.http.get<GetManualMovementResponse>(`${this.baseUrl}/ManualMovement`, { params })
      .pipe(
        // Fallback para dados mock se a API não estiver disponível
        catchError((error) => {
          console.warn('❌ API não disponível, usando dados mock. Erro:', error);
          return of(this.getMockMovements());
        })
      );
  }

  createMovement(movement: AddManualMovementRequest): Observable<AddManualMovementResponse> {
    return this.http.post<AddManualMovementResponse>(`${this.baseUrl}/ManualMovement`, movement)
      .pipe(
        catchError(() => {
          console.warn('API não disponível, simulando criação');
          return of({
            success: true,
            data: {
              id: 'mock-id-' + Date.now(),
              month: movement.month,
              year: movement.year,
              launchNumber: Math.floor(Math.random() * 1000) + 1,
              productCode: movement.productCode,
              cosifCode: movement.cosifCode,
              description: movement.description,
              movementDate: movement.movementDate,
              userCode: movement.userCode,
              value: movement.value
            },
            isSuccess: true,
            statusCode: 201,
            message: 'Movimento criado com sucesso (mock)',
            exception: null,
            errors: null
          });
        })
      );
  }

  getMovementById(id: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/ManualMovement/${id}`);
  }

  updateMovement(id: string, movement: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/ManualMovement/${id}`, movement);
  }

  deleteMovement(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/ManualMovement/${id}`);
  }

  private getMockMovements(): GetManualMovementResponse {
    return {
      success: true,
      data: [
        {
          id: '1',
          createdAt: '2024-01-15T10:00:00Z',
          createdBy: 'USER001',
          updatedAt: '2024-01-15T10:00:00Z',
          updatedBy: 'USER001',
          status: 'active' as any,
          month: 1,
          year: 2024,
          launchNumber: 1,
          productCode: 'SAV001',
          cosifCode: '1.1.1',
          description: 'Initial deposit adjustment',
          movementDate: '2024-01-15T10:00:00Z',
          userCode: 'USER001',
          value: 15250.00,
          product: {
            id: '1',
            createdAt: '2024-01-01T00:00:00Z',
            createdBy: 'SYSTEM',
            updatedAt: '2024-01-01T00:00:00Z',
            updatedBy: 'SYSTEM',
            status: 'active' as any,
            productCode: 'SAV001',
            description: 'Premium Savings Account'
          },
          productCosif: {
            id: '1',
            createdAt: '2024-01-01T00:00:00Z',
            createdBy: 'SYSTEM',
            updatedAt: '2024-01-01T00:00:00Z',
            updatedBy: 'SYSTEM',
            status: 'active' as any,
            productCode: 'SAV001',
            cosifCode: '1.1.1',
            classificationCode: 'Caixa e Banco'
          }
        },
        {
          id: '2',
          createdAt: '2024-01-16T14:30:00Z',
          createdBy: 'USER001',
          updatedAt: '2024-01-16T14:30:00Z',
          updatedBy: 'USER001',
          status: 'active' as any,
          month: 1,
          year: 2024,
          launchNumber: 2,
          productCode: 'CHK002',
          cosifCode: '1.2.1',
          description: 'Monthly maintenance fee reversal',
          movementDate: '2024-01-16T14:30:00Z',
          userCode: 'USER001',
          value: 25.00,
          product: {
            id: '2',
            createdAt: '2024-01-01T00:00:00Z',
            createdBy: 'SYSTEM',
            updatedAt: '2024-01-01T00:00:00Z',
            updatedBy: 'SYSTEM',
            status: 'active' as any,
            productCode: 'CHK002',
            description: 'Business Checking Account'
          },
          productCosif: {
            id: '2',
            createdAt: '2024-01-01T00:00:00Z',
            createdBy: 'SYSTEM',
            updatedAt: '2024-01-01T00:00:00Z',
            updatedBy: 'SYSTEM',
            status: 'active' as any,
            productCode: 'CHK002',
            cosifCode: '1.2.1',
            classificationCode: 'Ações'
          }
        }
      ],
      isSuccess: true,
      statusCode: 200,
      message: 'Dados mock carregados',
      exception: null,
      errors: null,
      page: 1,
      pageSize: 10,
      total: 2,
      totalPages: 1,
      hasPreviousPage: false,
      hasNextPage: false
    };
  }
}
