import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Appointment, CreateAppointmentRequest, UpdateAppointmentRequest } from '../models/appointment.model';
import { ListResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class AppointmentsService {
  private readonly baseUrl = `${environment.apiUrl}/appointments`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Appointment[]> {
    return this.http.get<ListResponse<Appointment>>(this.baseUrl).pipe(map((res) => res.data));
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
