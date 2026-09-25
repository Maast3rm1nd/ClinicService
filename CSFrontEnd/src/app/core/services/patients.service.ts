import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../models/api-response.model';
import { Patient } from '../models/patient.model';

@Injectable({ providedIn: 'root' })
export class PatientsService {
  private readonly baseUrl = `${environment.apiUrl}/patients`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Patient[]> {
    return this.http.get<ListResponse<Patient>>(this.baseUrl).pipe(map((res) => res.data));
  }

  getById(id: string): Observable<Patient> {
    return this.http.get<Patient>(`${this.baseUrl}/${id}`);
  }
}
