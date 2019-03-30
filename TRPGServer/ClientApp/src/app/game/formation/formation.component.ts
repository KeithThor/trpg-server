import { Component, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Formation } from "../model/formation.model";

@Component({
  selector: 'game-formation-component',
  templateUrl: './formation.component.html',
  styleUrls: ['./formation.component.css']
})
export class FormationComponent implements OnInit {
  constructor(private http: HttpClient) {

  }

  public formations: Formation[];

  ngOnInit(): void {
    this.http.get<Formation[]>("api/formation").subscribe({
      next: (formations: Formation[]) => {
        this.formations = formations;
      }
    });
  }
}
