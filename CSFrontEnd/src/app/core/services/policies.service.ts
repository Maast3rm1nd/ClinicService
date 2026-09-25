import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../models/api-response.model';
import { CreatePolicyRequest, Policy, UpdatePolicyRequest } from '../models/policy.model';

@Injectable({ providedIn: 'root' })
export class PoliciesService {
  private readonly baseUrl = `${environment.apiUrl}/policies`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Policy[]> {
    return this.http.get<ListResponse<Policy>>(this.baseUrl).pipe(map((res) => res.data));
  }

  create(request: CreatePolicyRequest): Observable<Policy> {
    return this.http.post<Policy>(this.baseUrl, request);
  }

  update(id: string, request: UpdatePolicyRequest): Observable<Policy> {
    return this.http.put<Policy>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  restore(id: string): Observable<Policy> {
    return this.http.post<Policy>(`${this.baseUrl}/${id}/restore`, {});
  }
}
