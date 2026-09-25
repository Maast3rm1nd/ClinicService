import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../models/api-response.model';
import {
  CreateSpecialisationRequest,
  Specialisation,
  UpdateSpecialisationRequest,
} from '../models/specialisation.model';

@Injectable({ providedIn: 'root' })
export class SpecialisationsService {
  private readonly baseUrl = `${environment.apiUrl}/specialisations`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Specialisation[]> {
    return this.http.get<ListResponse<Specialisation>>(this.baseUrl).pipe(map((res) => res.data));
  }

  create(request: CreateSpecialisationRequest): Observable<Specialisation> {
    return this.http.post<Specialisation>(this.baseUrl, request);
  }

  update(id: string, request: UpdateSpecialisationRequest): Observable<Specialisation> {
    return this.http.put<Specialisation>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
