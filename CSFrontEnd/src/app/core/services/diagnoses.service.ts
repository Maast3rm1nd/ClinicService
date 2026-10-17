import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FilterResponse, ListResponse } from '../models/api-response.model';
import { Diagnosis } from '../models/diagnosis.model';

@Injectable({ providedIn: 'root' })
export class DiagnosesService {
  private readonly baseUrl = `${environment.apiUrl}/diagnoses`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Diagnosis[]> {
    return this.http.get<ListResponse<Diagnosis>>(this.baseUrl).pipe(map((res) => res.data));
  }

  getByMedicalCard(medicalCardEntityId: string): Observable<Diagnosis[]> {
    return this.http
      .post<FilterResponse<Diagnosis>>(`${this.baseUrl}/filter`, [
        ['medicalCard', 'Equal', medicalCardEntityId],
      ])
      .pipe(map((res) => res.items));
  }
}
