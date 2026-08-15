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



  private timerId?: number;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    // Load immediately
    this.getPercentage();

    // Refresh every 10 seconds
    this.timerId = window.setInterval(() => {
      this.getPercentage();
    }, 10000);
  }

  getPercentage() {
    forkJoin({
      counter: this.http.get('/counter'),
      counterApi: this.http.get('/counter/day'),
      work: this.http.get('/counter/work'),
      weekend: this.http.get('/counter/weekend'),
      year: this.http.get('/counter/year'),
      hour: this.http.get('/counter/hour')
    }).subscribe({
      next: (result) => {
        this.counterResult = result.counter;
        this.counterApiResult = result.counterApi;
        this.workResult = result.work;
        this.weekendResult = result.weekend;
        this.yearResult = result.year;
        this.hourResult = result.hour;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  ngOnDestroy() {
    if (this.timerId) {
      window.clearInterval(this.timerId);
    }
  }

  protected readonly title = signal('countdown-prototype.client');
}
