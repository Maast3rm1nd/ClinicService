import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../models/api-response.model';
import {
  CreateInsuranceProviderRequest,
  InsuranceProvider,
  UpdateInsuranceProviderRequest,
} from '../models/insurance-provider.model';

@Injectable({ providedIn: 'root' })
export class InsuranceProvidersService {
  private readonly baseUrl = `${environment.apiUrl}/insurance-providers`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<InsuranceProvider[]> {
    return this.http.get<ListResponse<InsuranceProvider>>(this.baseUrl).pipe(map((res) => res.data));
  }

  create(request: CreateInsuranceProviderRequest): Observable<InsuranceProvider> {
    return this.http.post<InsuranceProvider>(this.baseUrl, request);
  }

  update(id: string, request: UpdateInsuranceProviderRequest): Observable<InsuranceProvider> {
    return this.http.put<InsuranceProvider>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  restore(id: string): Observable<InsuranceProvider> {
    return this.http.post<InsuranceProvider>(`${this.baseUrl}/${id}/restore`, {});
  }
}
