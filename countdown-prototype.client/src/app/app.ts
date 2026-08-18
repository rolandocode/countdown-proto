import { HttpClient } from '@angular/common/http';
import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App implements OnInit, OnDestroy {
  public forecasts: WeatherForecast[] = [];
  public counterResult: any = {};
  public counterApiResult: any = {};
  public workResult: any = {};
  public weekendResult: any = {};
  public yearResult: any = {};
  public hourResult: any = {};
  public monthResult: any = {};
  public payrollResult: any = {};
  public pause: boolean = false;

  // Controls when the timer ring is animated
  public isAnimating: boolean = false;
  // Indicates whether an active request is in-flight
  public isLoading: boolean = false;

  private timerId?: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.fetchDataAndStartTimer();
  }

  fetchDataAndStartTimer() {
    // Prevent overlapping requests
    if (this.isLoading) return;

    this.stopTimer();
    this.isAnimating = false;
    this.isLoading = true;

    this.getPercentage();
  }

  startTimer() {
    this.stopTimer();
    this.timerId = setInterval(() => {
      if (!this.pause) {
        this.fetchDataAndStartTimer();
      }
    }, 10000);
  }

  stopTimer() {
    if (this.timerId) {
      clearInterval(this.timerId);
      this.timerId = undefined;
    }
  }

  togglePause() {
    this.pause = !this.pause;

    if (!this.pause) {
      // Unpaused: trigger API and reset timer loop
      this.fetchDataAndStartTimer();
    } else {
      // Paused: stop interval and halt animation
      this.stopTimer();
      this.isAnimating = false;
    }
  }

  getPercentage() {
    forkJoin({
      counter: this.http.get('/counter'),
      counterApi: this.http.get('/counter/day'),
      work: this.http.get('/counter/work'),
      weekend: this.http.get('/counter/weekend'),
      year: this.http.get('/counter/year'),
      hour: this.http.get('/counter/hour'),
      month: this.http.get('/counter/month'),
      payroll: this.http.get('/counter/payroll')
    })
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: (result) => {
          this.counterResult = result.counter;
          this.counterApiResult = result.counterApi;
          this.workResult = result.work;
          this.weekendResult = result.weekend;
          this.yearResult = result.year;
          this.hourResult = result.hour;
          this.monthResult = result.month;
          this.payrollResult = result.payroll;

          // Start 10s ring animation and timer cycle ONLY after server responds
          if (!this.pause) {
            this.isAnimating = true;
            this.startTimer();
          }
        },
        error: (error) => {
          console.error(error);
          // Retry fetch cycle after interval even on error
          if (!this.pause) {
            this.startTimer();
          }
        }
      });
  }

  ngOnDestroy() {
    this.stopTimer();
  }

  protected readonly title = signal('countdown-prototype.client');
}
