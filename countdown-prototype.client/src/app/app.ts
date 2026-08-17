import { HttpClient } from '@angular/common/http';
import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { forkJoin } from 'rxjs';

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
  public renderTimer: boolean = true;

  private timerId?: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getPercentage();
    this.startTimer();
  }

  startTimer() {
    this.stopTimer();
    this.timerId = setInterval(() => {
      if (!this.pause) {
        this.getPercentage();
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
      // Unpaused: trigger API, reset interval timer
      this.getPercentage();
      this.startTimer();

      // Force SVG animation reset by toggling render flag
      this.renderTimer = false;
      setTimeout(() => {
        this.renderTimer = true;
      }, 0);
    } else {
      // Paused: stop interval timer
      this.stopTimer();
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
    }).subscribe({
      next: (result) => {
        this.counterResult = result.counter;
        this.counterApiResult = result.counterApi;
        this.workResult = result.work;
        this.weekendResult = result.weekend;
        this.yearResult = result.year;
        this.hourResult = result.hour;
        this.monthResult = result.month;
        this.payrollResult = result.payroll;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  ngOnDestroy() {
    this.stopTimer();
  }

  protected readonly title = signal('countdown-prototype.client');
}
