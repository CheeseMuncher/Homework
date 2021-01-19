import { Currency } from './currency';

export interface ICountry {
  alpha3Code: string;
  name: string;
  flag: string;
  population: number;
  timeZones: string[];
  currencies: Currency[];
  languages: string[];
  capital: string;
  borders: string[];
  cachedFlag: string;
}
