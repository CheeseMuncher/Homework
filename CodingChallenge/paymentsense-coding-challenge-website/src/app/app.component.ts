import { Component, ViewChild } from '@angular/core';
import { PaymentsenseCodingChallengeApiService } from './services';
import { take } from 'rxjs/operators';
import { faThumbsUp, faThumbsDown } from '@fortawesome/free-regular-svg-icons';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { ICountry } from './models/country';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  public faThumbsUp = faThumbsUp;
  public faThumbsDown = faThumbsDown;
  public title = 'Paymentsense Coding Challenge!';
  public paymentsenseCodingChallengeApiIsActive = false;
  public paymentsenseCodingChallengeApiActiveIcon = this.faThumbsDown;
  public paymentsenseCodingChallengeApiActiveIconColour = 'red';
  public displayedColumns = ['Name', 'Flag'];
  public dataSource: MatTableDataSource<ICountry>;

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  constructor(private paymentsenseCodingChallengeApiService: PaymentsenseCodingChallengeApiService) {
    paymentsenseCodingChallengeApiService.getHealth().pipe(take(1))
    .subscribe(
      apiHealth => {
        this.paymentsenseCodingChallengeApiIsActive = apiHealth === 'Healthy';
        this.paymentsenseCodingChallengeApiActiveIcon = this.paymentsenseCodingChallengeApiIsActive
          ? this.faThumbsUp
          : this.faThumbsUp;
        this.paymentsenseCodingChallengeApiActiveIconColour = this.paymentsenseCodingChallengeApiIsActive
          ? 'green'
          : 'red';
      },
      _ => {
        this.paymentsenseCodingChallengeApiIsActive = false;
        this.paymentsenseCodingChallengeApiActiveIcon = this.faThumbsDown;
        this.paymentsenseCodingChallengeApiActiveIconColour = 'red';
      });

    paymentsenseCodingChallengeApiService.getCountries().pipe(take(1))
      .subscribe(
        result => {
          const countries: ICountry[] = [];
          result.forEach(c => countries.push(createNewCountry(c.alpha3Code, c.name, c.flag)));

          this.dataSource = new MatTableDataSource(countries);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        },
        _ => {
        });
  }
}

function createNewCountry(alpha3Code: string, name: string, flag: string): ICountry {
  return {
    alpha3Code: alpha3Code,
    name: name,
    flag: flag
  };
}
