import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MessageService } from './message.service';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PlanService {
  public Url = 'https://localhost:32774/api/plan';

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) {}

  public extractData(res: any) {
    return res || {};
  }

  getPlan(params?: string): Observable<any> {
    if (params) {
      return this.httpClient
        .get(this.Url + '?' + params)
        .pipe(map(this.extractData));
    } else {
      return this.httpClient.get(this.Url).pipe(map(this.extractData));
    }
  }

  createPlan(
    planName: string,
    monthlyFee: number,
    numberOfMinutes: number
  ): Observable<any> {
    const body = {
      planName: planName,
      monthlyFee: monthlyFee,
      numberOfMinutes: numberOfMinutes,
    };

    return this.httpClient.post(this.Url, body).pipe(map(this.extractData));
  }

  createValidPlan(
    planName: string,
    monthlyFee: number,
    numberOfMinutes: number
  ): Observable<any> | null {
    if (!this.validateData(planName, monthlyFee, numberOfMinutes)) return null;
    return this.createPlan(planName, monthlyFee, numberOfMinutes);
  }

  validateData(
    planName: string,
    monthlyFee: number,
    numberOfMinutes: number
  ): boolean {
    if (planName == null) {
      this.log("ERROR: Plan Name can't be null.");
      return false;
    }
    if (monthlyFee == null) {
      this.log("ERROR: Monthly fee can't be null.");
      return false;
    }
    if (numberOfMinutes == null) {
      this.log("ERROR: Number of minutes can't be null.");
      return false;
    }
    return true;
  }

  log(message: string) {
    this.messageService.add(`Created: ${message}`);
  }
}
