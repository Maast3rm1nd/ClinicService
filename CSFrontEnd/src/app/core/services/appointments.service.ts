import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Appointment, CreateAppointmentRequest, UpdateAppointmentRequest } from '../models/appointment.model';
import { FilterResponse, ListResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class AppointmentsService {
  private readonly baseUrl = `${environment.apiUrl}/appointments`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Appointment[]> {
    return this.http.get<ListResponse<Appointment>>(this.baseUrl).pipe(map((res) => res.data));
  }

  filter(
    filter: unknown[],
    options: { takeCount?: number; sortBy?: string; sortDesc?: boolean } = {},
  ): Observable<Appointment[]> {
    let params = new HttpParams();
    if (options.takeCount !== undefined) {
      params = params.set('takeCount', options.takeCount);
    }
    if (options.sortBy) {
      params = params.set('sortBy', options.sortBy).set('sortDesc', options.sortDesc ?? false);
    }
    return this.http
      .post<FilterResponse<Appointment>>(`${this.baseUrl}/filter`, filter, { params })
      .pipe(map((res) => res.items));
  }

  getById(id: string): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateAppointmentRequest): Observable<Appointment> {
    return this.http.post<Appointment>(this.baseUrl, request);
  }

  update(id: string, request: UpdateAppointmentRequest): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.baseUrl}/${id}`, request);
  }

  cancel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  restore(id: string): Observable<Appointment> {
    return this.http.post<Appointment>(`${this.baseUrl}/${id}/restore`, {});
  }
}
