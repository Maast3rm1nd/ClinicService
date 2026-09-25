import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../models/api-response.model';
import { CreatePersonRequest, Person, UpdatePersonRequest } from '../models/person.model';

@Injectable({ providedIn: 'root' })
export class PersonsService {
  private readonly baseUrl = `${environment.apiUrl}/persons`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Person[]> {
    return this.http.get<ListResponse<Person>>(this.baseUrl).pipe(map((res) => res.data));
  }

  getById(id: string): Observable<Person> {
    return this.http.get<Person>(`${this.baseUrl}/${id}`);
  }

  create(request: CreatePersonRequest): Observable<Person> {
    return this.http.post<Person>(this.baseUrl, request);
  }

  update(id: string, request: UpdatePersonRequest): Observable<Person> {
    return this.http.put<Person>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
