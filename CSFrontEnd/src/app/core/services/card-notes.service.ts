import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FilterResponse } from '../models/api-response.model';
import { CardNote, CreateCardNoteRequest } from '../models/card-note.model';

@Injectable({ providedIn: 'root' })
export class CardNotesService {
  private readonly baseUrl = `${environment.apiUrl}/card-notes`;

  constructor(private readonly http: HttpClient) {}

  getByMedicalCard(medicalCardEntityId: string): Observable<CardNote[]> {
    return this.http
      .post<FilterResponse<CardNote>>(`${this.baseUrl}/filter`, [
        ['medicalCard', 'Equal', medicalCardEntityId],
      ])
      .pipe(map((res) => res.items));
  }

  create(request: CreateCardNoteRequest): Observable<CardNote> {
    return this.http.post<CardNote>(this.baseUrl, request);
  }
}
