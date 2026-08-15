import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-countdown',
  standalone: false,
  templateUrl: './countdown.html',
  styleUrl: './countdown.css',
})
export class Countdown {

  @Input() counterResult: any = {};
  @Input() counterName : string = "";
}
