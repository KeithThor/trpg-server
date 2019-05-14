import { Component, Input, OnInit, OnDestroy } from "@angular/core";
import { Subscription, Observable } from "rxjs";

/**Component responsible for displaying the amount of time left in a turn in battle. */
@Component({
  selector: "game-timer-component",
  templateUrl: "./timer.component.html",
  styleUrls: ["./timer.component.css"]
})
export class TimerComponent implements OnInit, OnDestroy {

  private _timeLeft: number;
  public get timeLeft(): number {
    return this._timeLeft;
  }
  @Input() public set timeLeft(value: number) {
    this.destroyTimer();
    this._timeLeft = value;
    this.startTimer();
  }

  private timerSubscription: Subscription;

  ngOnInit() {
    
  }

  ngOnDestroy() {
    this.destroyTimer();
  }

  /**Creates and starts a timer that ticks every second and reduces the amount of time left. */
  private startTimer(): void {
    let timer = Observable.timer(1000, 1000);
    this.timerSubscription = timer.subscribe({
      next: (val) => {
        if (this.timeLeft == null || this.timeLeft <= 0) return;
        else this.timeLeft--;
      }
    });
  }

  /**Destroys the timer subscription, if one exists, to prevent memory leaks. */
  private destroyTimer(): void {
    if (this.timerSubscription != null) this.timerSubscription.unsubscribe();
  }
}
