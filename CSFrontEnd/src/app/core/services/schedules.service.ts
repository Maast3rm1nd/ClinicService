import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../models/api-response.model';
import { CreateScheduleRequest, Schedule, UpdateScheduleRequest } from '../models/schedule.model';

@Injectable({ providedIn: 'root' })
export class SchedulesService {
  private readonly baseUrl = `${environment.apiUrl}/schedules`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Schedule[]> {
    return this.http.get<ListResponse<Schedule>>(this.baseUrl).pipe(map((res) => res.data));
  }

  create(request: CreateScheduleRequest): Observable<Schedule> {
    return this.http.post<Schedule>(this.baseUrl, request);
  }

  update(id: string, request: UpdateScheduleRequest): Observable<Schedule> {
    return this.http.put<Schedule>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
