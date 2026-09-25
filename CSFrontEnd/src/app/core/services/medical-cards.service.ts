import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ListResponse } from '../models/api-response.model';
import { MedicalCard } from '../models/medical-card.model';

@Injectable({ providedIn: 'root' })
export class MedicalCardsService {
  private readonly baseUrl = `${environment.apiUrl}/medical-cards`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<MedicalCard[]> {
    return this.http.get<ListResponse<MedicalCard>>(this.baseUrl).pipe(map((res) => res.data));
  }
}
