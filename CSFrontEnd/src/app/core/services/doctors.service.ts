import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../models/api-response.model';
import { CreateDoctorRequest, Doctor, UpdateDoctorRequest } from '../models/doctor.model';

@Injectable({ providedIn: 'root' })
export class DoctorsService {
  private readonly baseUrl = `${environment.apiUrl}/doctors`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Doctor[]> {
    return this.http.get<ListResponse<Doctor>>(this.baseUrl).pipe(map((res) => res.data));
  }

  getById(id: string): Observable<Doctor> {
    return this.http.get<Doctor>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateDoctorRequest): Observable<Doctor> {
    return this.http.post<Doctor>(this.baseUrl, request);
  }

  update(id: string, request: UpdateDoctorRequest): Observable<Doctor> {
    return this.http.put<Doctor>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
