import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ICountry } from '../models/country';

@Injectable({
  providedIn: 'root'
})
export class PaymentsenseCodingChallengeApiService {
  constructor(private httpClient: HttpClient) {}

  public getHealth(): Observable<string> {
    return this.httpClient.get('https://localhost:5001/health', { responseType: 'text' });
  }

  public getCountries(): Observable<ICountry[]> {
    return this.httpClient.get<ICountry[]>('https://localhost:5001/countries');
  }
}
