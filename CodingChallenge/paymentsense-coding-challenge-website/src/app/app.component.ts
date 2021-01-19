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
  public countries: ICountry[] = [];
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
          result.forEach(c => {
            c.cachedFlag = `https://localhost:5001/countries/${c.alpha3Code}/flag`;
            this.countries.push(c);
          });

          this.dataSource = new MatTableDataSource(this.countries);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        },
        _ => {
        });
  }

  public displayCountryData(alpha3Code: string) {
    const country = this.countries.find((c) => c.alpha3Code == alpha3Code);
    const messages = [];
    messages.push(`${country.name} (${country.alpha3Code})`);
    messages.push(`Population: ${country.population}`);
    messages.push(`Capital: ${country.capital}`);
    messages.push(`Timezone(s): ${country.timezones.join()}`);
    messages.push(`Currency(ies): ${country.currencies.map(function (c) { return `${c.code} ${c.name} (${c.symbol})` }).join()}`);
    messages.push(`Language(s): ${country.languages.join()}`);
    messages.push(`Neighbour(s): ${country.borders.join()}`);
    alert(messages.join('\r\n'));
  }
}
